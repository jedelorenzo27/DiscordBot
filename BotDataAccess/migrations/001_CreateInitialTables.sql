CREATE TABLE Users (
    UserId NVARCHAR(20) PRIMARY KEY,
    UserType NVARCHAR(20), 
    UserName NVARCHAR(20),
    Banned BIT
);

-- Permissions table ('Permissions' is a sql keyword) 
CREATE TABLE Entitlements (
    Id NVARCHAR(20),
    Entitlement NVARCHAR(50),
    Granted DATETIME
);

CREATE TABLE Challenges (
    ChallengeId NVARCHAR(20) PRIMARY KEY, -- Same as thread id, all challenges are associated with a thread
    ChannelId NVARCHAR(20), -- discord server id
    ServerId NVARCHAR(20), -- discord server id
    CreationDate DATETIME,
    LeetcodeName NVARCHAR(60),
    LeetcodeNumber NUMERIC(5),
    ChallengeDifficulty NVARCHAR(10),
);

CREATE TABLE ChallengeSubmissions (
    ChallengeId NVARCHAR(20),
    UserId NVARCHAR(20),
    SubmissionDate DATETIME
);

CREATE TABLE ChallengeSubscribers (
    ChannelId NVARCHAR(20), -- The channel where challenges are posted
    UserId NVARCHAR(20),
    SubscribeDate DATETIME
);