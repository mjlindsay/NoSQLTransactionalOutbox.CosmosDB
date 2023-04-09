using Microsoft.Azure.Cosmos;
using NoSQLTransactionalOutbox.CosmosDB.Exceptions;
using NoSQLTransactionalOutbox.Core.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NoSQLTransactionalOutbox.Core.Event;
using MediatR;

namespace NoSQLTransactionalOutbox.CosmosDB.Context
{
    public class DefaultCosmosContainerContext : IContainerContext
    {
        private Container _container;
        private IMediator _mediator;

        private List<DataPersistenceObject<IEntity>> DataObjects = new();

        public DefaultCosmosContainerContext(Container container, IMediator mediator) {
            _container = container;
            _mediator = mediator;
        }

        public void Add(DataPersistenceObject<IEntity> entity) {
            // Add only if we do not have another entity in the collection
            int matchingEntityCount =
                DataObjects.Where(dpo => dpo.Id == entity.Id && dpo.PartitionKey == entity.PartitionKey).Count();

            if (matchingEntityCount == 0) {
                DataObjects.Add(entity);
            }
        }

        public void Reset() {
            DataObjects.Clear();           
        }

        private IEnumerable<IGrouping<string, DataPersistenceObject<IEntity>>> GetDataPersistenceObjectsByPartitionKey()
            => DataObjects.GroupBy(dpo => dpo.PartitionKey);

        public async Task<IEnumerable<DataPersistenceObject<IEntity>>> SaveChangesAsync(CancellationToken cancellationToken = default) {
            switch (DataObjects.Count) {
                case 1:
                    var resultItem = await SaveSingleAsync(DataObjects.First(), cancellationToken);
                    var resultList = new List<DataPersistenceObject<IEntity>> { resultItem };
                    return resultList;
                case > 1:
                    var result = await SaveInTransactionalBatchAsync(cancellationToken);
                    return result;
                default:
                    return new List<DataPersistenceObject<IEntity>>();
            }
        }

        private async Task<DataPersistenceObject<IEntity>> SaveSingleAsync(
            DataPersistenceObject<IEntity> dataPersistenceObject,
            CancellationToken cancellationToken) {

            var reqOptions = new ItemRequestOptions {
                EnableContentResponseOnWrite = false
            };

            if (!string.IsNullOrWhiteSpace(dataPersistenceObject.ETag))
                reqOptions.IfMatchEtag = dataPersistenceObject.ETag;

            var pk = new PartitionKey(dataPersistenceObject.PartitionKey);

            try {
                ItemResponse<DataPersistenceObject<IEntity>> response;

                switch (dataPersistenceObject.EntityState) {
                    case EntityState.Created:
                        response = await _container.CreateItemAsync(dataPersistenceObject, pk, reqOptions, cancellationToken);
                        break;
                    case EntityState.Updated:
                    case EntityState.Deleted:
                        response = await _container.ReplaceItemAsync(dataPersistenceObject, dataPersistenceObject.Id, pk, reqOptions, cancellationToken);
                        break;
                    default:
                        DataObjects.Clear();
                        return new DataPersistenceObject<IEntity>();
                }

                dataPersistenceObject.ETag = response.ETag;

                // work has been successfully done - reset DataObjects list
                DataObjects.Clear();
                return dataPersistenceObject;
            } catch (CosmosException e) {
                // Not recoverable - clear context
                DataObjects.Clear();
                throw EvaluateCosmosError(e, Guid.Parse(dataPersistenceObject.Id), dataPersistenceObject.ETag);
            }
        }

        private async Task<List<DataPersistenceObject<IEntity>>> SaveInTransactionalBatchAsync(CancellationToken cancellationToken = default) {

            var partitionKey = new PartitionKey(DataObjects.First().PartitionKey);
            var transactionalBatch = _container.CreateTransactionalBatch(partitionKey);

            foreach (var dataPersistenceObject in DataObjects) {
                TransactionalBatchItemRequestOptions tbItemRequestOptions = new TransactionalBatchItemRequestOptions();

                if (!string.IsNullOrWhiteSpace(dataPersistenceObject.ETag))
                    tbItemRequestOptions.IfMatchEtag = dataPersistenceObject.ETag;

                switch (dataPersistenceObject.EntityState) {
                    case EntityState.Created:
                        transactionalBatch.CreateItem(dataPersistenceObject);
                        break;
                    case EntityState.Updated:
                    case EntityState.Deleted:
                        transactionalBatch.ReplaceItem(dataPersistenceObject.Id, dataPersistenceObject, tbItemRequestOptions);
                        break;
                }
            }

            var transactionalBatchResult = await transactionalBatch.ExecuteAsync(cancellationToken);
            if (!transactionalBatchResult.IsSuccessStatusCode) {
                for (int dataObjectIndex = 0; dataObjectIndex < DataObjects.Count; dataObjectIndex++) {
                    if (transactionalBatchResult[dataObjectIndex].StatusCode != System.Net.HttpStatusCode.FailedDependency) {
                        DataObjects.Clear();
                        throw EvaluateCosmosError(transactionalBatchResult[dataObjectIndex].StatusCode);
                    }
                }
            }

            // Create a copy of our current data objects, update etags from transactional batch, then clear
            // the data objects and return the list copy.
            var resultObjects = new List<DataPersistenceObject<IEntity>>(DataObjects);

            for (int dataObjectIndex = 0; dataObjectIndex < DataObjects.Count; dataObjectIndex++) {
                resultObjects[dataObjectIndex].ETag = transactionalBatchResult[dataObjectIndex].ETag;
            }

            DataObjects.Clear();

            return resultObjects;
        }

        private void RaiseDomainEvents(IEnumerable<DataPersistenceObject<IEntity>> dataPersistenceObjects) {
            var eventEmitters = dataPersistenceObjects
                .Where(dpo => dpo.Data is IEventEmitter<IEvent> ee)
                .Select(dpo => dpo.Data as IEventEmitter<IEvent>);

            if (eventEmitters.Count() <= 0)
                return;

            foreach (var domainEvents in eventEmitters.SelectMany(eventEmitter => eventEmitter.DomainEvents))
                _mediator.Publish(domainEvents);
        }

        private Exception EvaluateCosmosError(CosmosException error, Guid? id = null, string etag = null) {
            return EvaluateCosmosError(error.StatusCode, id, etag);
        }

        private Exception EvaluateCosmosError(HttpStatusCode statusCode, Guid? id = null, string etag = null) {
            return statusCode switch {
                HttpStatusCode.NotFound => new DomainObjectNotFoundException(
                    $"Domain object not found for Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.NotModified => new DomainObjectNotModifiedException(
                    $"Domain object not modified. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.Conflict => new DomainObjectConflictException(
                    $"Domain object conflict detected. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.PreconditionFailed => new DomainObjectPreconditionFailedException(
                    $"Domain object mid-air collision detected. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.TooManyRequests => new DomainObjectTooManyRequestsException(
                    "Too many requests occurred. Try again later)"),
                _ => new Exception("Cosmos Exception")
            };
        }
    }
}
