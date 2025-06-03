using Microsoft.AspNetCore.Mvc;
using MotoStore.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderRepository _repository;

    public OrdersController(IConfiguration configuration)
    {
        _repository = new OrderRepository(configuration);
    }

    [HttpGet]
    public IActionResult Get()
    {
        var orders = _repository.GetAllOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var order = _repository.GetOrderById(id);
        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    [HttpPost]
    public IActionResult Post(Order order)
    {
        _repository.AddOrder(order);
        return Ok(new { message = "Order added successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        bool deleted = _repository.DeleteOrder(id);
        if (!deleted)
            return NotFound(new { message = "Order not found" });

        return Ok(new { message = "Order deleted successfully" });
    }

    [HttpPost("with-items")]
    public IActionResult PostOrderWithItems(Order order)
    {
        if (order.Items == null || order.Items.Count == 0)
            return BadRequest(new { message = "Order must contain at least one item" });

        try
        {
            _repository.AddOrderWithItems(order);
            return Ok(new { message = "Order with items added successfully", orderId = order.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating order", error = ex.Message });
        }
    }

    [HttpGet("{id}/with-items")]
    public IActionResult GetOrderWithItems(int id)
    {
        var order = _repository.GetOrderWithItemsById(id);
        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }
}