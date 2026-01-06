using Microsoft.AspNetCore.Mvc;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;

namespace RealWorldUnitTest.Web.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsApiController : ControllerBase
  {
    private readonly IRepository<Product> _repository;

    public ProductsApiController(IRepository<Product> repository)
    {
      _repository = repository;
    }

    [HttpGet("{a}/{b}")]
    public IActionResult Add(int a, int b)
    {
      int total = new Helpers.Helper().Add(a, b);
      return Ok(total);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
      IEnumerable<Product> products = await _repository.GetAll();
      return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
      Product? product = await _repository.GetById(id);

      if (product == null)
        return NotFound();

      return Ok(product);
    }

    [HttpPut("{id}")]
    public IActionResult PutProduct(int id, Product product)
    {
      if (id != product.Id)
        return BadRequest();
      
      _repository.Update(product);

      return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> PostProduct(Product product)
    {
      await _repository.Create(product);

      return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
      Product? product = await _repository.GetById(id);
      if (product == null)
        return NotFound();
      
      _repository.Delete(product);
      return NoContent();
    }
  }
}
