USE [TourManagement]
GO

-- 1. Thêm Roles (Tắt IDENTITY_INSERT rồ bật lại để tự định nghĩa ID)
SET IDENTITY_INSERT [dbo].[Roles] ON;
INSERT INTO [dbo].[Roles] ([role_id], [role_name], [created_at]) VALUES 
(1, N'admin', GETDATE()),
(2, N'staff', GETDATE()),
(3, N'customer', GETDATE());
SET IDENTITY_INSERT [dbo].[Roles] OFF;
GO

-- 2. Thêm Users (Tắt/Bật IDENTITY_INSERT tương tự)
SET IDENTITY_INSERT [dbo].[Users] ON;
INSERT INTO [dbo].[Users] ([id], [name], [email], [password], [phone], [dob], [address], [gender], [role_id], [is_active], [created_at]) VALUES 
(1, N'System Admin', N'admin@tour.com', N'123', N'0987654321', '1990-01-01', N'Hà Nội', N'M', 1, 1, GETDATE()),
(2, N'Jane Staff', N'staff@tour.com', N'123', N'0987654322', '1995-05-15', N'Đà Nẵng', N'F', 2, 1, GETDATE()),
(3, N'John Customer', N'customer@tour.com', N'123', N'0987654323', '2000-10-20', N'TP. Hồ Chí Minh', N'M', 3, 1, GETDATE());
SET IDENTITY_INSERT [dbo].[Users] OFF;
GO

-- 3. Thêm BookingStatuses
INSERT INTO [dbo].[BookingStatuses] ([status_id], [status_name], [description]) VALUES 
('PENDING', N'Chờ xử lý', N'Booking vừa tạo, chờ kiểm tra và thanh toán'),
('CONFIRMED', N'Đã xác nhận', N'Booking đã được duyệt và giữ chỗ'),
('APPROVED', N'Đã phê duyệt', N'Tương tự Confirmed'),
('BOOKED', N'Đã đặt', N'Tương tự Confirmed'),
('COMPLETED', N'Hoàn thành', N'Khách đã đi tour xong'),
('CANCELLED', N'Đã hủy', N'Booking bị hủy bỏ'),
('CANCELED', N'Đã hủy', N'Booking bị hủy bỏ (Alias)'),
('REJECTED', N'Bị từ chối', N'Booking bị từ chối do hết chỗ hoặc lỗi');
GO

-- 4. Thêm PaymentStatuses
INSERT INTO [dbo].[PaymentStatuses] ([status_id], [status_name], [description]) VALUES 
('PENDING', N'Chờ thanh toán', N'Chưa nhận được tiền'),
('PAID', N'Đã thanh toán', N'Đã thanh toán đủ'),
('SUCCESS', N'Thành công', N'Giao dịch thành công'),
('COMPLETED', N'Hoàn thành', N'Thanh toán hoàn tất'),
('FAILED', N'Thất bại', N'Giao dịch thất bại');
GO

-- 5. Thêm TourStatuses
INSERT INTO [dbo].[TourStatuses] ([status_id], [status_name], [description]) VALUES 
('OPEN', N'Mở bán', N'Đang nhận khách'),
('CLOSED', N'Đóng', N'Đã đủ khách hoặc chốt sổ'),
('ONGOING', N'Đang diễn ra', N'Đoàn đang đi tour'),
('RUNNING', N'Đang chạy', N'Đoàn đang đi tour'),
('COMPLETED', N'Hoàn thành', N'Tour đã đi xong trở về'),
('CANCELLED', N'Hủy', N'Tour bị hủy do thời tiết hoặc không đủ khách');
GO

-- 6. Thêm Genders
INSERT INTO [dbo].[Genders] ([gender_id], [gender_name]) VALUES 
('M', N'Nam'),
('F', N'Nữ'),
('O', N'Khác');
GO

-- 7. Thêm PassengerCategories
INSERT INTO [dbo].[PassengerCategories] ([category_id], [category_name], [description]) VALUES 
('ADULT', N'Người lớn', N'Từ 12 tuổi trở lên'),
('CHILD', N'Trẻ em', N'Từ 2 đến dưới 12 tuổi'),
('INFANT', N'Em bé', N'Dưới 2 tuổi');
GO

