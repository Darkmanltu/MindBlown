using MindBlown.Types;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using MindBlown.Interfaces;

namespace MindBlown.Tests
{
    public class TestEntity : IHasGuidId
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }

    public class RepositoryTests
    {
        [Fact]
        public void Constructor_InitializesWithEmptyList()
        {
            
            var repository = new Repository<TestEntity>();

            
            Assert.Empty(repository.GetAll());
        }

        [Fact]
        public void Constructor_WithListParameter_InitializesWithItems()
        {
            
            var items = new List<TestEntity>
            {
                new TestEntity { Name = "Item1" },
                new TestEntity { Name = "Item2" }
            };

            
            var repository = new Repository<TestEntity>(items);

            // Assert
            Assert.Equal(2, repository.Count());
            Assert.Contains(items[0], repository.GetAll());
            Assert.Contains(items[1], repository.GetAll());
        }

        [Fact]
        public void Add_AddsItemToRepository()
        {
            
            var repository = new Repository<TestEntity>();
            var item = new TestEntity { Name = "NewItem" };

            
            repository.Add(item);

            
            Assert.Contains(item, repository.GetAll());
            Assert.Equal(1, repository.Count());
        }

        [Fact]
        public void Remove_RemovesItemFromRepository()
        {

            var repository = new Repository<TestEntity>();
            var item = new TestEntity { Name = "ToBeRemoved" };
            repository.Add(item);

            
            repository.Remove(item);

            
            Assert.DoesNotContain(item, repository.GetAll());
            Assert.Equal(0, repository.Count());
        }

        [Fact]
        public void GetById_ReturnsCorrectItem()
        {
            
            var repository = new Repository<TestEntity>();
            var item = new TestEntity { Name = "FindMe" };
            repository.Add(item);

            
            var retrievedItem = repository.GetById(item.Id);

            
            Assert.Equal(item, retrievedItem);
        }

        [Fact]
        public void GetById_ReturnsNullForNonExistentId()
        {
            
            var repository = new Repository<TestEntity>();
            var nonExistentId = Guid.NewGuid();

            
            var retrievedItem = repository.GetById(nonExistentId);

            
            Assert.Null(retrievedItem);
        }

        [Fact]
        public void GetAll_ReturnsAllItems()
        {
            
            var items = new List<TestEntity>
            {
                new TestEntity { Name = "Item1" },
                new TestEntity { Name = "Item2" }
            };
            var repository = new Repository<TestEntity>(items);

            
            var allItems = repository.GetAll();

            
            Assert.Equal(2, allItems.Count());
            Assert.Contains(items[0], allItems);
            Assert.Contains(items[1], allItems);
        }

        [Fact]
        public void Count_ReturnsCorrectItemCount()
        {
            
            var repository = new Repository<TestEntity>();
            var item = new TestEntity { Name = "CountMe" };
            repository.Add(item);

            
            var count = repository.Count();

            
            Assert.Equal(1, count);
        }

        [Fact]
        public void Indexer_GetsAndSetsItemAtIndex()
        {
            
            var repository = new Repository<TestEntity>();
            var item1 = new TestEntity { Name = "Item1" };
            var item2 = new TestEntity { Name = "Item2" };
            repository.Add(item1);

            
            Assert.Equal(item1, repository[0]);

            
            repository[0] = item2;
            Assert.Equal(item2, repository[0]);
        }
    }
}
