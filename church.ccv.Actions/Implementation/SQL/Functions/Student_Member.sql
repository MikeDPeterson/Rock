USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Student_IsMember]    Script Date: 9/25/2016 4:42:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IsMember: Determines if the given person is a member of CCV.
--Returns a row with their person ID and either a membership date, or a null value if they're not.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Student_IsMember](@PersonId int)
RETURNS @IsMemberTable TABLE
(
	PersonId int PRIMARY KEY NOT NULL,
	IsMember bit,
	MembershipDate datetime NULL
)
AS
BEGIN
	-- Setup the values that can change based on what we define
	DECLARE @MemberAttribKey varchar(MAX) = 'DateofMembership'
	DECLARE @MemberConnectionStatus int = 65
	

	-- grab their date of membership
	DECLARE @MembershipDate datetime
	DECLARE @IsMember bit = 0
	
	SELECT @MembershipDate = av.ValueAsDateTime
	FROM [dbo].AttributeValue av
		INNER JOIN [dbo].Attribute a ON a.Id = av.AttributeId
		INNER JOIN [dbo].Person p ON av.EntityId = p.Id AND p.ConnectionStatusValueId = @MemberConnectionStatus
	WHERE
		a.[Key] = @MemberAttribKey AND p.Id = @PersonId

	--treat a null membership date as "False" for IsMember
	SET @IsMember = 
		CASE
			WHEN @MembershipDate IS NOT NULL 
				THEN 1
			ELSE
				0
		END

	-- Store their ID and their date of membership (or null if they're not a member)
	BEGIN
		INSERT @IsMemberTable
		SELECT @PersonId, @IsMember, @MembershipDate;
	END
	RETURN;
END
