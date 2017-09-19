using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TodoApi.Tests.Models;
using Xunit;

namespace TodoApi.SmokeTests
{
    public class SmokeTests
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SmokeTests()
        {
            var configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables()
                            .Build();

            _baseUrl = configuration["Endpoint"];
            _httpClient = new HttpClient();
        }

        [Fact]
        public async void GetReturnsListOfItems()
        {
            var httpResponseMessage = await _httpClient.GetAsync(_baseUrl + "/api/todo");
            httpResponseMessage.EnsureSuccessStatusCode();
            
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            
            var items = JsonConvert.DeserializeObject<TodoItem[]>(responseString);
            Assert.Equal(1, items[0].Id);
            Assert.Equal("Item1", items[0].Name);
            Assert.False(items[0].IsComplete);
        }

        [Fact]
        public async void GetItemReturnsOneItem()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/api/todo/1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            var item = JsonConvert.DeserializeObject<TodoItem>(responseString);
            Assert.Equal(1, item.Id);
            Assert.Equal("Item1", item.Name);
            Assert.False(item.IsComplete);
        }
    }
}
