namespace TodoApi.Models;
public class ORDERINFO
{
    public string StoreId { get; set; }
    public string OrderNumber { get; set; }
    public string Amount { get; set; }
}

public class MERCHANTJSON
{
    public string MSGID { get; set; }
    public string CAVALUE { get; set; }
    public ORDERINFO ORDERINFO { get; set; }
}