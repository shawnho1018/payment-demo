-- Insert data into public.orderinfo
INSERT INTO public.orderinfo (StoreId, OrderNumber, Amount) VALUES
  ('Store123', 'Order001', '100.00'),
  ('Store456', 'Order002', '50.50'),
  ('Store789', 'Order003', '25.75');

-- Insert data into public.merchantinfo
INSERT INTO public.merchantinfo (MSGID, CAVALUE, ORDERINFO_OrderNumber) VALUES
  ('MSG001', 'CAVALUE1', 'Order001'),
  ('MSG002', 'CAVALUE2', 'Order002'),
  ('MSG003', 'CAVALUE3', 'Order003');
