
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'quan_ly_tai_lieu')
BEGIN
    CREATE DATABASE quan_ly_tai_lieu;
    PRINT N'Database quan_ly_tai_lieu đã được tạo thành công!'
END
GO

USE quan_ly_tai_lieu;
GO

IF OBJECT_ID('tai_lieu', 'U') IS NOT NULL
BEGIN
    DROP TABLE tai_lieu;
    PRINT N'Đã xóa bảng tai_lieu cũ!'
END
GO

CREATE TABLE tai_lieu (
    id INT PRIMARY KEY IDENTITY(1,1),
    ten NVARCHAR(200) NOT NULL,
    mon_hoc NVARCHAR(100),
    loai NVARCHAR(100),
    duong_dan NVARCHAR(500) NOT NULL,
    ghi_chu NVARCHAR(1000),
    ngay_them DATETIME DEFAULT GETDATE(),
    kich_thuoc FLOAT,
    tac_gia NVARCHAR(100),
    quan_trong BIT DEFAULT 0
);
GO
