USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Adult_IsTeaching]    Script Date: 9/28/2016 3:59:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IsTeaching: Returns true if the person is in at least one "Teacing" group, as a "leader" (see below on that)
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Adult_IsTeaching](@PersonId int, @ActiveMemberRequired bit)
RETURNS @IsTeachingTable TABLE
(
	PersonId int NOT NULL,
	
    --Neighborhood
    Neighborhood_SubSection_IsTeaching bit,
    Neighborhood_SubSection_GroupIds varchar(MAX) NULL,

    Neighborhood_IsTeaching bit,
    Neighborhood_GroupIds varchar(MAX) NULL,
    --

    --Young Adult
    YoungAdult_Section_IsTeaching bit,
    YoungAdult_Section_GroupIds varchar(MAX) NULL,

    YoungAdult_IsTeaching bit,
    YoungAdult_GroupIds varchar(MAX) NULL,
    --

    --Next Steps
    NextSteps_SubSection_IsTeaching bit,
    NextSteps_SubSection_GroupIds varchar(MAX) NULL,

    NextSteps_IsTeaching bit,
    NextSteps_GroupIds varchar(MAX) NULL,
	--

    --Next Gen
    NextGen_Section_IsTeaching bit,
    NextGen_Section_GroupIds varchar(MAX) NULL,

    NextGen_IsTeaching bit,
    NextGen_GroupIds varchar(MAX) NULL
    --
)
AS
BEGIN

    

    -- Note: Being IN the group isn't enough. The person's role in the group
	-- must have the "IsLeader" flag set. 
		
	--Example: An Attendee in a Neighborhood Group wouldn't have true returned.
	-- A Co-Coach in a Neighborhood Group would also _NOT_ have true returned. (Because despite the name, the IsLeader flag is not set on that role type)
	-- A COACH in a Neighborhood Group WOULD have true returned, because they're the only group role type that's flagged as a leader.
	-- I know.

	-- To see which roles count, see Rock -> General Settings-> Group Types-> One of the above Groups.
	-- Expand the Roles panel, and there you'll see which role types are "Leaders" and therefore get Teaching credit.


    --Teaching groups all have a "Section" group above them, where someon can oversee multiple groups.

	-- I'm going to use McDonald's franchises as an analogy for these.
    --@Neighborhood_SubSection_GroupTypeId	(Regional Manager over many McDonald's stores)
    --@Neighborhood_Group_GroupTypeId		(Manager of a specific McDonald's store)
    
    --@YoungAdult_Section_GroupTypeId		(Regional Manager over many McDonald's stores)
    --@YoungAdult_Group_GroupTypeId			(Manager of a specific McDonald's store)
    
    --@NextSteps_SubSection_GroupTypeId		(Regional Manager over many McDonald's stores)
    --@NextSteps_Group_GroupTypeId			(Manager of a specific McDonald's store)
    
    --@NextGen_Section_GroupTypeId			(Regional Manager over many McDonald's stores)
    --@NextGen_Group_GroupTypeId			(Manager of a specific McDonald's store)


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


    -- NEIGHBORHOOD SUBSECTION GROUP
    --------------------
    DECLARE @Neighborhood_SubSection_GroupTypeId int = 85
    DECLARE @Neighborhood_SubSection_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @Neighborhood_SubSection_GroupIdTable table( id int )

	INSERT INTO @Neighborhood_SubSection_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @Neighborhood_SubSection_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @Neighborhood_SubSection_GroupIds = COALESCE(@Neighborhood_SubSection_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @Neighborhood_SubSection_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @Neighborhood_SubSection_IsTeaching bit
	
	IF LEN(@Neighborhood_SubSection_GroupIds) > 0 
		SET @Neighborhood_SubSection_IsTeaching = 1
	ELSE
		SET @Neighborhood_SubSection_IsTeaching = 0
    ----------------------


    -- NEIGHBORHOOD GROUP
    --------------------
    DECLARE @Neighborhood_GroupTypeId int = 49
    DECLARE @Neighborhood_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @Neighborhood_GroupIdTable table( id int )

	INSERT INTO @Neighborhood_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @Neighborhood_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @Neighborhood_GroupIds = COALESCE(@Neighborhood_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @Neighborhood_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @Neighborhood_IsTeaching bit
	
	IF LEN(@Neighborhood_GroupIds) > 0 
		SET @Neighborhood_IsTeaching = 1
	ELSE
		SET @Neighborhood_IsTeaching = 0
    ----------------------
    


    -- YOUNG ADULT SECTION GROUP
    --------------------
    DECLARE @YoungAdult_Section_GroupTypeId int = 99
    DECLARE @YoungAdult_Section_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @YoungAdult_Section_GroupIdTable table( id int )

	INSERT INTO @YoungAdult_Section_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @YoungAdult_Section_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @YoungAdult_Section_GroupIds = COALESCE(@YoungAdult_Section_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @YoungAdult_Section_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @YoungAdult_Section_IsTeaching bit
	
	IF LEN(@YoungAdult_Section_GroupIds) > 0 
		SET @YoungAdult_Section_IsTeaching = 1
	ELSE
		SET @YoungAdult_Section_IsTeaching = 0
    ----------------------


    -- YOUNG ADULT GROUP
    --------------------
    DECLARE @YoungAdult_GroupTypeId int = 98
    DECLARE @YoungAdult_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @YoungAdult_GroupIdTable table( id int )

	INSERT INTO @YoungAdult_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @YoungAdult_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @YoungAdult_GroupIds = COALESCE(@YoungAdult_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @YoungAdult_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @YoungAdult_IsTeaching bit
	
	IF LEN(@YoungAdult_GroupIds) > 0 
		SET @YoungAdult_IsTeaching = 1
	ELSE
		SET @YoungAdult_IsTeaching = 0
    ----------------------



    -- NEXT STEPS SUBSECTION GROUP
    --------------------
    DECLARE @NextSteps_SubSection_GroupTypeId int = 82
    DECLARE @NextSteps_SubSection_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @NextSteps_SubSection_GroupIdTable table( id int )

	INSERT INTO @NextSteps_SubSection_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @NextSteps_SubSection_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @NextSteps_SubSection_GroupIds = COALESCE(@NextSteps_SubSection_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @NextSteps_SubSection_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @NextSteps_SubSection_IsTeaching bit
	
	IF LEN(@NextSteps_SubSection_GroupIds) > 0 
		SET @NextSteps_SubSection_IsTeaching = 1
	ELSE
		SET @NextSteps_SubSection_IsTeaching = 0
    ----------------------


    -- NEXT STEPS GROUP
    --------------------
    DECLARE @NextSteps_GroupTypeId int = 78
    DECLARE @NextSteps_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @NextSteps_GroupIdTable table( id int )

	INSERT INTO @NextSteps_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @NextSteps_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @NextSteps_GroupIds = COALESCE(@NextSteps_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @NextSteps_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @NextSteps_IsTeaching bit
	
	IF LEN(@NextSteps_GroupIds) > 0 
		SET @NextSteps_IsTeaching = 1
	ELSE
		SET @NextSteps_IsTeaching = 0
    ----------------------


    -- NEXT GEN SECTION GROUP
    --------------------
    DECLARE @NextGen_Section_GroupTypeId int = 95
    DECLARE @NextGen_Section_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @NextGen_Section_GroupIdTable table( id int )

	INSERT INTO @NextGen_Section_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @NextGen_Section_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @NextGen_Section_GroupIds = COALESCE(@NextGen_Section_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @NextGen_Section_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @NextGen_Section_IsTeaching bit
	
	IF LEN(@NextGen_Section_GroupIds) > 0 
		SET @NextGen_Section_IsTeaching = 1
	ELSE
		SET @NextGen_Section_IsTeaching = 0
    ----------------------


    -- NEXT GEN GROUP
    --------------------
    DECLARE @NextGen_GroupTypeId int = 94
    DECLARE @NextGen_GroupIds varchar(MAX)
	
    -- Get all groups the person is teaching in
    DECLARE @NextGen_GroupIdTable table( id int )

	INSERT INTO @NextGen_GroupIdTable( id )
	SELECT GroupId
	FROM [dbo].GroupMember gm
		INNER JOIN [dbo].[Group] g ON g.Id = gm.GroupId
		INNER JOIN [dbo].[GroupTypeRole] gtr ON gtr.Id = gm.GroupRoleId
	WHERE 
        g.Id NOT IN (SELECT Id FROM [_church_ccv_ufnActions_GetTrainingGroupIds]( )) AND -- This group is not one of the training groups.
        g.GroupTypeId = @NextGen_GroupTypeId AND
		gm.PersonId = @PersonId AND 
		(gm.GroupMemberStatus & @GroupMemberReqMask) != 0 AND --Make sure the status matches the mask requirement
        gtr.IsLeader = 1 AND --Make sure their role in the group is Leader
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @NextGen_GroupIds = COALESCE(@NextGen_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @NextGen_GroupIdTable

    -- if there's at least one group, then the person's teaching
    DECLARE @NextGen_IsTeaching bit
	
	IF LEN(@NextGen_GroupIds) > 0 
		SET @NextGen_IsTeaching = 1
	ELSE
		SET @NextGen_IsTeaching = 0
    ----------------------


	--Put the results into a single row we'll return
	INSERT INTO @IsTeachingTable( 
        PersonId, 
        
        -- Neighborhood
        Neighborhood_SubSection_IsTeaching,
        Neighborhood_SubSection_GroupIds,

        Neighborhood_IsTeaching,
        Neighborhood_GroupIds,
        --

        -- Young Adult
        YoungAdult_Section_IsTeaching,
        YoungAdult_Section_GroupIds,

        YoungAdult_IsTeaching,
        YoungAdult_GroupIds,
        --

        -- Next Steps
        NextSteps_SubSection_IsTeaching,
        NextSteps_SubSection_GroupIds,

        NextSteps_IsTeaching,
        NextSteps_GroupIds,
        --


        -- Next Gen
        NextGen_Section_IsTeaching,
        NextGen_Section_GroupIds,

        NextGen_IsTeaching,
        NextGen_GroupIds
        --
    )
	SELECT 
		@PersonId, 
		
        -- Neighborhood
        @Neighborhood_SubSection_IsTeaching,
        @Neighborhood_SubSection_GroupIds,

        @Neighborhood_IsTeaching,
        @Neighborhood_GroupIds,
        --

        -- Young Adult
        @YoungAdult_Section_IsTeaching,
        @YoungAdult_Section_GroupIds,

        @YoungAdult_IsTeaching,
        @YoungAdult_GroupIds,
        --

        -- Next Steps
        @NextSteps_SubSection_IsTeaching,
        @NextSteps_SubSection_GroupIds,

        @NextSteps_IsTeaching,
        @NextSteps_GroupIds,
        --


        -- Next Gen
        @NextGen_Section_IsTeaching,
        @NextGen_Section_GroupIds,

        @NextGen_IsTeaching,
        @NextGen_GroupIds
        --

	RETURN;
END
