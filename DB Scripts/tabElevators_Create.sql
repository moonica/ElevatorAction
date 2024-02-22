USE [ElevatorAction]
GO

/****** Object:  Table [dbo].[Elevators]    Script Date: 2024/02/21 20:35:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Elevators](
	[ElevatorID] [int] IDENTITY(1,1) NOT NULL,
	[ElevatorName] [nvarchar](100) NULL,
	[MaxCapacity] [float] NOT NULL,
	[ElevatorType] [int] NULL,
	[Model] [nvarchar](250) NULL,
	[SerialNr] [nvarchar](250) NULL
 CONSTRAINT [PK_Elevators] PRIMARY KEY CLUSTERED 
(
	[ElevatorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Elevators]  WITH CHECK ADD  CONSTRAINT [FK_Elevators_ElevatorTypes] FOREIGN KEY([ElevatorType])
REFERENCES [dbo].[ElevatorTypes] ([ElevatorTypeID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[Elevators] CHECK CONSTRAINT [FK_Elevators_ElevatorTypes]
GO

