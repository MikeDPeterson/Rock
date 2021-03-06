USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_GetTrainingGroupIds]    Script Date: 11/17/2016 4:23:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Returns a list of group IDs that are considered Training Groups, and should
-- be ignored by actions utilizing groups. (IsMentored, IsPeerLearning, IsTeaching, etc.)
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_GetTrainingGroupIds]()
RETURNS @TrainingGroupIds TABLE
(
    Id int
)
AS
BEGIN
    INSERT INTO @TrainingGroupIds
    
    -- Place all training groups here
          SELECT 1713512 
    UNION SELECT 1713518 
    UNION SELECT 1650304 
    UNION SELECT 1747187
    UNION SELECT 1695044
    UNION SELECT 1652917
    UNION SELECT 1726784
    UNION SELECT 1795721
    UNION SELECT 1813229
    UNION SELECT 1795723
    UNION SELECT 1738069
    --

    RETURN;
END

