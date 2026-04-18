-- ==========================================================================================
-- Table: [Books]
-- Purpose: Store reading list entries
-- Genre and Status stored as NVARCHAR (not normalized) for demo scope.
-- In production: normalize into lookup tables with FK constraints.
-- 
-- Index on Title to support Equals exact-match search.
-- Contains search (LIKE '%keyword%') cannot use this index but Equals search benefits from it directly.
-- ==========================================================================================
CREATE TABLE [dbo].[Books] (
    [Id]               INT             IDENTITY(1,1)  NOT NULL,
    [Title]            NVARCHAR(255)   NOT NULL,
    [Author]           NVARCHAR(255)   NOT NULL,
    [Genre]            NVARCHAR(100)   NOT NULL,
    [Notes]            NVARCHAR(1000)  NULL,
    [Status]           NVARCHAR(50)    NOT NULL CONSTRAINT [DF_Books_Status] DEFAULT ('Want to Read'),
    [CreatedDateTime]  DATETIME2(7)    NOT NULL CONSTRAINT [DF_Books_CreatedDateTime] DEFAULT (GETUTCDATE()),
    [ModifiedDateTime] DATETIME2(7)    NOT NULL CONSTRAINT [DF_Books_ModifiedDateTime] DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Books] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_Books_Title]
    ON [dbo].[Books] ([Title] ASC);
GO

INSERT INTO [dbo].[Books] 
    ([Title], [Author], [Genre], [Notes], [Status])
VALUES
    ('The Pragmatic Programmer',      'David Thomas',        'Technology',  'Great reference for clean coding practices',  'Completed'),
    ('Clean Code',                    'Robert C. Martin',    'Technology',  'Focus on readable and maintainable code',     'Completed'),
    ('Designing Data-Intensive Apps', 'Martin Kleppmann',    'Technology',  'Deep dive into distributed systems',          'Reading'),
    ('The Phoenix Project',           'Gene Kim',            'Technology',  'DevOps principles through a fictional story', 'Reading'),
    ('Domain-Driven Design',          'Eric Evans',          'Technology',  'Dense but foundational for system design',    'Want to Read'),
    ('Dune',                          'Frank Herbert',       'Sci-Fi',      'Classic world building',                      'Completed'),
    ('Project Hail Mary',             'Andy Weir',           'Sci-Fi',      'Loved The Martian, excited for this one',     'Reading'),
    ('Enders Game',                   'Orson Scott Card',    'Sci-Fi',      NULL,                                          'Want to Read'),
    ('Sapiens',                       'Yuval Noah Harari',   'Non-Fiction', 'Broad history of humankind',                  'Completed'),
    ('Atomic Habits',                 'James Clear',         'Non-Fiction', 'Practical and immediately actionable',        'Completed'),
    ('Thinking Fast and Slow',        'Daniel Kahneman',     'Non-Fiction', 'Dense but rewarding',                        'Want to Read'),
    ('The Name of the Wind',          'Patrick Rothfuss',    'Fantasy',     'Beautiful prose, waiting for book 3 forever','Completed'),
    ('The Way of Kings',              'Brandon Sanderson',   'Fantasy',     'First of the Stormlight Archive series',      'Reading'),
    ('Mistborn',                      'Brandon Sanderson',   'Fantasy',     NULL,                                          'Want to Read'),
    ('The Great Gatsby',              'F. Scott Fitzgerald', 'Classic',     'Reread for the third time',                   'Completed');
GO
