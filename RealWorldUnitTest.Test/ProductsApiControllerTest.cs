using Microsoft.AspNetCore.Mvc;
using Moq;
using RealWorldUnitTest.Web.Controllers;
using RealWorldUnitTest.Web.Helpers;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;

namespace RealWorldUnitTest.Test
{
  public class ProductsApiControllerTest
  {
    private readonly Mock<IRepository<Product>> _mockRepo;
    private readonly ProductsApiController _productsApiController;
    private readonly Helper _helper;
    private List<Product> _productList;

    public ProductsApiControllerTest()
    {
      _mockRepo = new Mock<IRepository<Product>>();
      _productsApiController = new ProductsApiController(_mockRepo.Object);
      _helper = new Helper();
      _productList = new List<Product>
      {
        new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 1000, Color = "Lacivert"},
        new Product { Id = 2, Name = "Defter", Price = 150, Stock = 10000, Color = "Sarı"},
        new Product { Id = 3, Name = "Silgi", Price = 50, Stock = 11000, Color = "Mavi"}
      };
    }

    [Theory]
    [InlineData(4, 5, 9)]
    [InlineData(2, 8, 10)]
    public void Add_SampleValues_ReturnTotal(int x, int y, int total) 
    {
      int result = _helper.Add(x, y);
      Assert.Equal(total, result);
    }

    [Fact]
    public async void GetProducts_ActionExecutes_ReturnOkResultWithProduct() 
    {
      _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_productList);
      IActionResult result = await _productsApiController.GetProducts();
      OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);//OkObjectResult Ok(projects) yani geriye object döndüğü için kullanılır. Sadece Ok() olsa yani object dönmezse OkResult kullanılır.
      IEnumerable<Product> returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
      Assert.Equal(3, returnProducts.Count());
    }

    [Theory]
    [InlineData(10)]
    public async void GetProduct_IdInvalid_ReturnNotFound(int id)
    {
      Product? product = null;
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

      IActionResult result = await _productsApiController.GetProduct(id);
      Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void GetProduct_IdValid_ReturnOkResultWithProduct(int id)
    {
      Product? product = _productList.FirstOrDefault(x => x.Id == id);
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

      IActionResult result = await _productsApiController.GetProduct(id);
      OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
      Product returnProducts = Assert.IsAssignableFrom<Product>(okResult.Value);
      Assert.Equal(product, returnProducts);
    }

    [Theory]
    [InlineData(10)]
    public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int id) 
    {
      Product product = _productList.FirstOrDefault()!;
      IActionResult result = _productsApiController.PutProduct(id, product);
      Assert.IsType<BadRequestResult>(result);
    }

    [Theory]
    [InlineData(2)]
    public void PutProduct_ActionExecutes_ReturnNoContent(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      _mockRepo.Setup(repo => repo.Update(product));

      IActionResult result = _productsApiController.PutProduct(id, product);
      _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async void PostProduct_ActionExecutes_ReturnCreatedAtAction() 
    {
      Product product = _productList.FirstOrDefault()!;
      _mockRepo.Setup(repo => repo.Create(product)).Returns(Task.CompletedTask);

      IActionResult actionResult = await _productsApiController.PostProduct(product);
      _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
      CreatedAtActionResult createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult);
      Assert.Equal("GetProduct", createdAtActionResult.ActionName);

      Assert.NotNull(createdAtActionResult.RouteValues);
      Assert.True(createdAtActionResult.RouteValues.ContainsKey("id"));
      Assert.Equal(product.Id, createdAtActionResult.RouteValues["id"]);

      Product returnedProduct = Assert.IsType<Product>(createdAtActionResult.Value);
      Assert.Equal(product, returnedProduct);
    }

    [Theory]
    [InlineData(10)]
    public async void DeleteProduct_IdInvalid_ReturnNotFound(int id)
    {
      Product product = null;
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);
      IActionResult actionResult = await _productsApiController.DeleteProduct(id);
      Assert.IsType<NotFoundResult>(actionResult);
    }

    [Theory]
    [InlineData(1)]
    public async void DeleteProduct_ActionExecutes_ReturnNoContent(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      _mockRepo.Setup(repo => repo.Delete(product));
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

      IActionResult actionResult = await _productsApiController.DeleteProduct(id);
      _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
      Assert.IsType<NoContentResult>(actionResult);
    }

  }
}
