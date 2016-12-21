USE [LocalhostDb]
GO

/****** Object:  Table [dbo].[catTurneroConsultorio]    Script Date: 12/12/2016 6:33:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[catTurneroConsultorio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_catTurnero] [int] NOT NULL,
	[consultorio] [varchar](50) NULL
) ON [PRIMARY]

GO


