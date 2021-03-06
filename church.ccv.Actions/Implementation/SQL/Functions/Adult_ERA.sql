USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Adult_IsERA]    Script Date: 9/25/2016 4:29:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IsERA: Determines if the given person is an ERA or not.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Adult_IsERA](@PersonId int)
RETURNS @IsERATable TABLE
(
	PersonId int PRIMARY KEY NOT NULL,
	IsERA bit
)
AS
BEGIN

	-- Setup the values that can change based on what we define as an ERA
	DECLARE @ERAAttribKey varchar(MAX) = 'CurrentlyanERA'

	DECLARE @IsERA bit = 0
	
	-- grab the state of ERA for the user
	SELECT @IsERA = 
		CASE 
			WHEN av.[Value] LIKE 'True' THEN 1
			ELSE 0
		END
	FROM [dbo].AttributeValue av
		INNER JOIN [dbo].Attribute a ON a.Id = av.AttributeId
		INNER JOIN [dbo].Person p ON av.EntityId = p.Id
	WHERE 
		a.[Key] = @ERAAttribKey AND p.Id = @PersonId

	BEGIN
		INSERT @IsERATable
		SELECT @PersonId, @IsERA
	END
	RETURN;
END
