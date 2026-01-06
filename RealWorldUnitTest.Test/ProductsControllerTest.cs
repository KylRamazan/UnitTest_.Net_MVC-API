using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.ContentModel;
using RealWorldUnitTest.Web.Controllers;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldUnitTest.Test
{
  public class ProductsControllerTest
  {
    private readonly Mock<IRepository<Product>> _mockRepo;
    private readonly ProductsController _productsController;
    private List<Product> _productList;

    public ProductsControllerTest()
    {
      _mockRepo = new Mock<IRepository<Product>>();
      _productsController = new ProductsController(_mockRepo.Object);
      _productList = new List<Product>
      {
        new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 1000, Color = "Lacivert"},
        new Product { Id = 2, Name = "Defter", Price = 150, Stock = 10000, Color = "Sarı"},
        new Product { Id = 3, Name = "Silgi", Price = 50, Stock = 11000, Color = "Mavi"}
      };
    }

    [Fact]
    public async void Index_ActionExecutes_ReturnView()
    {
      IActionResult result = await _productsController.Index();

      Assert.IsType<ViewResult>(result);//Controller içindeki Index methodunun ViewResult donup donmedigi kontrol edildi.
    }

    [Fact]
    public async void Index_ActionExecutes_ReturnProductList()
    {
      _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_productList);//mockRepo Controller'ı simüle ettiği için GetAll methodu çağrıldığında geriye bizim verdiğimiz productListesini dönecek.
      IActionResult actionResult = await _productsController.Index();
      ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);

      IEnumerable<Product> products = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);// ViewResult geriye productList donuyor mu donmuyor mu kontrol edildi.

      Assert.Equal(3, products.Count());//Donen listenin eleman sayısı beklediğimiz gibi 3 mü kontrol edildi.
    }

    [Fact]
    public async void Details_IdIsNull_ReturnRedirectToIndexAction()
    {
      IActionResult actionResult = await _productsController.Details(null);
      RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(actionResult);//Details methodu RedirectToActionResult donuyor mu kontrol edildi.
      Assert.Equal("Index", redirect.ActionName);//Redirect işlemi Index sayfasına yapılıyor mu kontrol edildi.
    }

    [Fact]
    public async void Details_IdInvalid_ReturnNotFound()
    {
      Product product = null;
      _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);
      IActionResult actionResult = await _productsController.Details(0);
      NotFoundResult redirect = Assert.IsType<NotFoundResult>(actionResult);//Details methodu NotFoundResult donuyor mu kontrol edildi.
      Assert.Equal(404, redirect.StatusCode);//Redirect işlemi 404 statusCodu donuyor mu kontrol edildi.
    }

    [Theory]
    [InlineData(1)]
    public async void Details_IdValid_ReturnProduct(int id)
    {
      Product? product = _productList.FirstOrDefault(x => x.Id == id);
      _mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
      IActionResult actionResult = await _productsController.Details(id);
      ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
      Product? resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);//ViewResult ile product modeli donuyor mu kontrol edildi.

      Assert.Equal(product, resultProduct);//resulttan gelen model ile benim listemdeki model uyuyor mu kontrol edildi.
    }

    [Fact]
    public void Create_ActionExecutes_ReturnView()
    {
      IActionResult result = _productsController.Create();
      Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async void CreatePost_InvalidModelState_ReturnView()
    {
      _productsController.ModelState.AddModelError("Name", "Name alanı zorunludur!");//Product modeli üzerinde hata eklendi.

      IActionResult actionResult = await _productsController.Create(_productList.FirstOrDefault()!);
      ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
      Assert.IsType<Product>(viewResult.Model);
    }

    [Fact]
    public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
    {
      IActionResult actionResult = await _productsController.Create(_productList.FirstOrDefault()!);
      RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);
      Assert.Equal("Index", result.ActionName);
    }

    [Fact]
    public async void CreatePost_ValidModelState_CreateMethodExecute()
    {
      Product newProduct = null;
      _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);//Create methodu içinde her hangi bir product gelebilir şeklinde ayarlandı ve Callbak ile gelen product nesnesi newProduct nesnesine atandı.
      IActionResult result = await _productsController.Create(_productList.FirstOrDefault()!);//Controller create çalıştırıldı.
      _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);//Create methodu en az 1 kez çalıştıysa test başarılı olacak.
      Assert.Equal(_productList.FirstOrDefault()!.Id, newProduct!.Id);
    }

    [Fact]
    public async void CreatePost_InValidModelState_NeverCreateMethodExecute()
    {
      _productsController.ModelState.AddModelError("Name", "Name alanı zorunludur!");

      IActionResult result = await _productsController.Create(_productList.FirstOrDefault()!);
      _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);//Create methodu hiç çalışmadıysa test başarılı olur.
    }

    [Fact]
    public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
    {
      IActionResult actionResult = await _productsController.Edit(null);
      RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);
      Assert.Equal("Index", result.ActionName);
    }

    [Theory]
    [InlineData(5)]
    public async void Edit_IdInvalid_ReturnNotFound(int id)
    {
      Product product = null;
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

      IActionResult actionResult = await _productsController.Edit(id);
      NotFoundResult result = Assert.IsType<NotFoundResult>(actionResult);
      Assert.Equal(404, result.StatusCode);
    }

    [Theory]
    [InlineData(3)]
    public async void Edit_IdValid_ReturnProduct(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

      IActionResult actionResult = await _productsController.Edit(id);
      ViewResult result = Assert.IsType<ViewResult>(actionResult);
      Product? resultProduct = Assert.IsAssignableFrom<Product>(result.Model);
      Assert.Equal(product, resultProduct);
    }

    [Theory]
    [InlineData(3)]
    public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;

      IActionResult actionResult = _productsController.Edit(2, product);
      NotFoundResult result = Assert.IsType<NotFoundResult>(actionResult);
      Assert.Equal(404, result.StatusCode);
    }

    [Theory]
    [InlineData(3)]
    public void EditPost_InvalidModelState_ReturnView(int id)
    {
      _productsController.ModelState.AddModelError("Name", "Name alanı zorunludur!");
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;

      IActionResult actionResult = _productsController.Edit(id, product);
      ViewResult result = Assert.IsType<ViewResult>(actionResult);
      Product? resultProduct = Assert.IsAssignableFrom<Product>(result.Model);

      Assert.Equal(product, resultProduct);
    }

    [Theory]
    [InlineData(3)]
    public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      IActionResult actionResult = _productsController.Edit(id, product);
      RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);
      Assert.Equal("Index", result.ActionName);
    }

    [Theory]
    [InlineData(3)]
    public void EditPost_ValidModelState_UpdateMethodExecute(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      _mockRepo.Setup(repo => repo.Update(product));

      IActionResult actionResult = _productsController.Edit(id, product);
      _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
      RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);
      Assert.Equal("Index", result.ActionName);
    }

    [Fact]
    public async void Delete_IdIsNull_ReturnNotFound()
    {
      IActionResult actionResult = await _productsController.Delete(null);
      NotFoundResult redirect = Assert.IsType<NotFoundResult>(actionResult);
      Assert.Equal(404, redirect.StatusCode);
    }

    [Theory]
    [InlineData(10)]
    public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int id)
    {
      Product product = null;
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);
      IActionResult actionResult = await _productsController.Delete(id);
      NotFoundResult redirect = Assert.IsType<NotFoundResult>(actionResult);
      Assert.Equal(404, redirect.StatusCode);
    }

    [Theory]
    [InlineData(2)]
    public async void Delete_ActionExecutes_ReturnProduct(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);
      IActionResult actionResult = await _productsController.Delete(id);
      ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
      Product? resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
      Assert.Equal(product, resultProduct);
    }

    [Theory]
    [InlineData(2)]
    public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int id) 
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      IActionResult actionResult = await _productsController.DeleteConfirmed(id);

      RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);
      Assert.Equal("Index", result.ActionName);
    }

    [Theory]
    [InlineData(2)]
    public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int id)
    {
      Product product = _productList.FirstOrDefault(x => x.Id == id)!;
      _mockRepo.Setup(repo => repo.Delete(product));
      _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

      IActionResult actionResult = await _productsController.DeleteConfirmed(id);
      _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
      RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);
      Assert.Equal("Index", result.ActionName);
    }
  }
}
