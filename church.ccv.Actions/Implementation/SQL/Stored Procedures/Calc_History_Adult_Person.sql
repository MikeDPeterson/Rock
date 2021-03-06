USE --[RockRMS_20161003]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Will create a row in the _church_ccv_Actions_History_Adult_Person table for each
-- ACTIVE ADULT in the Person Datamart. It will show their state for each Action.
ALTER PROCEDURE [dbo].[_church_ccv_spActions_Calc_History_Adult_Person]
AS
	BEGIN
        --Note: For the general totals, they only want to count active group members.
        --SO:
        --0		Inactive
        --1		Active
        --2		Pending

        -- Using the Datamart, build a table of people. This implicitely gets only Active Adults, as inactive aren't in the Datamart.
        -- Do this first so we have a UNIQUE list of IDs (since people can be in this table TWICE :\)
        DECLARE @PersonTable table(id int)
        INSERT INTO @PersonTable(id)
        SELECT DISTINCT p.PersonId
        FROM [dbo]._church_ccv_Datamart_Person p
        WHERE p.FamilyRole LIKE 'Adult'

        -- now get each person's alias id. history must store an alias so that if the person is merged, we have
        -- a pointer to them.
        DECLARE @PersonAliasTable table(personId int, aliasId int)

        -- guard against database errors (orphaned ids) by excluding persons with NULL alias ids.
        INSERT INTO @PersonAliasTable(personId, aliasId)
        SELECT pa.personId, pa.aliasId
        FROM
        (SELECT dbo.[ufnUtility_GetPrimaryPersonAliasId]( p.id ) AS aliasId, p.id AS personId
        FROM @PersonTable p) pa
        WHERE pa.aliasId IS NOT NULL


        --Peer Learning
        INSERT INTO _church_ccv_Actions_History_Adult_Person
        (
            [Date],
            [PersonAliasId],
            
            Baptised,
            ERA, 
            Give, 
            Member, 
            StartingPoint,
            Serving, 
            PeerLearning,
            Mentored,
            Teaching,
            
            PeerLearning_NH, 
            PeerLearning_YA,
            
            Mentored_NH,
            Mentored_YA,
            Mentored_NS,
            
            Teaching_NH_SubSection,
            Teaching_NH,
            Teaching_YA_Section,
            Teaching_YA,
            Teaching_NS_SubSection,
            Teaching_NS,
            Teaching_NG_Section,
            Teaching_NG,

            [CreatedDateTime],
            [ModifiedDateTime],
            [CreatedByPersonAliasId],
            [ModifiedByPersonAliasId],
            [Guid],
            [ForeignId],
            [ForeignGuid],
            [ForeignKey]
        )
        SELECT
        CONVERT( date, SYSDATETIME() ), 
        p.aliasId,
        B.BaptisedDate,
        E.IsERA,
        G.IsGiving,
        MB.MembershipDate,
        SP.StartingPointDate,
        S.IsServing,
        (PL.Neighborhood_IsPeerLearning | PL.YoungAdult_IsPeerLearning) as bit,
        
        (M.Neighborhood_IsMentored | M.NextSteps_IsMentored | M.YoungAdult_IsMentored) as bit,

        (T.Neighborhood_SubSection_IsTeaching | T.Neighborhood_IsTeaching |
         T.YoungAdult_Section_IsTeaching | T.YoungAdult_IsTeaching |
         T.NextSteps_SubSection_IsTeaching | T.NextSteps_IsTeaching |
         T.NextGen_Section_IsTeaching | T.NextGen_IsTeaching) as bit,

         PL.Neighborhood_IsPeerLearning,
         PL.YoungAdult_IsPeerLearning,

         M.Neighborhood_IsMentored,
         M.YoungAdult_IsMentored,
         M.NextSteps_IsMentored,

         T.Neighborhood_SubSection_IsTeaching,
         T.Neighborhood_IsTeaching,

         T.YoungAdult_Section_IsTeaching,
         T.YoungAdult_IsTeaching,

         T.NextSteps_SubSection_IsTeaching,
         T.NextSteps_IsTeaching,

         T.NextGen_Section_IsTeaching,
         T.NextGen_IsTeaching,
		 
		SYSDATETIME(),
		SYSDATETIME(),
		NULL,
		NULL,
		NEWID(),
		NULL,
		NULL,
		NULL

        FROM @PersonAliasTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Adult_IsPeerLearning( p.personId, 1 ) PL
        OUTER APPLY [dbo]._church_ccv_ufnActions_Adult_IsMentored( p.personId, 1 ) M
        OUTER APPLY [dbo]._church_ccv_ufnActions_Adult_IsTeaching( p.personId, 1 ) T
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsServing( p.personId, 1 ) S
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsBaptised( p.personId ) B
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsERA( p.personId ) E
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsGiving( p.personId ) G
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsMember( p.personId ) MB
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_TakenStartingPoint( p.personId )SP

END
