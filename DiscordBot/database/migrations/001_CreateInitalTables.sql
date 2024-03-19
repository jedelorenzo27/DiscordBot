CREATE TABLE Users (
    UserId NUMERIC(20) PRIMARY KEY,
    UserName NVARCHAR(20),
    Banned BIT
);

-- Permissions table ('Permissions' is a sql keyword) 
CREATE TABLE Entitlements (
    UserId NUMERIC(20),
    Entitlment NVARCHAR(20),
    Granted DATETIME
)

CREATE TABLE Challenges (
    ChannelId NUMERIC(20), -- Same as thread id, all challenges are associated with a thread
    ServerId NUMERIC(20), -- discord server id
    CreationDate DATETIME,
    LeetcodeId NUMERIC(5), 
    LeetcodeName NVARCHAR(20),
);

CREATE TABLE ChallengeSubmissions (
    ChannelId NUMERIC(20),
    UserId NUMERIC(20),
    SubmissionDate DATETIME
);