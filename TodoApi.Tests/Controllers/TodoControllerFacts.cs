using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests.Controllers
{
    public class TodoControllerFacts
    {
        public class GetAll
        {
            private TodoController _subject;
            private Mock<TodoContext> _todoContext;
            private Mock<DbSet<TodoItem>> _dbSet;

            public GetAll()
            {
                List<TodoItem> todoItems = new List<TodoItem> {new TodoItem(), new TodoItem()};

                _dbSet = new Mock<DbSet<TodoItem>>();
                _dbSet.Setup(x => x.ToList()).Returns(todoItems);

                _todoContext = new Mock<TodoContext>();
                _todoContext.Setup(ctx => ctx.TodoItems).Returns(_dbSet.Object);
                
//                _subject = new TodoController(_todoContext.Object);
            }
            
//            [Fact]
//            public void returnsListOfTodoItems()
//            {
//                Assert.NotEmpty(_subject.GetAll());
//                Assert.Equal(2, _subject.GetAll().Count());
//                Assert.IsType(typeof(TodoItem), _subject.GetAll().ElementAt(0));
//                Assert.IsType(typeof(TodoItem), _subject.GetAll().ElementAt(1));
//            }
            
        }
    }
}