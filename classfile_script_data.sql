USE [master]
GO
/****** Object:  Database [Classfile]    Script Date: 3/7/2023 22:02:01 ******/
CREATE DATABASE [Classfile]
GO
ALTER DATABASE [Classfile] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Classfile] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Classfile] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Classfile] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Classfile] SET ARITHABORT OFF 
GO
ALTER DATABASE [Classfile] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Classfile] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Classfile] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Classfile] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Classfile] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Classfile] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Classfile] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Classfile] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Classfile] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Classfile] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Classfile] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Classfile] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Classfile] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Classfile] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Classfile] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Classfile] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Classfile] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Classfile] SET RECOVERY FULL 
GO
ALTER DATABASE [Classfile] SET  MULTI_USER 
GO
ALTER DATABASE [Classfile] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Classfile] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Classfile] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Classfile] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Classfile] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Classfile] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Classfile', N'ON'
GO
ALTER DATABASE [Classfile] SET QUERY_STORE = ON
GO
ALTER DATABASE [Classfile] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Classfile]
GO
/****** Object:  Table [dbo].[account]    Script Date: 3/7/2023 22:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[account](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](100) NULL,
	[password] [varchar](200) NULL,
	[fullname] [nvarchar](200) NULL,
	[account_type] [varchar](5) NULL,
	[imageAvatar] [varchar](100) NULL,
 CONSTRAINT [PK_account] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[account_class]    Script Date: 3/7/2023 22:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[account_class](
	[class_id] [int] NOT NULL,
	[account_id] [int] NOT NULL,
 CONSTRAINT [PK_student_class] PRIMARY KEY CLUSTERED 
(
	[class_id] ASC,
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[class]    Script Date: 3/7/2023 22:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[class](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[class_name] [nvarchar](50) NULL,
	[teacher_account_id] [int] NULL,
	[class_code] [varchar](10) NULL,
	[imageCover] [varchar](100) NULL,
 CONSTRAINT [PK_class] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[file]    Script Date: 3/7/2023 22:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[file](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[post_id] [int] NULL,
	[file_name] [varchar](100) NULL,
	[file_type] [varchar](100) NULL,
	[file_name_root] [varchar](200) NULL,
 CONSTRAINT [PK_file] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[post]    Script Date: 3/7/2023 22:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[post](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[class_id] [int] NULL,
	[title] [nvarchar](500) NULL,
	[posted_account_id] [int] NULL,
	[date_created] [datetime] NULL,
 CONSTRAINT [PK_post] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[account] ON 

INSERT [dbo].[account] ([id], [username], [password], [fullname], [account_type], [imageAvatar]) VALUES (1, N'area1110', N'123', N'Nguyễn Khánh Huy', N'TC', NULL)
INSERT [dbo].[account] ([id], [username], [password], [fullname], [account_type], [imageAvatar]) VALUES (2, N'teacher', N'teacher', N'Nguyễn Khánh Huy', N'TC', NULL)
SET IDENTITY_INSERT [dbo].[account] OFF
GO
INSERT [dbo].[account_class] ([class_id], [account_id]) VALUES (1, 1)
GO
SET IDENTITY_INSERT [dbo].[class] ON 

INSERT [dbo].[class] ([id], [class_name], [teacher_account_id], [class_code], [imageCover]) VALUES (1, N'PRN231', 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[class] OFF
GO
SET IDENTITY_INSERT [dbo].[file] ON 

INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (1, 1, N'1_3/5/2023 20:32:52', N'mp4', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (2, 3, N'3_20354305032023', N'mp4', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (3, 4, N'4_20565105032023.mp4', N'mp4', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (4, 5, N'5_20570005032023.mp4', N'mp4', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (5, 11, N'11_22513305032023.zip', N'application/x-zip-compressed', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (6, 12, N'12_23005305032023.exe', N'application/octet-stream', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (7, 13, N'13_23052805032023.msi', N'application/octet-stream', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (8, 13, N'13_23052805032023.exe', N'application/octet-stream', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (9, 13, N'13_23052805032023.config', N'application/xml', NULL)
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (10, 14, N'14_23160305032023.msi', N'application/octet-stream', N'NitroSense.msi')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (11, 14, N'14_23160305032023.exe', N'application/octet-stream', N'Setup.exe')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (12, 14, N'14_23160305032023.config', N'application/xml', N'Setup.exe.config')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (13, 15, N'15_23220605032023.exe', N'application/octet-stream', N'BingWallpaper.exe')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (14, 16, N'16_23333205032023.zip', N'application/x-zip-compressed', N'cmder_mini.zip')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (15, 17, N'OfficeSetup_17_23393005032023.exe', N'application/octet-stream', N'OfficeSetup.exe')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (16, 18, N'rufus-3.21_18_23580705032023.exe', N'application/octet-stream', N'rufus-3.21.exe')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (17, 18, N'SQL2022-SSEI-Dev_18_23580705032023.exe', N'application/octet-stream', N'SQL2022-SSEI-Dev.exe')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (18, 18, N'UltraViewer_setup_6.6_vi_18_23580705032023.exe', N'application/octet-stream', N'UltraViewer_setup_6.6_vi.exe')
INSERT [dbo].[file] ([id], [post_id], [file_name], [file_type], [file_name_root]) VALUES (19, 18, N'VisualStudioSetup_18_23580705032023.exe', N'application/octet-stream', N'VisualStudioSetup.exe')
SET IDENTITY_INSERT [dbo].[file] OFF
GO
SET IDENTITY_INSERT [dbo].[post] ON 

INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (1, 1, N'123456', 1, CAST(N'2023-03-05T20:32:52.667' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (3, 1, N'1234565', 1, CAST(N'2023-03-05T20:35:36.773' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (4, 1, N'1234565', 1, CAST(N'2023-03-05T20:56:51.503' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (5, 1, N'1234565', 1, CAST(N'2023-03-05T20:57:00.077' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (6, 1, N'123456', 1, CAST(N'2023-03-05T22:38:13.097' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (11, 1, N'1234', 1, CAST(N'2023-03-05T22:51:33.307' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (12, 1, N'Heheheh', 1, CAST(N'2023-03-05T23:00:52.953' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (13, 1, N'Hì hì', 1, CAST(N'2023-03-05T23:05:28.970' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (14, 1, N'Post with file rootname', 1, CAST(N'2023-03-05T23:16:03.557' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (15, 1, N'Hehehehe', 1, CAST(N'2023-03-05T23:22:06.653' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (16, 1, N'Thử file size', 1, CAST(N'2023-03-05T23:33:32.143' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (17, 1, N'', 1, CAST(N'2023-03-05T23:39:30.120' AS DateTime))
INSERT [dbo].[post] ([id], [class_id], [title], [posted_account_id], [date_created]) VALUES (18, 1, N'test nì', 1, CAST(N'2023-03-05T23:58:07.593' AS DateTime))
SET IDENTITY_INSERT [dbo].[post] OFF
GO
ALTER TABLE [dbo].[account_class]  WITH CHECK ADD  CONSTRAINT [FK_account_class_account] FOREIGN KEY([account_id])
REFERENCES [dbo].[account] ([id])
GO
ALTER TABLE [dbo].[account_class] CHECK CONSTRAINT [FK_account_class_account]
GO
ALTER TABLE [dbo].[account_class]  WITH CHECK ADD  CONSTRAINT [FK_account_class_class] FOREIGN KEY([class_id])
REFERENCES [dbo].[class] ([id])
GO
ALTER TABLE [dbo].[account_class] CHECK CONSTRAINT [FK_account_class_class]
GO
ALTER TABLE [dbo].[class]  WITH CHECK ADD  CONSTRAINT [FK_class_account] FOREIGN KEY([teacher_account_id])
REFERENCES [dbo].[account] ([id])
GO
ALTER TABLE [dbo].[class] CHECK CONSTRAINT [FK_class_account]
GO
ALTER TABLE [dbo].[file]  WITH CHECK ADD  CONSTRAINT [FK_file_post] FOREIGN KEY([post_id])
REFERENCES [dbo].[post] ([id])
GO
ALTER TABLE [dbo].[file] CHECK CONSTRAINT [FK_file_post]
GO
ALTER TABLE [dbo].[post]  WITH CHECK ADD  CONSTRAINT [FK_post_account] FOREIGN KEY([posted_account_id])
REFERENCES [dbo].[account] ([id])
GO
ALTER TABLE [dbo].[post] CHECK CONSTRAINT [FK_post_account]
GO
ALTER TABLE [dbo].[post]  WITH CHECK ADD  CONSTRAINT [FK_post_class] FOREIGN KEY([class_id])
REFERENCES [dbo].[class] ([id])
GO
ALTER TABLE [dbo].[post] CHECK CONSTRAINT [FK_post_class]
GO
USE [master]
GO
ALTER DATABASE [Classfile] SET  READ_WRITE 
GO
