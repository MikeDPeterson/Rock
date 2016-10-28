if (db_name() = 'RockRMS') begin
  throw  51000, 'This should only be run on non-production databases', 1;
end

/* Recreate RockReadOnly and RockUser so they point to the current server's Logins */
DROP USER [RockReadOnly]
GO
CREATE USER [RockReadOnly] FOR LOGIN [RockReadOnly]
GO
ALTER ROLE [db_datareader] ADD MEMBER [RockReadOnly]
GO

DROP USER [RockUser]
GO
CREATE USER [RockUser] FOR LOGIN [RockUser]
GO
ALTER ROLE [db_owner] ADD MEMBER [RockUser]
GO

-- set InternalApplicationRoot, PublicApplicationRoot, and OrganizationWebsite to ccvdev
update AttributeValue set [Value] = 'http://ccvdev.ccv.church/' where AttributeId = (select top 1 Id from  Attribute where [Key] = 'InternalApplicationRoot' and EntityTypeId is null)
update AttributeValue set [Value] = 'http://ccvdev.ccv.church/' where AttributeId = (select top 1 Id from  Attribute where [Key] = 'PublicApplicationRoot' and EntityTypeId is null)
update AttributeValue set [Value] = 'http://ccvdev.ccv.church' where AttributeId = (select top 1 Id from  Attribute where [Key] = 'OrganizationWebsite' and EntityTypeId is null)


-- UPDATE JOBS SO THEY ARE INACTIVE
UPDATE [ServiceJob] set [isActive] = 0

-- disable RequiresEncryption for the login page (production has it enabled)
UPDATE [Page] set RequiresEncryption = 0 where Id = 3

UPDATE [Page] set RequiresEncryption = 0 where RequiresEncryption = 1

-- DELETE AUTH EDITS FOR DELETE
UPDATE [Auth]
  SET [AllowOrDeny] = 'A' 
  WHERE [Guid] IN ('32937D01-0F34-41A0-94F0-9351D8A43051', '6A47DA72-EEF4-44D4-B8A7-E371ABF7F387', '950C674A-8685-4FD3-9A15-4BA0B84C3745', '7EE72B4F-980C-449C-A65A-CBD87C059F28')

  DELETE FROM [Block] WHERE [Guid] IN ('3B69EA68-68A7-4C93-86C6-C487F5E4FF03','AE3737D3-4CCE-4ECD-AC2C-FA8FD929F487')

--
-- BLANK OUT EMAILS
UPDATE [Person]
set [Email] = LOWER(dbo.[ufnUtility_RemoveNonAlphaCharacters]([NickName])) + LOWER(dbo.[ufnUtility_RemoveNonAlphaCharacters]([LastName])) + '@safety.netz'
WHERE [Email] IS NOT NULL

-- Fake Phonenumbers
update PhoneNumber set Number = '623555' +substring(Number, len(number)-4, 4), NumberFormatted = '(623) 555-' + SUBSTRING(number, len(number)-4, 4)

-- Update the Mail Medium/Transport settings to use SMTP with Localhost/25
DECLARE @SMTPEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Communication.Transport.SMTP' )
DECLARE @MailEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Communication.Medium.Email' )
DECLARE @MailAttributeId int

-- SMTP Server
SET @MailAttributeId = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [EntityTypeId] = @SMTPEntityTypeId AND [Key] = 'Server' )
UPDATE [AttributeValue] SET [Value] = 'localhost' WHERE [AttributeId] = @MailAttributeId

-- set all mail transport's server to localhost
update AttributeValue set Value = 'localhost' where AttributeId in (
SELECT Id FROM [Attribute] WHERE [EntityTypeId] in (
SELECT Id FROM [EntityType] WHERE [Name] like '%.Communication.Transport.%'
) and [Key] like 'Server'
)

-- SMTP Port
SET @MailAttributeId = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [EntityTypeId] = @SMTPEntityTypeId AND [Key] = 'Port' )
UPDATE [AttributeValue] SET [Value] = '25' WHERE [AttributeId] = @MailAttributeId

-- SMTP Username
SET @MailAttributeId = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [EntityTypeId] = @SMTPEntityTypeId AND [Key] = 'UserName' )
UPDATE [AttributeValue] SET [Value] = '' WHERE [AttributeId] = @MailAttributeId

-- SMTP Password
SET @MailAttributeId = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [EntityTypeId] = @SMTPEntityTypeId AND [Key] = 'Password' )
UPDATE [AttributeValue] SET [Value] = '' WHERE [AttributeId] = @MailAttributeId

-- SMTP UseSSL
SET @MailAttributeId = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [EntityTypeId] = @SMTPEntityTypeId AND [Key] = 'UseSSL' )
UPDATE [AttributeValue] SET [Value] = 'False' WHERE [AttributeId] = @MailAttributeId

-- Mail Transport
DECLARE @SMTPEntityTypeGuid varchar(50) = ( SELECT LOWER(CAST([Guid] as varchar(50))) FROM [EntityType] WHERE [Id] = @SMTPEntityTypeId )
SET @MailAttributeId = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [EntityTypeId] = @MailEntityTypeId AND [Key] = 'TransportContainer' )
UPDATE [AttributeValue] SET [Value] = @SMTPEntityTypeGuid WHERE [AttributeId] = @MailAttributeId

--
-- Give rights to groups types
-- Event Group Type
INSERT INTO [Auth]
	([EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid])
VALUES
	(91, 55, 1, 'Edit', 'A', 0, 33, newid())

INSERT INTO [Auth]
	([EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid])
VALUES
	(91, 55, 0, 'Edit', 'A', 0, 3, newid())

-- Ministry Group
INSERT INTO [Auth]
	([EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid])
VALUES
	(91, 56, 1, 'Edit', 'A', 0, 33, newid())

INSERT INTO [Auth]
	([EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid])
VALUES
	(91, 56, 0, 'Edit', 'A', 0, 3, newid())

-- Serving Group
INSERT INTO [Auth]
	([EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid])
VALUES
	(91, 23, 1, 'Edit', 'A', 0, 33, newid())

INSERT INTO [Auth]
	([EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid])
VALUES
	(91, 23, 0, 'Edit', 'A', 0, 3, newid())

 

