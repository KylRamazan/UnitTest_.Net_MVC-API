using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest2.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldUnitTest2.Test
{
  public class ProductsControllerTest
  {
    protected DbContextOptions<DbUnitTestContext> _contextOptions { get; private set; }
    public void SetContextOptions(DbContextOptions<DbUnitTestContext> contextOptions) 
    {
      _contextOptions = contextOptions;
      Seed();
    }

    public void Seed()
    {
      using (DbUnitTestContext context = new DbUnitTestContext(_contextOptions))
      {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Category.Add(new Category { Name = "Kalemler" });
        context.Category.Add(new Category { Name = "Defterler" });
        context.Category.Add(new Category { Name = "Silgiler" });
        context.SaveChanges();

        context.Products.Add(new Product { Name = "Kalem 2", Price = 85, Stock = 78, Color = "Siyah", CategoryId = 1});
        context.Products.Add(new Product { Name = "Silgi", Price = 25, Stock = 150, Color = "Lacivert", CategoryId = 3});
        context.SaveChanges();
      }
    }

  }
}
