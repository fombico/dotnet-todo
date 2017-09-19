using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService
    {
        private readonly TodoContext _todoContext;

        public TodoService(TodoContext todoContext)
        {
            _todoContext = todoContext;
            
            todoContext.Database.EnsureCreated();
            
            if (!_todoContext.TodoItems.Any())
            {
                _todoContext.TodoItems.Add(new TodoItem() { Name = "Item1"});
                _todoContext.SaveChanges();
            }
        }

        public TodoItem Add(string name)
        {
            return Add(new TodoItem() {Name = name});
        }

        public TodoItem Add(TodoItem todoItem)
        {
            EntityEntry<TodoItem> entityEntry = _todoContext.Add(todoItem);
            _todoContext.SaveChanges();
            return entityEntry.Entity;
        }

        public List<TodoItem> GetItems()
        {
            return _todoContext.TodoItems.ToList();
        }

        public TodoItem GetById(long id)
        {
            return _todoContext.TodoItems.FirstOrDefault(t => t.Id == id);
        }

        public TodoItem Update(TodoItem todoItem)
        {
            if (todoItem?.Name == null)
            {
                return null;
            }
            var todo = _todoContext.TodoItems.FirstOrDefault(t => t.Id == todoItem.Id);
            if (todo == null)
            {
                return null;
            }
            
            var entityEntry = _todoContext.TodoItems.Update(todoItem);
            _todoContext.SaveChanges();
            return entityEntry.Entity;
        }

        public void Delete(long id)
        {
            var todo = _todoContext.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null) return;
            _todoContext.TodoItems.Remove(todo);
            _todoContext.SaveChanges();
        }
    }
}