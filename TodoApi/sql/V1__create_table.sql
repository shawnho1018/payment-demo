  -- Table for ORDERINFO
CREATE TABLE IF NOT EXISTS public.orderinfo ( 
    StoreId VARCHAR(255) NOT NULL,
    OrderNumber VARCHAR(255) NOT NULL,
    Amount VARCHAR(255) NOT NULL,
    PRIMARY KEY (OrderNumber) 
);
  -- Table for MERCHANTJSON
CREATE TABLE IF NOT EXISTS
  public.merchantinfo ( 
    MSGID VARCHAR(255) NOT NULL PRIMARY KEY,
    CAVALUE VARCHAR(255) NOT NULL,
    ORDERINFO_OrderNumber VARCHAR(255) NOT NULL,
    FOREIGN KEY (ORDERINFO_OrderNumber) REFERENCES public.orderinfo(OrderNumber) 
);