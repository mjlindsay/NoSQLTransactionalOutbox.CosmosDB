using MediatR;
using Microsoft.Azure.Cosmos;
using Moq;
using NoSQLTransactionalOutbox.CosmosDB.Context;
using NoSQLTransactionalOutbox.CosmosDB.Tests.Fixtures;

namespace NoSQLTransactionalOutbox.CosmosDB.Tests
{
    public class DefaultCosmosContainerContextTests
    {
        Mock<Container> Container { get; init; }

        DefaultCosmosContainerContext Context { get; init; }

        public DefaultCosmosContainerContextTests() {
            Container = new Mock<Container>();


            Context = new DefaultCosmosContainerContext(Container.Object, new Mock<IMediator>().Object);
        }

        [Fact]
        public void CanAddDataPersistenceObject() {
            // Arrange
            const string entityId = "testId";
            var entity = new FakeEntity() {
                Id = entityId
            };

            var dpo = new DataPersistenceObject<FakeEntity> {
                Id = entityId,
                Data = entity,
                PartitionKey = entityId, // Same as ID
                EntityType = "test-entity",
                EntityState = EntityState.Created,
                Ttl = -1
            };

            // Act
            Context.AddOrReplace(dpo);

            // Assert
            Assert.Equal(dpo, Context.DataObjects.First());
        }

        [Fact]
        public void CanReplaceDataPersistenceObject() {
            // Arrange
            const string entityId = "testId";
            var entity = new FakeEntity() {
                Id = entityId
            };

            var originalDpo = new DataPersistenceObject<FakeEntity> {
                Id = entityId,
                Data = entity,
                PartitionKey = entityId,
                EntityType = "test-entity",
                EntityState = EntityState.Created,
                Ttl = -1
            };

            var replacementDpo = new DataPersistenceObject<FakeEntity> {
                Id = entityId,
                Data = entity,
                PartitionKey = entityId,
                EntityType = "test-entity",
                EntityState = EntityState.Created,
                Ttl = 100
            };

            // Act
            Context.AddOrReplace(originalDpo);
            Context.AddOrReplace(replacementDpo);

            Assert.Equal(replacementDpo, Context.DataObjects.First());
        }

        [Fact]
        public void CanResetDataObjects() {
            // Arrange
            const string entityId = "testId";
            var entity = new FakeEntity() {
                Id = entityId
            };

            var dpo = new DataPersistenceObject<FakeEntity> {
                Id = entityId,
                Data = entity,
                PartitionKey = entityId, // Same as ID
                EntityType = "test-entity",
                EntityState = EntityState.Created,
                Ttl = -1
            };

            // Act
            Context.AddOrReplace(dpo);
            Context.Reset();

            // Assert
            Assert.Empty(Context.DataObjects);
        }
    }
}