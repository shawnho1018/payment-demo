using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services; 
namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet(Name = "GetOrder")]
    public IEnumerable<ORDERINFO> GetList()
    {
        return _orderService.GetList();
    }
    [HttpPost(Name = "CreateOrder")]
    public int Create(ORDERINFO order)
    {
        return _orderService.Create(order);
    }
    [HttpPut(Name = "UpdateOrder")]
    public int Update(ORDERINFO order)
    {
        return _orderService.Update(order);
    }
    [HttpGet("{orderNumber}")]
    public ORDERINFO GetByOrderNumber(string orderNumber)
    {
        return _orderService.GetByOrderNumber(orderNumber);
    }
}
