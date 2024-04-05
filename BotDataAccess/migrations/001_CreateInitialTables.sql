CREATE TABLE Users (
    UserId NVARCHAR(20) PRIMARY KEY,
    UserType NVARCHAR(20), 
    UserName NVARCHAR(20),
    Banned BIT
);

CREATE TABLE UsageStats (
    Id NVARCHAR(20),
    StatType NVARCHAR(50),
    TimesUsed NUMERIC(10),
);

-- Permissions table ('Permissions' is a sql keyword) 
CREATE TABLE Entitlements (
    Id NVARCHAR(20), -- Could be userId, threadId, channelId, serverId
    Entitlement NVARCHAR(50),
    Granted DATETIME
);

CREATE TABLE Challenges (
    ChallengeId NVARCHAR(20) PRIMARY KEY, -- Same as thread id, all challenges are associated with a thread
    ServerId NVARCHAR(20), -- discord server id
    CreationDate DATETIME,
    LeetcodeName NVARCHAR(60),
    LeetcodeNumber NUMERIC(5),
    ChallengeDifficulty NVARCHAR(10),
);

CREATE TABLE ChallengeSubmissions (
    ChallengeId NVARCHAR(20),
    UserId NVARCHAR(20),
    BigO NVARCHAR(20),
    ProgrammingLanguage NVARCHAR(30),
    SubmissionDate DATETIME
);

CREATE TABLE ChallengeSubscribers (
    DiscordId NVARCHAR(20), -- The thread or server the user will be subscribed to
    UserId NVARCHAR(20),
    SubscribeDate DATETIME
);