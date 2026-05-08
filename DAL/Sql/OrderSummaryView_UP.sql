CREATE VIEW View_OrderSummary
AS
    SELECT o.Id, o.[OrderDate], o.TotalValue
    FROM [Orders] AS o
    GROUP BY o.Id, o.[OrderDate], o.TotalValue