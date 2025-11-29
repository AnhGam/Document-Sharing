USE [quan_ly_tai_lieu]
GO

-- 1. Feature 6 & 9: Add Tags and Deadline to tai_lieu table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tai_lieu]') AND name = 'tags')
BEGIN
    ALTER TABLE [dbo].[tai_lieu] ADD [tags] NVARCHAR(500) NULL;
    PRINT 'Added tags column to tai_lieu table.';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tai_lieu]') AND name = 'deadline')
BEGIN
    ALTER TABLE [dbo].[tai_lieu] ADD [deadline] DATETIME NULL;
    PRINT 'Added deadline column to tai_lieu table.';
END
GO

-- 2. Feature 7: Personal Notes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[personal_notes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[personal_notes](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [user_id] [int] NOT NULL,
        [document_id] [int] NOT NULL,
        [note_content] [nvarchar](MAX) NULL,
        [status] [nvarchar](50) NULL DEFAULT 'Chua doc', -- 'Chua doc', 'Dang hoc', 'Da on xong'
        [updated_at] [datetime] DEFAULT GETDATE(),
        PRIMARY KEY CLUSTERED ([id] ASC),
        FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id]) ON DELETE CASCADE,
        FOREIGN KEY([document_id]) REFERENCES [dbo].[tai_lieu] ([id]) ON DELETE CASCADE
    );
    PRINT 'Created personal_notes table.';
END
GO

-- 3. Feature 8: Collections
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[collections]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[collections](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [user_id] [int] NOT NULL,
        [name] [nvarchar](100) NOT NULL,
        [description] [nvarchar](500) NULL,
        [created_at] [datetime] DEFAULT GETDATE(),
        PRIMARY KEY CLUSTERED ([id] ASC),
        FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id]) ON DELETE CASCADE
    );
    PRINT 'Created collections table.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[collection_items]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[collection_items](
        [collection_id] [int] NOT NULL,
        [document_id] [int] NOT NULL,
        [added_at] [datetime] DEFAULT GETDATE(),
        PRIMARY KEY CLUSTERED ([collection_id] ASC, [document_id] ASC),
        FOREIGN KEY([collection_id]) REFERENCES [dbo].[collections] ([id]) ON DELETE CASCADE,
        FOREIGN KEY([document_id]) REFERENCES [dbo].[tai_lieu] ([id]) ON DELETE CASCADE
    );
    PRINT 'Created collection_items table.';
END
GO