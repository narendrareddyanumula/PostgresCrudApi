// Core.UnitTests/Services/ProductServiceTests.cs
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using AutoFixture;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ProductServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly IMapper _mapper;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());

        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDto>().ReverseMap();
        });
        _mapper = mapperConfig.CreateMapper();

        _productService = new ProductService(_productRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = _fixture.CreateMany<Product>(3).ToList();
        _productRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        Assert.Equal(products.Count, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var productDto = _fixture.Create<ProductDto>();

        // Act
        await _productService.AddAsync(productDto);

        // Assert
        _productRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // Arrange
        var productDto = _fixture.Create<ProductDto>();

        // Act
        await _productService.UpdateAsync(productDto);

        // Assert
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct()
    {
        // Arrange
        var productId = _fixture.Create<int>();

        // Act
        await _productService.DeleteAsync(productId);

        // Assert
        _productRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
    }
}
