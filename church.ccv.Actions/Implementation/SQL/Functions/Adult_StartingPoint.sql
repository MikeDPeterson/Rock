USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Adult_TakenStartingPoint]    Script Date: 9/25/2016 4:41:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--StartingPoint: Determines if the given person has taken Starting Point or not.
--Returns a row with their person ID and either a starting point date, or a null value if they haven't taken it.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Adult_TakenStartingPoint](@PersonId int)
RETURNS @TakenStartingPointTable TABLE
(
	PersonId int PRIMARY KEY NOT NULL,
	TakenStartingPoint bit,
	StartingPointDate datetime NULL
)
AS
BEGIN
	-- Setup the values that can change based on what we define
	DECLARE @StartingPointGroupTypeId int = 57
	

	---- grab their date they took starting point
	DECLARE @StartingPointDate datetime = null
	DECLARE @TakenStartingPoint bit = 0
	
    SELECT TOP 1 @StartingPointDate = A.StartDateTime
    FROM Person P
    INNER JOIN PersonAlias PA ON PA.PersonId = P.Id
    INNER JOIN Attendance A ON A.PersonAliasId = PA.Id
    INNER JOIN [Group] G ON G.Id = A.GroupId
    WHERE G.GroupTypeId = @StartingPointGroupTypeId
	    AND A.DidAttend = 1
        AND P.Id = @PersonId
    ORDER BY A.StartDateTime DESC
    
	--treat a null starting point date as "False" for TakenStartingPoint
	SET @TakenStartingPoint = 
		CASE
			WHEN @StartingPointDate IS NOT NULL 
				THEN 1
			ELSE
				0
		END

	-- Store their ID and their date of starting point (or null if they've not taken it)
	BEGIN
		INSERT @TakenStartingPointTable
		SELECT @PersonId, @TakenStartingPoint, @StartingPointDate;
	END
	RETURN;
END
