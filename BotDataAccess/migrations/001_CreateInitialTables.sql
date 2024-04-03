CREATE TABLE Users (
    UserId NVARCHAR(20) PRIMARY KEY,
    UserType NVARCHAR(20), 
    UserName NVARCHAR(20),
    Banned BIT
);

-- Permissions table ('Permissions' is a sql keyword) 
CREATE TABLE Entitlements (
    Id NVARCHAR(20),
    Entitlement NVARCHAR(20),
    Granted DATETIME
);

CREATE TABLE Challenges (
    ChannelId NVARCHAR(20), -- Same as thread id, all challenges are associated with a thread
    ServerId NVARCHAR(20), -- discord server id
    CreationDate DATETIME,
    LeetcodeNumber NUMERIC(5), 
    LeetcodeName NVARCHAR(20)
);

CREATE TABLE ChallengeSubmissions (
    ChannelId NVARCHAR(20),
    UserId NVARCHAR(20),
    SubmissionDate DATETIME
);

CREATE TABLE ChallengeSubscribers (
    ChannelId NVARCHAR(20), -- Could be Discord Server Id or Thread Id associated with a specific challenge
    UserId NVARCHAR(20),
    SubscribeDate DATETIME
);