USE [RockRMS_20161003]
GO
/****** Object:  StoredProcedure [dbo].[_church_ccv_spActions_Build_Full_History]    Script Date: 10/7/2016 11:57:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- This will generate two types of history.
-- 1. "Calc History" for adults and students. This gives us the total number of people performing each action per region per campus.
-- 2. "Calc History Person" for adults and students. This gives us the actual Action State for every active adult / student in the database.
-- This should be run once a week to get a snapshot of how many people are performing each CCV action.
ALTER PROCEDURE [dbo].[_church_ccv_spActions_Build_Full_History]
AS
	BEGIN
	    
        DECLARE @RegionGroupTypeGuid uniqueidentifier = 'C3A3EB51-53CA-4EC1-B9B4-BB62E0C61445'
        DECLARE @CampusId int

        -- For each ACTIVE campus
        DECLARE c0 CURSOR
        FOR
            SELECT Id
            FROM [dbo].Campus
            WHERE [dbo].Campus.IsActive = 1

            OPEN c0;

            FETCH NEXT
            FROM c0
            INTO @CampusId

            WHILE @@FETCH_STATUS = 0
            BEGIN

                    -- Load all the regions for the campus
                    DECLARE @CampusRegions table( GroupId int )
                    INSERT INTO @CampusRegions
                    (
                        GroupId
                    )
                    SELECT 
                        g.Id AS GroupId
                    FROM 
                        [dbo].[Group] g
                    INNER JOIN 
                        [dbo].[GroupType] gt ON gt.Id = g.GroupTypeId
                    WHERE 
                        gt.[Guid] = @RegionGroupTypeGuid AND g.CampusId = @CampusId
            
                    DECLARE @GroupId int

                    -- Now for each REGION in the Campus
                    DECLARE c1 CURSOR
                    FOR
                        SELECT GroupId
                        FROM @CampusRegions

                        OPEN c1;

                        FETCH NEXT 
                        FROM c1 
                        INTO @GroupId

                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            
                            -- Calculate a single row for Adult and a single row for Student (each row in a campus + region)
                            EXEC dbo._church_ccv_spActions_Calc_History_Adult @CampusId, @GroupId;
                            EXEC dbo._church_ccv_spActions_Calc_History_Student @CampusId, @GroupId;

                            FETCH NEXT 
                            FROM c1 
                            INTO @GroupId
                        END
                    CLOSE c1
                    DEALLOCATE c1

                    DELETE FROM @CampusRegions;

                    FETCH NEXT 
                    FROM c0 
                    INTO @CampusId
            END
        CLOSE c0
        DEALLOCATE c0


        -- Now, generate the Action State for every adult & student in the database.
        EXEC dbo._church_ccv_spActions_Calc_History_Adult_Person;
        EXEC dbo._church_ccv_spActions_Calc_History_Student_Person;

	END
