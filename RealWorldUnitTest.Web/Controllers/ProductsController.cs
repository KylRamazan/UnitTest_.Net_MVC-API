using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWorldUnitTest.Web.Controllers
{
  public class ProductsController : Controller
  {
    private readonly IRepository<Product> _repository;

    public ProductsController(IRepository<Product> repository)
    {
      _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
      return View(await _repository.GetAll());
    }

    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return RedirectToAction("Index");
      }

      Product? product = await _repository.GetById((int)id);
      if (product == null)
      {
        return NotFound();
      }

      return View(product);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color")] Product product)
    {
      if (ModelState.IsValid)
      {
        await _repository.Create(product);
        return RedirectToAction(nameof(Index));
      }
      return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return RedirectToAction("Index");
      }

      Product? product = await _repository.GetById((int)id);
      if (product == null)
      {
        return NotFound();
      }
      return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock,Color")] Product product)
    {
      if (id != product.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        _repository.Update(product);

        return RedirectToAction(nameof(Index));
      }

      return View(product);
    }

    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      Product? product = await _repository.GetById((int)id);
      if (product == null)
      {
        return NotFound();
      }

      return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      Product? product = await _repository.GetById(id);
      if (product != null)
        _repository.Delete(product);
      

      return RedirectToAction(nameof(Index));
    }
  }
}
