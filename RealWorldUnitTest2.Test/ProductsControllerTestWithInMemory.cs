using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest2.Web.Controllers;
using RealWorldUnitTest2.Web.Models;

namespace RealWorldUnitTest2.Test
{
  public class ProductsControllerTestWithInMemory : ProductsControllerTest
  {
    public ProductsControllerTestWithInMemory()
    {
      SetContextOptions(new DbContextOptionsBuilder<DbUnitTestContext>().UseInMemoryDatabase("DbUnitTestInMemory").Options);
    }

    [Fact]
    public async void Create_ProductModelValid_ReturnRedirectToActionWithSaveProduct()
    {
      Product newProduct = new Product
      {
        Name = "Defter",
        Price = 75,
        Stock = 185,
        Color = "Sarı-Lacivert"
      };

      using (var context = new DbUnitTestContext(_contextOptions))
      {
        Category category = context.Category.FirstOrDefault(x => x.Name == "Defterler")!;
        newProduct.CategoryId = category.Id;

        ProductsController productsController = new ProductsController(context);
        IActionResult actionResult = await productsController.Create(newProduct);
        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(actionResult);
        Assert.Equal("Index", redirect.ActionName);
      }

      using (var context = new DbUnitTestContext(_contextOptions))
      {
        Product? product = context.Products.FirstOrDefault(x => x.Name == newProduct.Name);
        Assert.Equal(product.Name, newProduct.Name);
      }

    }

    [Theory]
    [InlineData(2)]
    public async void DeleteCategory_ExistCategoryId_DeletedAllProducts(int categoryId)
    {
      using (var context = new DbUnitTestContext(_contextOptions))
      {
        Category? deletedCategory = await context.Category.FindAsync(categoryId);
        context.Category.Remove(deletedCategory!);
        context.SaveChanges();
      }

      using (var context = new DbUnitTestContext(_contextOptions))
      {
        List<Product> products = await context.Products.Where(x => x.CategoryId == categoryId).ToListAsync();
        Assert.Empty(products);
      }

    }
  }
}
