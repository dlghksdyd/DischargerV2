USE [MINDIMS_MOE]
GO
/****** Object:  StoredProcedure [dbo].[IFT_HISTORY_ALARM]    Script Date: 2025-11-07 오전 10:53:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =================================================
-- Author:		YGSONG
-- Update date: 2024-04-19
-- Description:	IFT_HISTORY_ALARM - 설비 알람 히스토리
-- History:
--	2024.04.19 YGSONG: MTYPE 추가
-- =================================================
ALTER PROCEDURE [dbo].[IFT_HISTORY_ALARM]
		@MTYPE				varchar(50)	= '',
		@MC_CD				varchar(7)	= '',
		@CH_NO				int			= 0,
		@Alarm_Treat		int			= 0,
		@Alarm_Time			datetime2	= '',
		@Alarm_Code			int			= 0,
		@Alarm_Desc			varchar(80)	= '',
		@Alarm_NewInserted	varchar(1)	= ''
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WITH(NOLOCK) WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME ='QLT_HISTORY_ALARM')
CREATE TABLE dbo.QLT_HISTORY_ALARM (
                                       MTYPE				varchar(50)	NOT NULL,
                                       MC_CD				varchar(7)	NOT NULL,
                                       CH_NO				int			NOT NULL,
                                       Alarm_Treat			int			NULL,
                                       Alarm_Time			datetime2	NOT NULL,
                                       Alarm_Code			int			NULL,
                                       Alarm_Desc			varchar(80)	NOT NULL,
                                       Alarm_NewInserted	varchar(1)	NULL,
)

    IF NOT EXISTS (SELECT *, ROW_NUMBER() OVER(ORDER BY Alarm_Time DESC)  AS ROWNUM 
					FROM dbo.QLT_HISTORY_ALARM
					WHERE MTYPE = @MTYPE AND MC_CD = @MC_CD AND Alarm_Time = @Alarm_Time AND Alarm_Desc = @Alarm_Desc)
		INSERT INTO dbo.QLT_HISTORY_ALARM (MTYPE, MC_CD, CH_NO, Alarm_Treat, Alarm_Time, Alarm_Code, Alarm_Desc, Alarm_NewInserted)
			VALUES (@MTYPE, @MC_CD, @CH_NO, @Alarm_Treat, @Alarm_Time, @Alarm_Code, @Alarm_Desc, @Alarm_NewInserted)
END
