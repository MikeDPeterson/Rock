USE [RockRMS_20161013]
GO
/****** Object:  StoredProcedure [dbo].[_church_ccv_spActions_Calc_History_Adult]    Script Date: 10/13/2016 3:07:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Will create a row in the _church_ccv_Actions_History_Adult table that shows
-- the number of *ACTIVE MEMBER OR ATTENDEE ADULTS* performing any of the available actions.
ALTER PROCEDURE [dbo].[_church_ccv_spActions_Calc_History_Adult]
	@CampusId int,
	@RegionId int
AS
	BEGIN
		
        --Note: For general totals, they only want to count members and attendees.
        --SO: 
        --65	Member
        --66	Visitor
        --67	Web Prospect
        --146	Attendee

        --Note: For the general totals, they only want to count active group members.
        --SO:
        --0		Inactive
        --1		Active
        --2		Pending

        -- Historical counts should use only Active Adults that are Members & Attendees
        DECLARE @MemberConnectionStatusId int = 65
        DECLARE @AttendeeConnectionStatusId int = 146

        -- Using the Datamart, build a table of people. This implicitely gets only Active Adults, as inactive aren't in the Datamart.
        DECLARE @PersonTable table(id int)
        INSERT INTO @PersonTable(id)
        SELECT DISTINCT p.PersonId
        FROM [dbo]._church_ccv_Datamart_Person p
        WHERE p.FamilyRole LIKE 'Adult' AND (p.ConnectionStatusValueId = @MemberConnectionStatusId OR p.ConnectionStatusValueId = @AttendeeConnectionStatusId) AND
        (p.CampusId = @CampusId OR @CampusId = -1) AND (p.NeighborhoodId = @RegionId OR @RegionId = -1)

        DECLARE @TotalPeople int

        DECLARE @BaptismCount int
        DECLARE @ERACount int
        DECLARE @GiveCount int
        DECLARE @MembershipCount int
        DECLARE @ServingCount int
        DECLARE @StartingPointCount int

        DECLARE @PeerLearningCount int
        DECLARE @MentoredCount int
        DECLARE @TeachingCount int

        DECLARE @PeerLearningCount_NH int
        DECLARE @PeerLearningCount_YA int

        DECLARE @MentoredCount_NH int
        DECLARE @MentoredCount_YA int
        DECLARE @MentoredCount_NS int

        DECLARE @TeachingCount_NH_SubSection int
        DECLARE @TeachingCount_NH int

        DECLARE @TeachingCount_YA_Section int
        DECLARE @TeachingCount_YA int

        DECLARE @TeachingCount_NS_SubSection int
        DECLARE @TeachingCount_NS int

        DECLARE @TeachingCount_NG_Section int
        DECLARE @TeachingCount_NG int

        SELECT 
        @TotalPeople = COUNT( p.id )
        FROM @PersonTable p

        --Peer Learning
        SELECT 
        @PeerLearningCount = COUNT ( p.Id ),
        @PeerLearningCount_NH = COALESCE( SUM( CAST( G.Neighborhood_IsPeerLearning as int ) ), 0 ),
        @PeerLearningCount_YA = COALESCE( SUM( CAST( G.YoungAdult_IsPeerLearning as int ) ), 0 )
        FROM @PersonTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Adult_IsPeerLearning( p.Id, 1 ) G
        WHERE G.YoungAdult_IsPeerLearning = 1 OR G.Neighborhood_IsPeerLearning = 1
        -----------------------------------------

        --Mentored
        SELECT 
        @MentoredCount = COUNT ( p.Id ),
        @MentoredCount_YA = COALESCE( SUM ( CAST( G.YoungAdult_IsMentored as int ) ), 0 ),
        @MentoredCount_NH = COALESCE( SUM ( CAST( G.Neighborhood_IsMentored as int ) ), 0 ),
        @MentoredCount_NS = COALESCE( SUM ( CAST( G.NextSteps_IsMentored as int ) ), 0 )
        FROM @PersonTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Adult_IsMentored( p.Id, 1 ) G
        WHERE G.YoungAdult_IsMentored = 1 OR G.Neighborhood_IsMentored = 1 OR G.NextSteps_IsMentored = 1
        -----------------------------------------

        --Teaching
        SELECT 
        @TeachingCount = COUNT ( p.Id ),
        @TeachingCount_NH_SubSection = COALESCE( SUM( CAST( T.Neighborhood_SubSection_IsTeaching as int ) ), 0 ),
        @TeachingCount_NH = COALESCE( SUM( CAST( T.Neighborhood_IsTeaching as int ) ), 0 ),
        @TeachingCount_YA_Section = COALESCE( SUM( CAST( T.YoungAdult_Section_IsTeaching as int ) ), 0 ),
        @TeachingCount_YA = COALESCE( SUM( CAST( T.YoungAdult_IsTeaching as int ) ), 0 ),
        @TeachingCount_NS_SubSection = COALESCE( SUM( CAST( T.NextSteps_SubSection_IsTeaching as int ) ), 0 ),
        @TeachingCount_NS = COALESCE( SUM( CAST( T.NextSteps_IsTeaching as int ) ), 0 ),
        @TeachingCount_NG_Section = COALESCE( SUM( CAST( T.NextGen_Section_IsTeaching as int ) ), 0 ),
        @TeachingCount_NG = COALESCE( SUM( CAST( T.NextGen_IsTeaching as int ) ), 0 )
        FROM @PersonTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Adult_IsTeaching( p.Id, 1 ) T
        WHERE 
        T.Neighborhood_SubSection_IsTeaching = 1 OR
        T.Neighborhood_IsTeaching = 1 OR
        T.YoungAdult_Section_IsTeaching = 1 OR
        T.YoungAdult_IsTeaching = 1 OR
        T.NextSteps_SubSection_IsTeaching = 1 OR
        T.NextSteps_IsTeaching = 1 OR
        T.NextGen_Section_IsTeaching = 1 OR
        T.NextGen_IsTeaching = 1
        -------------------------------

        --Serving
        SELECT @ServingCount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsServing( p.Id, 1 ) S
        WHERE S.IsServing = 1
        -------------------------------

        --Baptism
        SELECT @BaptismCount = COUNT(*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsBaptised( p.Id ) B
        WHERE B.IsBaptised = 1

        --ERA
        SELECT @ERACount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsERA( p.Id ) E
        WHERE E.IsEra = 1

        --GIVE
        SELECT @GiveCount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsGiving( p.Id ) G
        WHERE G.IsGiving = 1

        --Member
        SELECT @MembershipCount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_IsMember( p.Id ) M
        WHERE M.IsMember = 1

        --StartingPoint
        SELECT @StartingPointCount = COUNT(*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Adult_TakenStartingPoint( p.Id ) SP
        WHERE SP.TakenStartingPoint = 1
        
        INSERT INTO [dbo]._church_ccv_Actions_History_Adult( 
            [Date],

            CampusId,
            RegionId,
            TotalPeople,

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

            @CampusId,
            @RegionId,
            @TotalPeople,

            @BaptismCount,  
            @ERACount, 
            @GiveCount, 
            @MembershipCount, 
            @StartingPointCount,
            @ServingCount, 
            @PeerLearningCount,
            @MentoredCount,
            @TeachingCount,

            @PeerLearningCount_NH, 
            @PeerLearningCount_YA, 
    
            @MentoredCount_NH,
            @MentoredCount_YA,
            @MentoredCount_NS,

            @TeachingCount_NH_SubSection,
            @TeachingCount_NH,
            @TeachingCount_YA_Section,
            @TeachingCount_YA,
            @TeachingCount_NS_SubSection,
            @TeachingCount_NS,
            @TeachingCount_NG_Section,
            @TeachingCount_NG,

            SYSDATETIME(),
            SYSDATETIME(),
            NULL,
            NULL,
            NEWID(),
            NULL,
            NULL,
            NULL
END
