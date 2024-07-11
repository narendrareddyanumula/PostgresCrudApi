// WebApi.IntegrationTests/ProductsControllerTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApi;
using Xunit;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Startup> _factory;

    public ProductsControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk()
    {
        // Arrange
        var response = await _client.GetAsync("/api/products");

        // Act
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var postResponse = await _client.PostAsync("/api/products", 
            new StringContent(JsonConvert.SerializeObject(new { Name = "TestProduct", Price = 10.0m }), Encoding.UTF8, "application/json"));
        postResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync(postResponse.Headers.Location);

        // Act
        response.EnsureSuccessStatusCode();

        // Assert
        var product = JsonConvert.DeserializeObject<ProductDto>(await response.Content.ReadAsStringAsync());
        Assert.Equal("TestProduct", product.Name);
    }

    [Fact]
    public async Task PostProduct_ShouldCreateProduct()
    {
        // Arrange
        var product = new { Name = "TestProduct", Price = 10.0m };

        // Act
        var response = await _client.PostAsync("/api/products", 
            new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        // Assert
        var createdProduct = JsonConvert.DeserializeObject<ProductDto>(await response.Content.ReadAsStringAsync());
        Assert.Equal("TestProduct", createdProduct.Name);
    }

    [Fact]
    public async Task PutProduct_ShouldUpdateProduct()
    {
        // Arrange
        var postResponse = await _client.PostAsync("/api/products", 
            new StringContent(JsonConvert.SerializeObject(new { Name = "TestProduct", Price = 10.0m }), Encoding.UTF8, "application/json"));
        postResponse.EnsureSuccessStatusCode();

        var createdProduct = JsonConvert.DeserializeObject<ProductDto>(await postResponse.Content.ReadAsStringAsync());

        var updatedProduct = new { Id = createdProduct.Id, Name = "UpdatedProduct", Price = 15.0m };

        // Act
        var putResponse = await _client.PutAsync($"/api/products/{createdProduct.Id}", 
            new StringContent(JsonConvert.SerializeObject(updatedProduct), Encoding.UTF8, "application/json"));
        putResponse.EnsureSuccessStatusCode();

        // Assert
        var product = JsonConvert.DeserializeObject<ProductDto>(await putResponse.Content.ReadAsStringAsync());
        Assert.Equal("UpdatedProduct", product.Name);
    }

    [Fact]
    public async Task DeleteProduct_ShouldDeleteProduct()
    {
        // Arrange
        var postResponse = await _client.PostAsync("/api/products", 
            new StringContent(JsonConvert.SerializeObject(new { Name = "TestProduct", Price = 10.0m }), Encoding.UTF8, "application/json"));
        postResponse.EnsureSuccessStatusCode();

        var createdProduct = JsonConvert.DeserializeObject<ProductDto>(await postResponse.Content.ReadAsStringAsync());

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        // Assert
        var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
