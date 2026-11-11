SET XACT_ABORT ON;
BEGIN TRANSACTION;

INSERT INTO dbo.Products (Name, Description, Price, Quantity)
SELECT sample.Name, sample.Description, sample.Price, sample.Quantity
FROM (VALUES
    (N'Coffee', N'Coffee supply for the cafeteria', 2.50, 30),
    (N'Milk', N'One carton of milk', 1.50, 40),
    (N'Juice', N'One bottle of orange juice', 1.25, 35),
    (N'Sandwich', N'One cheese sandwich', 2.00, 25),
    (N'Bottled Water', N'One small bottle of water', 0.50, 60),
    (N'Paper Cups', N'One disposable paper cup', 0.10, 200)
) AS sample(Name, Description, Price, Quantity)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Products AS existing WITH (UPDLOCK, HOLDLOCK)
    WHERE existing.Name = sample.Name
);

COMMIT TRANSACTION;

SELECT Name, Price, Quantity
FROM dbo.Products
WHERE Name IN (N'Coffee', N'Milk', N'Juice', N'Sandwich', N'Bottled Water', N'Paper Cups')
ORDER BY Name;
