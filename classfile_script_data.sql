USE [master]
GO
/****** Object:  Database [Classfile]    Script Date: 2/28/2023 23:07:43 ******/
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
/****** Object:  Table [dbo].[account]    Script Date: 2/28/2023 23:07:43 ******/
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
 CONSTRAINT [PK_account] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[account_class]    Script Date: 2/28/2023 23:07:43 ******/
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
/****** Object:  Table [dbo].[class]    Script Date: 2/28/2023 23:07:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[class](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[class_name] [nvarchar](50) NULL,
	[teacher_account_id] [int] NULL,
 CONSTRAINT [PK_class] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[file]    Script Date: 2/28/2023 23:07:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[file](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[post_id] [int] NULL,
	[file_name] [varchar](100) NULL,
	[file_type] [varchar](10) NULL,
 CONSTRAINT [PK_file] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[post]    Script Date: 2/28/2023 23:07:43 ******/
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
