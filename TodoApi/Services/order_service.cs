using System.Collections; // 引入集合相關的命名空間，例如 IEnumerable
using Dapper; // 引入 Dapper 套件，用於簡化資料庫操作
using Npgsql; // 引入 Npgsql 套件，用於連接 PostgreSQL 資料庫
using TodoApi.Models; // 引入自訂的模型類別，例如 ORDERINFO
using OpenTelemetry; // 引入 OpenTelemetry 相關的命名空間
using OpenTelemetry.Trace; // 引入 OpenTelemetry 追蹤相關的命名空間

namespace TodoApi.Services; // 定義命名空間

public class OrderService // 定義 OrderService 類別
{
    private readonly string _connectionString; // 聲明一個私有唯讀字串欄位，用於儲存資料庫連接字串
    private readonly Tracer _tracer; // 聲明一個私有唯讀 Tracer 欄位，用於追蹤

    // 建構函式，用於注入 IConfiguration 和 TracerProvider
    public OrderService(IConfiguration configuration, TracerProvider tracerProvider)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection"); // 從配置中取得資料庫連接字串
        _tracer = tracerProvider.GetTracer("NPGSQL"); // 取得 Npgsql 的 Tracer 實例
    }

    // 取得所有訂單列表
    public IEnumerable<ORDERINFO> GetList()
    {
        using var span = _tracer.StartActiveSpan("OrderService.GetList"); // 開始一個新的追蹤 Span，名稱為 "OrderService.GetList"
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString)) // 建立一個新的 NpgsqlConnection 物件，並傳入資料庫連接字串
            {
                var sql = "SELECT * FROM public.orderinfo"; // 定義 SQL 查詢語法
                return connection.Query<ORDERINFO>(sql); // 使用 Dapper 執行 SQL 查詢，並將結果映射到 ORDERINFO 物件列表
            }
        }
        catch (Exception ex)
        {
            span.RecordException(ex); // 如果發生異常，記錄異常資訊到追蹤 Span
            throw; // 拋出異常
        }
    }

    // 建立新訂單
    public int Create(ORDERINFO order)
    {
        using var span = _tracer.StartActiveSpan("OrderService.Create"); // 開始一個新的追蹤 Span，名稱為 "OrderService.Create"
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString)) // 建立一個新的 NpgsqlConnection 物件，並傳入資料庫連接字串
            {
                var sql = @"INSERT INTO public.orderinfo (StoreId, OrderNumber, Amount) 
                            VALUES (@StoreId, @OrderNumber, @Amount)"; // 定義 SQL 新增語法
                return connection.Execute(sql, new { order.StoreId, order.OrderNumber, order.Amount }); // 使用 Dapper 執行 SQL 新增，並傳入參數
            }
        }
        catch (Exception ex)
        {
            span.RecordException(ex); // 如果發生異常，記錄異常資訊到追蹤 Span
            throw; // 拋出異常
        }
    }

    // 更新現有訂單
    public int Update(ORDERINFO order)
    {
        using var span = _tracer.StartActiveSpan("OrderService.Update"); // 開始一個新的追蹤 Span，名稱為 "OrderService.Update"
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString)) // 建立一個新的 NpgsqlConnection 物件，並傳入資料庫連接字串
            {
                var sql = @"UPDATE public.orderinfo 
                            SET StoreId = @StoreId, Amount = @Amount 
                            WHERE OrderNumber = @OrderNumber"; // 定義 SQL 更新語法
                return connection.Execute(sql, new { order.StoreId, order.OrderNumber, order.Amount }); // 使用 Dapper 執行 SQL 更新，並傳入參數
            }
        }
        catch (Exception ex)
        {
            span.RecordException(ex); // 如果發生異常，記錄異常資訊到追蹤 Span
            throw; // 拋出異常
        }
    }

    // 根據訂單編號取得特定訂單
    public ORDERINFO GetByOrderNumber(string orderNumber)
    {
        using var span = _tracer.StartActiveSpan("OrderService.GetByOrderNumber"); // 開始一個新的追蹤 Span，名稱為 "OrderService.GetByOrderNumber"
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString)) // 建立一個新的 NpgsqlConnection 物件，並傳入資料庫連接字串
            {
                var sql = "SELECT * FROM public.orderinfo WHERE OrderNumber = @orderNumber"; // 定義 SQL 查詢語法
                return connection.QueryFirstOrDefault<ORDERINFO>(sql, new { orderNumber }); // 使用 Dapper 執行 SQL 查詢，並將結果映射到 ORDERINFO 物件
            }
        }
        catch (Exception ex)
        {
            span.RecordException(ex); // 如果發生異常，記錄異常資訊到追蹤 Span
            throw; // 拋出異常
        }
    }
    // 根據訂單編號刪除特定訂單
    public int DeleteByOrderNumber(string orderNumber)
    {
        using var span = _tracer.StartActiveSpan("OrderService.DeleteByOrderNumber"); // 開始一個新的追蹤 Span，名稱為 "OrderService.DeleteByOrderNumber"
        try {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var sql = "DELETE FROM public.orderinfo WHERE OrderNumber = @orderNumber";
                return connection.Execute(sql, new {orderNumber});
            }
        }
        catch (Exception ex)
        {
            span.RecordException(ex);
            throw;
        }
    }
}