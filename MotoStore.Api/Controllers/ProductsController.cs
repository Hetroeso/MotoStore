using Microsoft.AspNetCore.Mvc;
using MotoStore.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductRepository _repository;
    private readonly AuthContext _auth;

    public ProductsController(IConfiguration configuration, AuthContext auth)
    {
        _repository = new ProductRepository(configuration);
        _auth = auth;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var products = _repository.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _repository.GetProductById(id);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpPost]
    public IActionResult AddProduct(Product product)
    {
        if (!_auth.IsAuthenticated || !_auth.IsAdmin)
            return Forbid("Only admins can add products.");

        _repository.AddProduct(product);
        return Ok(new { message = "Product added successfully" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Product product)
    {
        if (!_auth.IsAuthenticated || !_auth.IsAdmin)
            return Forbid("Only admins can add products.");

        if (id != product.Id)
            return BadRequest(new { message = "ID mismatch" });

        bool success = _repository.UpdateProduct(product);
        if (!success)
            return NotFound(new { message = "Product not found" });

        return Ok(new { message = "Product updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_auth.IsAuthenticated || !_auth.IsAdmin)
            return Forbid("Only admins can add products.");

        bool success = _repository.DeleteProduct(id);
        if (!success)
            return NotFound(new { message = "Product not found" });

        return Ok(new { message = "Product deleted successfully" });
    }
}