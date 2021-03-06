USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Student_IsMentored]    Script Date: 9/28/2016 4:00:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IsMentored: Determines if the given person is being "mentored".
--This means they're in at least one group where someone is teaching / mentoring them.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Student_IsMentored](@PersonId int, @ActiveMemberRequired bit)
RETURNS @IsMentoredTable TABLE
(
	PersonId int NOT NULL,

    NextGen_IsMentored bit,
    NextGen_GroupIds varchar(MAX) NULL
)
AS
BEGIN

    -- Defined a bit mask that will ensure the GroupMemberStatus
    -- is either ONLY active, or Pending / Active.
    DECLARE @GroupMemberReqMask int

    -- How this works:
    -- Pending MemberStatus = 0011 in binary (3 in decimal)
    -- Active MemberStatus = 0001 in binary  (1 in decimal)
    
    IF @ActiveMemberRequired = 1
        -- Define the mask as 0001, which means it'll only return 1 when masked against Active
        SET @GroupMemberReqMask = 1
    ELSE
        -- Define the mask as 0011, which means 1 is returned when masked against Pending OR Active
        SET @GroupMemberReqMask = 3

    -- NextGen GROUP
    --------------------
    DECLARE @NextGen_Group_GroupTypeId int = 94
    DECLARE @NextGen_GroupIds varchar(MAX)
	
    -- Get all groups the person is mentored in
    DECLARE @NextGen_GroupIdTable table( id int )
	INSERT INTO @NextGen_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
        INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @NextGen_Group_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 0 AND --Make sure their role in the group is NOT Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @NextGen_GroupIds = COALESCE(@NextGen_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @NextGen_GroupIdTable

    -- if there's at least one group, then the person's mentored
    DECLARE @NextGen_IsMentored bit
	
	IF LEN(@NextGen_GroupIds) > 0 
		SET @NextGen_IsMentored = 1
	ELSE
		SET @NextGen_IsMentored = 0
    ----------------------


	--Put the results into a single row we'll return
	INSERT INTO @IsMentoredTable( 
        PersonId, 
        
        NextGen_IsMentored, 
        NextGen_GroupIds
        )
	SELECT 
		@PersonId, 

        @NextGen_IsMentored,
        @NextGen_GroupIds

	RETURN;
END
