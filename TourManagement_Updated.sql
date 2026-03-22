USE [master]
GO
/****** Object:  Database [TourManagement]    Script Date: 3/22/2026 2:38:48 AM ******/
CREATE DATABASE [TourManagement]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TourManagement', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TourManagement.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TourManagement_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TourManagement_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TourManagement] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TourManagement].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TourManagement] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TourManagement] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TourManagement] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TourManagement] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TourManagement] SET ARITHABORT OFF 
GO
ALTER DATABASE [TourManagement] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TourManagement] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TourManagement] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TourManagement] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TourManagement] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TourManagement] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TourManagement] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TourManagement] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TourManagement] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TourManagement] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TourManagement] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TourManagement] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TourManagement] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TourManagement] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TourManagement] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TourManagement] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TourManagement] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TourManagement] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TourManagement] SET  MULTI_USER 
GO
ALTER DATABASE [TourManagement] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TourManagement] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TourManagement] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TourManagement] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TourManagement] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TourManagement] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [TourManagement] SET QUERY_STORE = ON
GO
ALTER DATABASE [TourManagement] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TourManagement]
GO
/****** Object:  User [Admin]    Script Date: 3/22/2026 2:38:48 AM ******/
CREATE USER [Admin] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[BookingPassengers]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingPassengers](
	[passenger_id] [varchar](20) NOT NULL,
	[booking_id] [varchar](20) NOT NULL,
	[full_name] [nvarchar](100) NOT NULL,
	[gender_id] [varchar](2) NULL,
	[dob] [date] NULL,
	[category_id] [varchar](10) NOT NULL,
	[passport_no] [varchar](50) NULL,
	[notes] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[passenger_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[booking_id] [varchar](20) NOT NULL,
	[user_id] [int] NOT NULL,
	[group_id] [varchar](20) NOT NULL,
	[booking_date] [datetime] NOT NULL,
	[adults] [int] NOT NULL,
	[children] [int] NOT NULL,
	[infants] [int] NOT NULL,
	[total_price] [decimal](18, 2) NOT NULL,
	[status_id] [varchar](10) NOT NULL,
	[notes] [nvarchar](max) NULL,
	[created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[booking_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Destinations]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Destinations](
	[destination_id] [varchar](20) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[city] [nvarchar](100) NOT NULL,
	[country] [nvarchar](100) NULL,
	[description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[destination_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Genders]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Genders](
	[gender_id] [varchar](2) NOT NULL,
	[gender_name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[gender_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PassengerCategories]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PassengerCategories](
	[category_id] [varchar](10) NOT NULL,
	[category_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](150) NULL,
PRIMARY KEY CLUSTERED 
(
	[category_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[payment_id] [varchar](20) NOT NULL,
	[booking_id] [varchar](20) NOT NULL,
	[amount] [decimal](18, 2) NOT NULL,
	[payment_date] [datetime] NOT NULL,
	[method] [nvarchar](50) NOT NULL,
	[transaction_ref] [varchar](100) NULL,
	[status_id] [varchar](10) NOT NULL,
	[created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[payment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [nvarchar](50) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[role_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingStatuses]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingStatuses](
	[status_id] [varchar](10) NOT NULL,
	[status_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentStatuses]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentStatuses](
	[status_id] [varchar](10) NOT NULL,
	[status_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TourStatuses]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TourStatuses](
	[status_id] [varchar](10) NOT NULL,
	[status_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TourGroups]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TourGroups](
	[group_id] [varchar](20) NOT NULL,
	[tour_id] [varchar](20) NOT NULL,
	[depart_date] [date] NOT NULL,
	[return_date] [date] NOT NULL,
	[max_capacity] [int] NOT NULL,
	[current_bookings] [int] NOT NULL,
	[status_id] [varchar](10) NOT NULL,
	[created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TourPrices]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TourPrices](
	[price_id] [varchar](20) NOT NULL,
	[tour_id] [varchar](20) NOT NULL,
	[price] [decimal](18, 2) NOT NULL,
	[valid_from] [date] NOT NULL,
	[valid_to] [date] NOT NULL,
	[notes] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[price_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tours]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tours](
	[tour_id] [varchar](20) NOT NULL,
	[name] [nvarchar](150) NOT NULL,
	[destination_id] [varchar](20) NOT NULL,
	[duration_days] [int] NOT NULL,
	[base_price] [decimal](18, 2) NOT NULL,
	[category] [nvarchar](50) NULL,
	[max_participants] [int] NULL,
	[description] [nvarchar](max) NULL,
	[created_by] [int] NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[guide_id] [int] NULL,
	[ImageUrl] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[tour_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/22/2026 2:38:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[email] [nvarchar](150) NOT NULL,
	[password] [nvarchar](255) NOT NULL,
	[phone] [nvarchar](20) NULL,
	[dob] [date] NULL,
	[address] [nvarchar](255) NULL,
	[gender] [nvarchar](20) NULL,
	[role_id] [int] NOT NULL,
	[is_active] [bit] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[last_login] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tours_CreatedAt]    Script Date: 3/22/2026 2:38:48 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tours_CreatedAt] ON [dbo].[Tours]
(
	[created_at] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (getdate()) FOR [booking_date]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ((1)) FOR [adults]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ((0)) FOR [children]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ((0)) FOR [infants]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ('PENDING') FOR [status_id]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (getdate()) FOR [payment_date]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT ('PENDING') FOR [status_id]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[TourGroups] ADD  DEFAULT ((50)) FOR [max_capacity]
GO
ALTER TABLE [dbo].[TourGroups] ADD  DEFAULT ((0)) FOR [current_bookings]
GO
ALTER TABLE [dbo].[TourGroups] ADD  DEFAULT ('OPEN') FOR [status_id]
GO
ALTER TABLE [dbo].[TourGroups] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Tours] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[BookingPassengers]  WITH CHECK ADD  CONSTRAINT [FK_BookingPassengers_Bookings] FOREIGN KEY([booking_id])
REFERENCES [dbo].[Bookings] ([booking_id])
GO
ALTER TABLE [dbo].[BookingPassengers] CHECK CONSTRAINT [FK_BookingPassengers_Bookings]
GO
ALTER TABLE [dbo].[BookingPassengers]  WITH CHECK ADD  CONSTRAINT [FK_BookingPassengers_Genders] FOREIGN KEY([gender_id])
REFERENCES [dbo].[Genders] ([gender_id])
GO
ALTER TABLE [dbo].[BookingPassengers] CHECK CONSTRAINT [FK_BookingPassengers_Genders]
GO
ALTER TABLE [dbo].[BookingPassengers]  WITH CHECK ADD  CONSTRAINT [FK_BookingPassengers_PassengerCat] FOREIGN KEY([category_id])
REFERENCES [dbo].[PassengerCategories] ([category_id])
GO
ALTER TABLE [dbo].[BookingPassengers] CHECK CONSTRAINT [FK_BookingPassengers_PassengerCat]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_BookingStatuses] FOREIGN KEY([status_id])
REFERENCES [dbo].[BookingStatuses] ([status_id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_BookingStatuses]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_TourGroups] FOREIGN KEY([group_id])
REFERENCES [dbo].[TourGroups] ([group_id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_TourGroups]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_Users]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Bookings] FOREIGN KEY([booking_id])
REFERENCES [dbo].[Bookings] ([booking_id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Bookings]
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
ALTER TABLE [dbo].[TourGroups]  WITH CHECK ADD  CONSTRAINT [FK_TourGroups_Tours] FOREIGN KEY([tour_id])
REFERENCES [dbo].[Tours] ([tour_id])
GO
ALTER TABLE [dbo].[TourGroups] CHECK CONSTRAINT [FK_TourGroups_Tours]
GO
ALTER TABLE [dbo].[TourPrices]  WITH CHECK ADD  CONSTRAINT [FK_TourPrices_Tours] FOREIGN KEY([tour_id])
REFERENCES [dbo].[Tours] ([tour_id])
GO
ALTER TABLE [dbo].[TourPrices] CHECK CONSTRAINT [FK_TourPrices_Tours]
GO
ALTER TABLE [dbo].[Tours]  WITH CHECK ADD  CONSTRAINT [FK_Tours_Destinations] FOREIGN KEY([destination_id])
REFERENCES [dbo].[Destinations] ([destination_id])
GO
ALTER TABLE [dbo].[Tours] CHECK CONSTRAINT [FK_Tours_Destinations]
GO
ALTER TABLE [dbo].[Tours]  WITH CHECK ADD  CONSTRAINT [FK_Tours_Guide] FOREIGN KEY([guide_id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Tours] CHECK CONSTRAINT [FK_Tours_Guide]
GO
ALTER TABLE [dbo].[Tours]  WITH CHECK ADD  CONSTRAINT [FK_Tours_Users] FOREIGN KEY([created_by])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Tours] CHECK CONSTRAINT [FK_Tours_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[Roles] ([role_id])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
ALTER TABLE [dbo].[TourGroups]  WITH CHECK ADD  CONSTRAINT [CHK_CurrentBookings] CHECK  (([current_bookings]>=(0) AND [current_bookings]<=[max_capacity]))
GO
ALTER TABLE [dbo].[TourGroups] CHECK CONSTRAINT [CHK_CurrentBookings]
GO
USE [master]
GO
ALTER DATABASE [TourManagement] SET  READ_WRITE 
GO
