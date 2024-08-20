using Microsoft.AspNetCore.Mvc; // 引入 ASP.NET Core MVC 框架相關的命名空間
using System.Net; // 引入網路相關的命名空間
using TodoApi.Models; // 引入自訂的模型類別
using TodoApi.Services; // 引入自訂的服務類別
using System.Diagnostics.Metrics;
namespace TodoApi.Controllers; // 定義命名空間

[ApiController] // 標記此類別為控制器，用於處理 Web API 請求
[Route("[controller]")] // 定義控制器路由，這裡使用控制器名稱作為路由前綴
public class OrderController : ControllerBase // 定義 OrderController 類別，繼承自 ControllerBase
{
    private static readonly Meter meter = new Meter("TodoApi.Controllers.OrderController");
    private static readonly Counter<long> getOrderCounter = meter.CreateCounter<long>("GetOrder");
    private static readonly Counter<long> createOrderCounter = meter.CreateCounter<long>("CreateOrder");
    private static readonly Counter<long> updateOrderCounter = meter.CreateCounter<long>("UpdateOrder");
    private static readonly Counter<long> getByOrderNumberCounter = meter.CreateCounter<long>("GetByOrderNumber");

    private static readonly Counter<long> deleteByOrderNumberCounter = meter.CreateCounter<long>("DeleteByOrderNumber");
    private readonly OrderService _orderService; // 聲明一個 OrderService 類型的私有唯讀欄位，用於處理訂單相關的邏輯
    private ILogger<OrderController> logger; // 聲明一個 ILogger 類型的私有欄位，用於記錄日誌

    // 建構函式，用於注入 OrderService 和 ILogger
    public OrderController(OrderService orderService, ILogger<OrderController> logger)
    {
        this._orderService = orderService; // 初始化 OrderService 欄位
        this.logger = logger; // 初始化 ILogger 欄位
    }

    // GET: /order
    // 獲取所有訂單列表
    [HttpGet(Name = "GetOrder")] // 標記此方法處理 HTTP GET 請求，路由名稱為 "GetOrder"
    public IEnumerable<ORDERINFO> GetList() // 定義 GetList 方法，返回一個 ORDERINFO 類型的可列舉物件
    {
        getOrderCounter.Add(1);
        return _orderService.GetList(); // 呼叫 OrderService 的 GetList 方法獲取所有訂單列表
    }

    // POST: /order
    // 建立新訂單
    [HttpPost(Name = "CreateOrder")] // 標記此方法處理 HTTP POST 請求，路由名稱為 "CreateOrder"
    public int Create(ORDERINFO order) // 定義 Create 方法，接收一個 ORDERINFO 類型的參數，返回一個整數
    {
        createOrderCounter.Add(1);
        if (order == null){ // 檢查訂單參數是否為空
            logger.LogError("Missing order parameter"); // 如果為空，記錄錯誤日誌
            throw new HttpRequestException("Missing order parameter", null, HttpStatusCode.BadRequest); // 拋出異常，提示缺少訂單參數
        }
        return _orderService.Create(order); // 呼叫 OrderService 的 Create 方法建立新訂單，並返回訂單 ID
    }

    // PUT: /order
    // 更新現有訂單
    [HttpPut(Name = "UpdateOrder")] // 標記此方法處理 HTTP PUT 請求，路由名稱為 "UpdateOrder"
    public int Update(ORDERINFO order) // 定義 Update 方法，接收一個 ORDERINFO 類型的參數，返回一個整數
    {
        updateOrderCounter.Add(1);
        if (order == null){ // 檢查訂單參數是否為空
            logger.LogError("Missing order parameter"); // 如果為空，記錄錯誤日誌
            throw new HttpRequestException("Missing order parameter", null, HttpStatusCode.BadRequest); // 拋出異常，提示缺少訂單參數
        }
        return _orderService.Update(order); // 呼叫 OrderService 的 Update 方法更新現有訂單，並返回更新的結果
    }

    // GET: /order/{orderNumber}
    // 根據訂單編號獲取特定訂單
    [HttpGet("{orderNumber}")] // 標記此方法處理 HTTP GET 請求，路由包含一個名為 "orderNumber" 的參數
    public ORDERINFO GetByOrderNumber(string orderNumber) // 定義 GetByOrderNumber 方法，接收一個字串類型的訂單編號參數，返回一個 ORDERINFO 類型的物件
    {
        getByOrderNumberCounter.Add(1);
        if (orderNumber == null) { // 檢查訂單編號參數是否為空
            logger.LogError("Missing order Number"); // 如果為空，記錄錯誤日誌
            throw new HttpRequestException("Missing order number", null, HttpStatusCode.BadRequest); // 拋出異常，提示缺少訂單編號
        }
        return _orderService.GetByOrderNumber(orderNumber); // 呼叫 OrderService 的 GetByOrderNumber 方法根據訂單編號獲取特定訂單
    }

    [HttpDelete("{orderNumber}")] 
    public int DeleteByOrderNumber(string orderNumber){
        deleteByOrderNumberCounter.Add(1);
        if (orderNumber == null) {
            logger.LogError("Missing order Number");
            throw new HttpRequestException("Missing order number", null, HttpStatusCode.BadRequest);
        }
        return _orderService.DeleteByOrderNumber(orderNumber);
    }
}
