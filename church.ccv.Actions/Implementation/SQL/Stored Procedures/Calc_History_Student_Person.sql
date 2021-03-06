--USE [RockRMS_20161003]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Will create a row in the _church_ccv_Actions_History_Student_Person table for each
-- ACTIVE STUDENT in the Person Datamart. It will show their state for each Action.
ALTER PROCEDURE [dbo].[_church_ccv_spActions_Calc_History_Student_Person]
AS
	BEGIN
        -- Students are defined as a "Child" in a family, between grades 7 and 12.
        DECLARE @StudentLowerGrade int = 7
        DECLARE @StudentUpperGrade int = 12

        --Note: For the general totals, they only want to count active group members.
        --SO:
        --0		Inactive
        --1		Active
        --2		Pending

        -- Using the Datamart, build a table of people. This implicitely gets only Active Students, as inactive aren't in the Datamart.
        -- Do this first so we have a UNIQUE list of IDs (since people can be in this table TWICE :\)
        DECLARE @PersonTable table(id int)
        INSERT INTO @PersonTable(id)
        SELECT DISTINCT p.PersonId
        FROM [dbo]._church_ccv_Datamart_Person p
        -- Students are: Child in family and between Lower & Upper grade.
        WHERE p.FamilyRole LIKE 'Child' AND (p.Grade >= @StudentLowerGrade AND p.Grade <= @StudentUpperGrade)

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
        INSERT INTO _church_ccv_Actions_History_Student_Person
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
            
            PeerLearning_NG, 
            
            Mentored_NG,
            
            Teaching_Undefined,
			
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

        -- as more group types are added, add bitwise OR them together
        (PL.NextGen_IsPeerLearning ) as bit, 
        (M.NextGen_IsMentored ) as bit,
        (T.Undefined_IsTeaching ) as bit,


         PL.NextGen_IsPeerLearning,

         M.NextGen_IsMentored,

         T.Undefined_IsTeaching,
		 
		 SYSDATETIME(),
 		 SYSDATETIME(), 
		 NULL,
		 NULL,
		 NEWID(),
		 NULL,
		 NULL,
		 NULL

        FROM @PersonAliasTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Student_IsPeerLearning( p.personId, 1 ) PL
        OUTER APPLY [dbo]._church_ccv_ufnActions_Student_IsMentored( p.personId, 1 ) M
        OUTER APPLY [dbo]._church_ccv_ufnActions_Student_IsTeaching( p.personId, 1 ) T
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsServing( p.personId, 1 ) S
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsBaptised( p.personId ) B
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsERA( p.personId ) E
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsGiving( p.personId ) G
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsMember( p.personId ) MB
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_TakenStartingPoint( p.personId ) SP

END
