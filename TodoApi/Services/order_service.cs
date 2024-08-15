using System.Collections;
using Dapper;
using Npgsql;
using TodoApi.Models;

namespace TodoApi.Services;
public class OrderService {
    private readonly string _connectionString;
    public OrderService(IConfiguration configuration) {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    public IEnumerable<ORDERINFO> GetList() {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "SELECT * FROM public.orderinfo";
            return connection.Query<ORDERINFO>(sql);
        }        
    }
    public int Create(ORDERINFO order) {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO public.orderinfo (StoreId, OrderNumber, Amount) 
                        VALUES (@StoreId, @OrderNumber, @Amount)";
            return connection.Execute(sql, new {order.StoreId, order.OrderNumber, order.Amount});
        }   
    }
    public int Update(ORDERINFO order) {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = @"UPDATE public.orderinfo 
                        SET StoreId = @StoreId, Amount = @Amount 
                        WHERE OrderNumber = @OrderNumber";
            return connection.Execute(sql, new { order.StoreId, order.OrderNumber, order.Amount });
        }
    }
    public ORDERINFO GetByOrderNumber(string orderNumber)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "SELECT * FROM public.orderinfo WHERE OrderNumber = @orderNumber";
            return connection.QueryFirstOrDefault<ORDERINFO>(sql, new { orderNumber });
        }
    }
}