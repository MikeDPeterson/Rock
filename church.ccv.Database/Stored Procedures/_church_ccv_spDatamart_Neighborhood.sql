/*
<doc>
	<summary>
 		This stored procedure builds the data mart table _church_ccv_spDatamart_Neighborhood
	</summary>
	
	<remarks>	
	</remarks>
	<code>
		EXEC [dbo].[_church_ccv_spDatamart_Neighborhood]
	</code>
</doc>
*/
ALTER PROCEDURE [dbo].[_church_ccv_spDatamart_Neighborhood]
AS
BEGIN
    DECLARE @NeighborhoodGroupTypeGuid UNIQUEIDENTIFIER = 'C3A3EB51-53CA-4EC1-B9B4-BB62E0C61445'
    DECLARE @NeighborhoodPastorRoleId INT = 45
    DECLARE @NeighborhoodLeaderRoleId INT = 46

    SET NOCOUNT ON;

    TRUNCATE TABLE _church_ccv_Datamart_Neighborhood

    INSERT INTO _church_ccv_Datamart_Neighborhood (
        [NeighborhoodId]
        ,[NeighborhoodName]
        ,[Guid]
        ,[GroupCount]
        ,[NeighborhoodLeaderName]
        ,[NeighborhoodLeaderId]
        ,[NeighborhoodPastorName]
        ,[NeighborhoodPastorId]
        ,[HouseholdCount]
        ,[AdultCount]
        ,[AdultsInGroups]
        ,[AdultsBaptized]
        ,[AdultMemberCount]
        ,[AdultMembersInGroups]
        ,[AdultAttendeeCount]
        ,[AdultAttendeesInGroups]
        ,[AdultVisitors]
        ,[AdultParticipants]
        ,[ChildrenCount]
        )
    SELECT g.[Id]
        ,g.[Name]
        ,g.[Guid]
        ,(
            SELECT COUNT(*)
            FROM [Group] gc
            WHERE gc.[ParentGroupId] = g.[Id]
            )
        ,(
            SELECT TOP 1 p.[NickName] + ' ' + p.[LastName]
            FROM [Person] p
            INNER JOIN [GroupMember] gm ON gm.[PersonId] = p.[Id]
            INNER JOIN [Group] lg ON lg.[Id] = gm.[GroupId]
            WHERE lg.[Id] = g.[Id]
                AND gm.[GroupRoleId] = @NeighborhoodLeaderRoleId
            )
        ,(
            SELECT TOP 1 p.[Id]
            FROM [Person] p
            INNER JOIN [GroupMember] gm ON gm.[PersonId] = p.[Id]
            INNER JOIN [Group] lg ON lg.[Id] = gm.[GroupId]
            WHERE lg.[Id] = g.[Id]
                AND gm.[GroupRoleId] = @NeighborhoodLeaderRoleId
            )
        ,(
            SELECT TOP 1 p.[NickName] + ' ' + p.[LastName]
            FROM [Person] p
            INNER JOIN [GroupMember] gm ON gm.[PersonId] = p.[Id]
            INNER JOIN [Group] lg ON lg.[Id] = gm.[GroupId]
            WHERE lg.[Id] = g.[Id]
                AND gm.[GroupRoleId] = @NeighborhoodPastorRoleId
            )
        ,(
            SELECT TOP 1 p.[Id]
            FROM [Person] p
            INNER JOIN [GroupMember] gm ON gm.[PersonId] = p.[Id]
            INNER JOIN [Group] lg ON lg.[Id] = gm.[GroupId]
            WHERE lg.[Id] = g.[Id]
                AND gm.[GroupRoleId] = @NeighborhoodPastorRoleId
            )
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Family] df
                WHERE df.[NeighborhoodId] = g.[Id]
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [InNeighborhoodGroup] = 1
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [IsBaptized] = 1
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [ConnectionStatus] = 'Member'
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [ConnectionStatus] = 'Member'
                    AND [InNeighborhoodGroup] = 1
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [ConnectionStatus] = 'Attendee'
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [ConnectionStatus] = 'Atendee'
                    AND [InNeighborhoodGroup] = 1
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [ConnectionStatus] = 'Visitor'
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Adult'
                    AND [ConnectionStatus] = 'Participant'
                ), 0)
        ,COALESCE((
                SELECT COUNT(*)
                FROM [_church_ccv_Datamart_Person] dp
                WHERE dp.[NeighborhoodId] = g.[Id]
                    AND [FamilyRole] = 'Child'
                ), 0)
    FROM [Group] g
    INNER JOIN [GroupType] gt ON g.GroupTypeId = gt.[Id]
    WHERE gt.[Guid] = @NeighborhoodGroupTypeGuid
        AND g.[Id] <> 1201281 -- top-level parent group of neigborhood areas

    -- calculate percentages
    UPDATE _church_ccv_Datamart_Neighborhood
    SET [AdultsInGroupsPercentage] = CASE 
            WHEN [AdultCount] > 0
                THEN CAST(((CAST([AdultsInGroups] AS DECIMAL(9, 2)) / [AdultCount]) * 100) AS DECIMAL(9, 2))
            ELSE 0
            END
        ,[AdultsBaptizedPercentage] = CASE 
            WHEN [AdultCount] > 0
                THEN CAST(((CAST([AdultsBaptized] AS DECIMAL(9, 2)) / [AdultCount]) * 100) AS DECIMAL(9, 2))
            ELSE 0
            END
        ,[AdultMembersInGroupsPercentage] = CASE 
            WHEN [AdultMemberCount] > 0
                THEN CAST(((CAST([AdultMembersInGroups] AS DECIMAL(9, 2)) / [AdultMemberCount]) * 100) AS DECIMAL(9, 2))
            ELSE 0
            END
        ,[AdultAttendeesInGroupsPercentage] = CASE 
            WHEN [AdultAttendeeCount] > 0
                THEN CAST(((CAST([AdultAttendeesInGroups] AS DECIMAL(9, 2)) / [AdultAttendeeCount]) * 100) AS DECIMAL(9, 2))
            ELSE 0
            END
END
