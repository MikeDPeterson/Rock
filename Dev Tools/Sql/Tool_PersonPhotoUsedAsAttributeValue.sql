/*
SELECT count(*), sum(datalength(bfd.Content))
  FROM [BinaryFile] bf
  join BinaryFileData bfd
  on bfd.Id = bf.Id
  where bf.BinaryFileTypeId = 5 and bf.Id not in (select PhotoId from Person where PhotoId is not null)
  order by bf.ModifiedDateTime desc
  */
--create table BinaryFilePhotoUsedAsAttributes ( Guid uniqueIdentifier) 
DECLARE binaryFileCursor CURSOR
FOR
SELECT cast([Guid] AS NVARCHAR(max))
FROM BinaryFile bf
WHERE bf.BinaryFileTypeId = 5
    AND bf.Id NOT IN (
        SELECT PhotoId
        FROM Person
        WHERE PhotoId IS NOT NULL
        )

DECLARE @binaryFileGuid NVARCHAR(max)

BEGIN
    OPEN binaryFileCursor

    FETCH NEXT
    FROM binaryFileCursor
    INTO @binaryFileGuid

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF EXISTS (
                SELECT *
                FROM AttributeValue
                WHERE Value = @binaryFileGuid
                )
            INSERT INTO BinaryFilePhotoUsedAsAttributes ([Guid])
            VALUES (@binaryFileGuid)

        FETCH NEXT
        FROM binaryFileCursor
        INTO @binaryFileGuid
    END

    CLOSE binaryFileCursor

    DEALLOCATE binaryFileCursor
END
