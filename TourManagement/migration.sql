USE [TourManagement]
GO

-- 1. Tạo 3 bảng mới
CREATE TABLE [dbo].[BookingStatuses](
	[status_id] [varchar](10) NOT NULL PRIMARY KEY,
	[status_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](200) NULL
)
GO

CREATE TABLE [dbo].[PaymentStatuses](
	[status_id] [varchar](10) NOT NULL PRIMARY KEY,
	[status_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](200) NULL
)
GO

CREATE TABLE [dbo].[TourStatuses](
	[status_id] [varchar](10) NOT NULL PRIMARY KEY,
	[status_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](200) NULL
)
GO

-- 2. Đổ dữ liệu tương ứng vào các bảng (bạn có thể tự tinh chỉnh các status này sau)
INSERT INTO [dbo].[BookingStatuses] (status_id, status_name) VALUES 
('PENDING', N'Chờ xác nhận'),
('CONFIRMED', N'Đã xác nhận'),
('COMPLETED', N'Hoàn thành'),
('CANCELLED', N'Hủy');

INSERT INTO [dbo].[PaymentStatuses] (status_id, status_name) VALUES 
('PENDING', N'Chờ thanh toán'),
('PAID', N'Đã thanh toán'),
('FAILED', N'Thất bại'),
('REFUNDED', N'Đã hoàn tiền');

INSERT INTO [dbo].[TourStatuses] (status_id, status_name) VALUES 
('OPEN', N'Mở đặt chỗ'),
('FULL', N'Đầy chỗ'),
('DEPARTED', N'Đã khởi hành'),
('COMPLETED', N'Hoàn thành');
GO

-- CẬP NHẬT DỮ LIỆU ĐỂ TRÁN LỖI RÀNG BUỘC (Constraints)
-- Nếu có Booking nào đang ở trạng thái 'PAID' (sai lệch logic cũ), ta tạm ép về 'CONFIRMED'
UPDATE [dbo].[Bookings] SET status_id = 'CONFIRMED' WHERE status_id = 'PAID'
-- Nếu có Payment nào đang ở trạng thái khác 'PENDING', 'PAID', 'FAILED', 'REFUNDED', ép về 'PAID'
UPDATE [dbo].[Payments] SET status_id = 'PAID' WHERE status_id NOT IN ('PENDING', 'PAID', 'FAILED', 'REFUNDED')

-- 3. XÓA các Khóa ngoại (Foreign Keys) cũ
ALTER TABLE [dbo].[Bookings] DROP CONSTRAINT [FK_Bookings_Statuses]
GO
ALTER TABLE [dbo].[Payments] DROP CONSTRAINT [FK_Payments_Statuses]
GO
ALTER TABLE [dbo].[TourGroups] DROP CONSTRAINT [FK_TourGroups_Statuses]
GO

-- 4. THÊM Khóa ngoại mới trỏ về 3 bảng riêng
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_BookingStatuses] FOREIGN KEY([status_id])
REFERENCES [dbo].[BookingStatuses] ([status_id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_BookingStatuses]
GO

ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_PaymentStatuses] FOREIGN KEY([status_id])
REFERENCES [dbo].[PaymentStatuses] ([status_id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_PaymentStatuses]
GO

ALTER TABLE [dbo].[TourGroups]  WITH CHECK ADD  CONSTRAINT [FK_TourGroups_TourStatuses] FOREIGN KEY([status_id])
REFERENCES [dbo].[TourStatuses] ([status_id])
GO
ALTER TABLE [dbo].[TourGroups] CHECK CONSTRAINT [FK_TourGroups_TourStatuses]
GO

-- 5. Xóa bỏ bảng Status cũ cùng dữ liệu của nó (Vì đã không còn được trỏ tới)
DROP TABLE [dbo].[Statuses]
GO
