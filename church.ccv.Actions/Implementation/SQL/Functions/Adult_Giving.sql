USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Adult_IsGiving]    Script Date: 9/25/2016 4:37:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Is Giving: Determines if the given person is giving or not.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Adult_IsGiving](@PersonId int)
RETURNS @IsGivingTable TABLE
(
	PersonId int PRIMARY KEY NOT NULL,
	IsGiving bit --True if they are, false if they're not
)
AS
BEGIN

	-- Setup the values that can change based on what we define
	DECLARE @MinGiving money = 250
	DECLARE @GivingHistoryAttribKey varchar(MAX) = 'GivingInLast12Months'
		
	-- grab the amount the user has given
	DECLARE @IsGiving bit = 0
	DECLARE @GivingAmount money

	SELECT @GivingAmount = av.[Value]
	FROM [dbo].AttributeValue av
		INNER JOIN [dbo].Attribute a ON a.Id = av.AttributeId
		INNER JOIN [dbo].Person p ON av.EntityId = p.Id
	WHERE 
		a.[Key] = @GivingHistoryAttribKey AND p.Id = @PersonId

	-- set yes if it's greater than the min
	IF( @GivingAmount >= @MinGiving )
		SET @IsGiving = 1;
	ELSE
		SET @IsGiving = 0

	BEGIN
		INSERT @IsGivingTable
		SELECT @PersonId, @IsGiving
	END
	RETURN;
END
