USE [RockRMS]
GO
/****** Object:  UserDefinedFunction [dbo].[_church_ccv_ufnActions_Adult_IsPeerLearning]    Script Date: 9/28/2016 3:56:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IsPeerLearning: Determines if the given person is in at least one group that counts as "Peer Learning".
--This means the group contains the person's "peers" whom they can learn from. Examples: Neighborhood Groups or Young Adult Groups.
--This is as opposed to a Next Step Group, where the person would be learning from a Coach/Mentor, not a peer.
ALTER FUNCTION [dbo].[_church_ccv_ufnActions_Adult_IsPeerLearning](@PersonId int, @ActiveMemberRequired bit)
RETURNS @IsPeerLearningTable TABLE
(
	PersonId int NOT NULL,
	
    Neighborhood_IsPeerLearning bit, --True if they are, false if they're not
	Neighborhood_GroupIds varchar(MAX) NULL,

    YoungAdult_IsPeerLearning bit,
    YoungAdult_GroupIds varchar(MAX) NULL
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



    -- YOUNG ADULT GROUP
    DECLARE @YoungAdult_Group_GroupTypeId int = 98
    DECLARE @YoungAdult_GroupIds varchar(MAX)
	
    -- Get all groups the person is peer learning in
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
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @YoungAdult_GroupIds = COALESCE(@YoungAdult_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @YoungAdult_GroupIdTable

    -- if there's at least one group, then the person's peer learning
    DECLARE @YoungAdult_IsPeerLearning bit
	
	IF LEN(@YoungAdult_GroupIds) > 0 
		SET @YoungAdult_IsPeerLearning = 1
	ELSE
		SET @YoungAdult_IsPeerLearning = 0
    ----------------------


    -- NEIGHBORHOOD
    DECLARE @Neighborhood_Group_GroupTypeId int = 49
    DECLARE @Neighborhood_GroupIds varchar(MAX)
	
    -- Get all groups the person is peer learning in
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
		g.IsActive = 1 --Make sure the GROUP IS active

    -- build a comma delimited string with the groups
	SELECT @Neighborhood_GroupIds = COALESCE(@Neighborhood_GroupIds + ', ', '' ) + CONVERT(nvarchar(MAX), id)
	FROM @Neighborhood_GroupIdTable

    -- if there's at least one group, then the person's peer learning
    DECLARE @Neighborhood_IsPeerLearning bit
	
	IF LEN(@Neighborhood_GroupIds) > 0 
		SET @Neighborhood_IsPeerLearning = 1
	ELSE
		SET @Neighborhood_IsPeerLearning = 0
    ----------------------

    

	--Put the results into a single row we'll return
	INSERT INTO @IsPeerLearningTable( 
        PersonId, 
        
        Neighborhood_IsPeerLearning, 
        Neighborhood_GroupIds, 
        
        YoungAdult_IsPeerLearning, 
        YoungAdult_GroupIds )
	SELECT 
		@PersonId, 
		
        @Neighborhood_IsPeerLearning, 
		@Neighborhood_GroupIds,

        @YoungAdult_IsPeerLearning,
        @YoungAdult_GroupIds

	RETURN;
END
