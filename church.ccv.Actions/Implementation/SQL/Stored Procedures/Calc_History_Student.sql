USE [RockRMS_20161013]
GO
/****** Object:  StoredProcedure [dbo].[_church_ccv_spActions_Calc_History_Student]    Script Date: 10/13/2016 3:12:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Will create a row in the _church_ccv_Actions_History_Student table that shows
-- the number of *ACTIVE MEMBER OR ATTENDEE STUDENTS* performing any of the available actions.
ALTER PROCEDURE [dbo].[_church_ccv_spActions_Calc_History_Student]
	@CampusId int,
	@RegionId int
AS
	BEGIN
        -- Students are defined as a "Child" in a family, between grades 7 and 12.
        DECLARE @StudentLowerGrade int = 7
        DECLARE @StudentUpperGrade int = 12
		
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

        -- Historical counts should use only Active Students that are Members & Attendees
        DECLARE @MemberConnectionStatusId int = 65
        DECLARE @AttendeeConnectionStatusId int = 146

        -- Using the Datamart, build a table of people. This implicitely gets only Active Students, as inactive aren't in the Datamart.
        DECLARE @PersonTable table(id int)
        INSERT INTO @PersonTable(id)
        SELECT DISTINCT p.PersonId
        FROM [dbo]._church_ccv_Datamart_Person p
        -- Students are: Child in family and between Lower & Upper grade.
        WHERE p.FamilyRole LIKE 'Child' AND (p.Grade >= @StudentLowerGrade AND p.Grade <= @StudentUpperGrade) AND 
              (p.ConnectionStatusValueId = @MemberConnectionStatusId OR p.ConnectionStatusValueId = @AttendeeConnectionStatusId) AND
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

        DECLARE @PeerLearningCount_NG int

        DECLARE @MentoredCount_NG int

        DECLARE @TeachingCount_Undefined int

        SELECT 
        @TotalPeople = COUNT( p.id )
        FROM @PersonTable p

        --Peer Learning
        SELECT 
        @PeerLearningCount = COUNT ( p.Id ),
        @PeerLearningCount_NG = COALESCE( SUM( CAST( G.NextGen_IsPeerLearning as int ) ), 0 )
        FROM @PersonTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Student_IsPeerLearning( p.Id, 1 ) G
        WHERE G.NextGen_IsPeerLearning = 1
        -----------------------------------------

        --Mentored
        SELECT
        @MentoredCount = COUNT ( p.Id ),
        @MentoredCount_NG = COALESCE( SUM( CAST( G.NextGen_IsMentored as int ) ), 0 )
        FROM @PersonTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Student_IsMentored( p.Id, 1 ) G
        WHERE G.NextGen_IsMentored = 1
        -----------------------------------------

        --Teaching
        SELECT 
        @TeachingCount = COUNT ( p.Id ),
        @TeachingCount_Undefined = COALESCE( SUM( CAST( T.Undefined_IsTeaching as int ) ), 0 )
        FROM @PersonTable P
        OUTER APPLY [dbo]._church_ccv_ufnActions_Student_IsTeaching( p.Id, 1 ) T
        WHERE T.Undefined_IsTeaching = 1
        -------------------------------

        --Serving
        SELECT @ServingCount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsServing( p.Id, 1 ) S
        WHERE S.IsServing = 1
        -------------------------------

        --Baptism
        SELECT @BaptismCount = COUNT(*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsBaptised( p.Id ) B
        WHERE B.IsBaptised = 1

        --ERA
        SELECT @ERACount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsERA( p.Id ) E
        WHERE E.IsEra = 1

        --GIVE
        SELECT @GiveCount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsGiving( p.Id ) G
        WHERE G.IsGiving = 1

        --Member
        SELECT @MembershipCount = COUNT (*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_IsMember( p.Id ) M
        WHERE M.IsMember = 1

        --StartingPoint
        SELECT @StartingPointCount = COUNT(*)
        FROM @PersonTable P
        CROSS APPLY [dbo]._church_ccv_ufnActions_Student_TakenStartingPoint( p.Id ) SP
        WHERE SP.TakenStartingPoint = 1
        
        INSERT INTO [dbo]._church_ccv_Actions_History_Student( 
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

            @PeerLearningCount_NG, 
    
            @MentoredCount_NG,

            @TeachingCount_Undefined,

            SYSDATETIME(),
            SYSDATETIME(),
            NULL,
            NULL,
            NEWID(),
            NULL,
            NULL,
            NULL

END
