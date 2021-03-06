USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Student_IsBaptised]    Script Date: 9/25/2016 4:27:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IsBaptised: Determines if the given person has been baptised, and when.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Student_IsBaptised](@PersonId int )
RETURNS @IsBaptisedTable TABLE
(
	PersonId int PRIMARY KEY NOT NULL,
	IsBaptised bit,
	BaptisedDate datetime NULL
)
AS
BEGIN
	-- Setup the values that can change based on what we define to be baptised
	DECLARE @BaptismAttributeId int = 174
		
	-- default return values to null
	DECLARE @BaptismDate datetime
	DECLARE @IsBaptised bit = 0

	--First get the baptism date, which can either be a date, 0, or null.
	SELECT @BaptismDate = av.ValueAsDateTime
	FROM [dbo].Person p
		INNER JOIN [dbo].AttributeValue av ON av.EntityId = p.Id
	WHERE 
		p.Id = @PersonId AND av.AttributeId = @BaptismAttributeId

	--we'll treat a null baptism date as "False" for IsBaptised
	SET @IsBaptised = 
		CASE
			WHEN @BaptismDate IS NOT NULL 
				THEN 1
			ELSE
				0
		END

	-- Fill the returning table row and we're done
	BEGIN
		INSERT @IsBaptisedTable
		SELECT @PersonId, @IsBaptised, @BaptismDate;
	END
	RETURN;
END

