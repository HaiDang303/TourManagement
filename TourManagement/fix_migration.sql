USE [TourManagement]
GO

-- 1. Ép các bản ghi TourGroup có Status rác (nếu có) trở về 'OPEN'
UPDATE [dbo].[TourGroups] 
SET status_id = 'OPEN' 
WHERE status_id NOT IN ('OPEN', 'FULL', 'DEPARTED', 'COMPLETED');
GO

-- 2. Thử lại việc THÊM Khóa ngoại (Foreign Keys) cho TourGroups
ALTER TABLE [dbo].[TourGroups]  WITH CHECK ADD  CONSTRAINT [FK_TourGroups_TourStatuses] FOREIGN KEY([status_id])
REFERENCES [dbo].[TourStatuses] ([status_id])
GO

ALTER TABLE [dbo].[TourGroups] CHECK CONSTRAINT [FK_TourGroups_TourStatuses]
GO

-- 3. Xóa bỏ bảng Status cũ cùng dữ liệu của nó
DROP TABLE [dbo].[Statuses]
GO
