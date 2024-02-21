USE [ElevatorAction]
GO

/****** Object:  Table [dbo].[ElevatorTypes]    Script Date: 2024/02/21 20:35:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ElevatorTypes](
	[ElevatorTypeID] [int] IDENTITY(1,1) NOT NULL,
	[ElevatorTypeDescription] [nvarchar](100) NULL,
 CONSTRAINT [PK_ElevatorTypes] PRIMARY KEY CLUSTERED 
(
	[ElevatorTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

