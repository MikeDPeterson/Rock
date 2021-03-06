USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Adult_IsMentored]    Script Date: 9/28/2016 3:55:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IsMentored: Determines if the given person is being "mentored".
--This means they're in at least one group where someone is teaching / mentoring them.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Adult_IsMentored](@PersonId int, @ActiveMemberRequired bit)
RETURNS @IsMentoredTable TABLE
(
	PersonId int NOT NULL,

	Neighborhood_IsMentored bit, --True if they are, false if they're not
    Neighborhood_GroupIds varchar(MAX) NULL,
    
    YoungAdult_IsMentored bit,
    YoungAdult_GroupIds varchar(MAX) NULL,

    NextSteps_IsMentored bit,
    NextSteps_GroupIds varchar(MAX) NULL
)
AS
BEGIN

	-- TODO: It's possible a Next Steps "co-coach" will have this return true. We probably need per-group member type filtering :\
	-- The good news is the way it's setup now matches the current design that's rolled out in production.


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



    -- NEIGHBORHOOD GROUP
    --------------------
    DECLARE @Neighborhood_Group_GroupTypeId int = 49
    DECLARE @Neighborhood_GroupIds varchar(MAX)
	
    -- Get all groups the person is mentored in
    DECLARE @Neighborhood_GroupIdTable table( id int )
	INSERT INTO @Neighborhood_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
        INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @Neighborhood_Group_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 0 AND --Make sure their role in the group is NOT Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @Neighborhood_GroupIds = COALESCE(@Neighborhood_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @Neighborhood_GroupIdTable

    -- if there's at least one group, then the person's mentored
    DECLARE @Neighborhood_IsMentored bit
	
	IF LEN(@Neighborhood_GroupIds) > 0 
		SET @Neighborhood_IsMentored = 1
	ELSE
		SET @Neighborhood_IsMentored = 0
    ----------------------


    -- YOUNG ADULT GROUP
    --------------------
    DECLARE @YoungAdult_Group_GroupTypeId int = 98
    DECLARE @YoungAdult_GroupIds varchar(MAX)
	
    -- Get all groups the person is mentored in
    DECLARE @YoungAdult_GroupIdTable table( id int )
	INSERT INTO @YoungAdult_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
        INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @YoungAdult_Group_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 0 AND --Make sure their role in the group is NOT Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @YoungAdult_GroupIds = COALESCE(@YoungAdult_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @YoungAdult_GroupIdTable

    -- if there's at least one group, then the person's mentored
    DECLARE @YoungAdult_IsMentored bit
	
	IF LEN(@YoungAdult_GroupIds) > 0 
		SET @YoungAdult_IsMentored = 1
	ELSE
		SET @YoungAdult_IsMentored = 0
    ----------------------



    -- NEXT STEPS GROUP
    --------------------
    DECLARE @NextSteps_Group_GroupTypeId int = 78
    DECLARE @NextSteps_GroupIds varchar(MAX)
	
    -- Get all groups the person is mentored in
    DECLARE @NextSteps_GroupIdTable table( id int )
	INSERT INTO @NextSteps_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
        INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @NextSteps_Group_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 0 AND --Make sure their role in the group is NOT Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @NextSteps_GroupIds = COALESCE(@NextSteps_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @NextSteps_GroupIdTable

    -- if there's at least one group, then the person's mentored
    DECLARE @NextSteps_IsMentored bit
	
	IF LEN(@NextSteps_GroupIds) > 0 
		SET @NextSteps_IsMentored = 1
	ELSE
		SET @NextSteps_IsMentored = 0
    ----------------------


	--Put the results into a single row we'll return
	INSERT INTO @IsMentoredTable( 
        PersonId, 
        
        Neighborhood_IsMentored, 
        Neighborhood_GroupIds, 
        
        YoungAdult_IsMentored, 
        YoungAdult_GroupIds,
        
        NextSteps_IsMentored, 
        NextSteps_GroupIds )
	SELECT 
		@PersonId, 
		
        @Neighborhood_IsMentored, 
		@Neighborhood_GroupIds,

        @YoungAdult_IsMentored,
        @YoungAdult_GroupIds,

        @NextSteps_IsMentored,
        @NextSteps_GroupIds

	RETURN;
END
