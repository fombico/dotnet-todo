using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests.Services
{
    public class TodoServiceFacts
    {
        public class Add
        {
            private readonly DbContextOptions<TodoContext> _dbContextOptions;

            public Add()
            {
                _dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }
            
            [Fact]
            public void SavesTodoItemByString()
            {
                TodoItem todoItem;
                using (var context = new TodoContext(_dbContextOptions))
                {
                    var service = new TodoService(context);
                    todoItem = service.Add("item1");
                }
                Assert.Equal("item1", todoItem.Name);
            }

            [Fact]
            public void SavesTodoItemObject()
            {
                TodoItem todoItem;
                using (var context = new TodoContext(_dbContextOptions))
                {
                    var service = new TodoService(context);
                    todoItem = service.Add(new TodoItem() { Name = "item2"});
                }
                Assert.Equal("item2", todoItem.Name);
            }
        }

        public class GetItems
        {
            private readonly TodoService _service;

            public GetItems()
            {
                var dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _service = new TodoService(new TodoContext(dbContextOptions));
            }

            [Fact]
            public void ReturnsListOfItems()
            {
                _service.Add(new TodoItem() {Name = "Task A"});
                _service.Add(new TodoItem() {Name = "Task B"});

                var todoItems = _service.GetItems();
                
                Assert.Equal(3, todoItems.Count);
                Assert.Equal("Item1", todoItems.ElementAt(0).Name);
                Assert.Equal("Task A", todoItems.ElementAt(1).Name);
                Assert.Equal("Task B", todoItems.ElementAt(2).Name);
                throw new Exception("Failing test"); // TODO remove
            }
        }

        public class GetById
        {
            private readonly TodoService _service;

            public GetById()
            {
                var dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _service = new TodoService(new TodoContext(dbContextOptions));
            }

            [Fact]
            public void ReturnsItemIfPresent()
            {
                var todoItem = _service.Add(new TodoItem() {Name = "item0"});

                var result = _service.GetById(todoItem.Id);
                Assert.NotNull(result);
                Assert.Equal("item0", result.Name);
            }

            [Fact]
            public void ReturnsNullifAbsent()
            {
                var result = _service.GetById(1);
                
                Assert.Null(result);
            }
        }

        public class Update
        {
            private readonly TodoService _service;

            public Update()
            {
                var dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _service = new TodoService(new TodoContext(dbContextOptions));
            }

            [Fact]
            public void SetsNewValue()
            {
                var todoItem = _service.Add(new TodoItem() {Name = "item0"});
                todoItem.Name = "item1";

                var result = _service.Update(todoItem);
                
                Assert.Equal(todoItem.Id, result.Id);
                Assert.Equal("item1", result.Name);
            }

            [Fact]
            public void NoNameReturnsNull()
            {
                var todoItem = new TodoItem();
                
                var result = _service.Update(todoItem);
                
                Assert.Null(result);
            }

            [Fact]
            public void NullObjectReturnsNull()
            {
                var result = _service.Update(null);
                
                Assert.Null(result);
            }

            [Fact]
            public void IdNotFoundReturnsNull()
            {
                var todoItem = new TodoItem() { Id = 1000, Name = "name"};
                
                var result = _service.Update(todoItem);
                
                Assert.Null(result);
            }
        }

        public class Delete
        {
            
            private readonly TodoService _service;

            public Delete()
            {
                var dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _service = new TodoService(new TodoContext(dbContextOptions));
            }

            [Fact]
            public void RemovesItem()
            {
                var todoItem = _service.Add(new TodoItem() {Name = "item0"});
                
                _service.Delete(todoItem.Id);

                var result = _service.GetById(todoItem.Id);
                
                Assert.Null(result);
            }
        }
    }
}