-- 8. Thêm Destinations
INSERT INTO [dbo].[Destinations] ([destination_id], [name], [city], [country], [description]) VALUES 
('DEST001', N'Vịnh Hạ Long', N'Quảng Ninh', N'Việt Nam', N'Kỳ quan thiên nhiên thế giới'),
('DEST002', N'Bà Nà Hills', N'Đà Nẵng', N'Việt Nam', N'Khu vui chơi giải trí trên núi'),
('DEST003', N'Vinpearl Land', N'Nha Trang', N'Việt Nam', N'Thiên đường giải trí nhiệt đới');
GO

-- 9. Thêm Tours
INSERT INTO [dbo].[Tours] ([tour_id], [name], [destination_id], [duration_days], [base_price], [category], [max_participants], [description], [created_by], [created_at], [guide_id], [ImageUrl]) VALUES 
('TOUR001', N'Tour Khám Phá Hạ Long 3N2Đ', 'DEST001', 3, 2500000, N'Nghỉ dưỡng', 40, N'Trải nghiệm du thuyền chuẩn 5 sao trên vịnh Hạ Long', 1, GETDATE(), 2, NULL),
('TOUR002', N'Tour Đà Nẵng - Hội An 4N3Đ', 'DEST002', 4, 4500000, N'Khám phá', 45, N'Tự do vui chơi trải nghiệm tại Đà Nẵng và phố cổ Hội An', 1, GETDATE(), 2, NULL);
GO

-- 10. Thêm TourPrices (Khuyến mãi, giá vé các đợt)
INSERT INTO [dbo].[TourPrices] ([price_id], [tour_id], [price], [valid_from], [valid_to], [notes]) VALUES 
('PRICE001', 'TOUR001', 2500000, '2026-01-01', '2026-12-31', N'Giá niêm yết năm 2026'),
('PRICE002', 'TOUR002', 4500000, '2026-01-01', '2026-12-31', N'Giá niêm yết năm 2026');
GO

-- 11. Thêm TourGroups (Các đợt khởi hành)
INSERT INTO [dbo].[TourGroups] ([group_id], [tour_id], [depart_date], [return_date], [max_capacity], [current_bookings], [status_id], [created_at]) VALUES 
('GRP001', 'TOUR001', '2026-04-10', '2026-04-12', 40, 2, 'OPEN', GETDATE()),
('GRP002', 'TOUR002', '2026-05-01', '2026-05-04', 45, 0, 'OPEN', GETDATE());
GO

-- 12. Thêm Bookings (Khách hàng đặt tour)
INSERT INTO [dbo].[Bookings] ([booking_id], [user_id], [group_id], [booking_date], [adults], [children], [infants], [total_price], [status_id], [notes], [created_at]) VALUES 
('BK001', 3, 'GRP001', GETDATE(), 2, 0, 0, 5000000, 'PENDING', N'Khách yêu cầu ăn chay', GETDATE());
GO

-- 13. Thêm Payments (Thanh toán của đơn hàng)
INSERT INTO [dbo].[Payments] ([payment_id], [booking_id], [amount], [payment_date], [method], [transaction_ref], [status_id], [created_at]) VALUES 
('PAY001', 'BK001', 5000000, GETDATE(), N'Chuyển khoản', 'TXN123456789', 'PENDING', GETDATE());
GO

-- 14. Thêm BookingPassengers (Danh sách hành khách chi tiết)
INSERT INTO [dbo].[BookingPassengers] ([passenger_id], [booking_id], [full_name], [gender_id], [dob], [category_id], [passport_no], [notes]) VALUES 
('PAX001', 'BK001', N'John Customer', 'M', '2000-10-20', 'ADULT', 'X99999999', NULL),
('PAX002', 'BK001', N'Marry Customer', 'F', '2000-05-05', 'ADULT', 'X88888888', NULL);
GO
