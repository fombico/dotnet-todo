using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests.Controllers
{
    public class TodoControllerFacts
    {
        private TestServer _server;
        private HttpClient _client;

        public TodoControllerFacts()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot("/Users/pivotal/RiderProjects/TodoApi/TodoApi")
                .UseEnvironment("Integration Test")
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetAllReturnsList()
        {
            var response = await _client.GetAsync("/api/todo");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            var items = JsonConvert.DeserializeObject<TodoItem[]>(responseString);
            Assert.Equal(1, items.Length);
            Assert.Equal(1, items[0].Id);
            Assert.Equal("Item1", items[0].Name);
            Assert.False(items[0].IsComplete);
        }

        [Fact]
        public async Task GetByIdReturnsTodoItem()
        {
            var response = await _client.GetAsync("/api/todo/1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            var item = JsonConvert.DeserializeObject<TodoItem>(responseString);
            Assert.Equal(1, item.Id);
            Assert.Equal("Item1", item.Name);
            Assert.False(item.IsComplete);
        }

        [Fact]
        public async Task CreateAddsTodoItem()
        {
            var requestBody = JsonConvert.SerializeObject(new TodoItem() {Name = "Walk dogs"});
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/todo", httpContent);
            response.EnsureSuccessStatusCode();
            
            var url = response.Headers.Location;
            var todoItemResponse = await _client.GetAsync(url);
            var responseString = await todoItemResponse.Content.ReadAsStringAsync();
            
            var item = JsonConvert.DeserializeObject<TodoItem>(responseString);
            
            Assert.Equal("Walk dogs", item.Name);
            Assert.False(item.IsComplete);
        }

        [Fact]
        public async Task UpdateModifiesTodoItem()
        {
            var requestBody =
                JsonConvert.SerializeObject(new TodoItem() {Id = 1, IsComplete = true, Name = "Walk dogs"});
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            await _client.PutAsync("/api/todo/1", httpContent);
            
            var response = await _client.GetAsync("/api/todo/1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            var item = JsonConvert.DeserializeObject<TodoItem>(responseString);
            Assert.Equal(1, item.Id);
            Assert.Equal("Walk dogs", item.Name);
            Assert.True(item.IsComplete);
        }

        [Fact]
        public async Task DeleteRemovesItem()
        {
            await _client.DeleteAsync("/api/todo/1");
            
            var response = await _client.GetAsync("/api/todo/1");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}