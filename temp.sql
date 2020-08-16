use Gateway;

delete from UserEmails;
delete from AspNetUsers;

insert into AspNetUsers 
(
    AccessFailedCount,
    LockoutEnabled,
    TwoFactorEnabled,
    UserName,
    NormalizedUserName,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    AccountCreateTime,
    Id,
    NickName,
    PreferedLanguage,
    Email,
    IconFilePath
)
VALUES
(
    0,
    1,
    0,
    'admin@anduinxue.com',
    'ADMIN@ANDUINXUE.COM',
    'AQAAAAEAACcQAAAAEHswFfciW8m5p2De3KzBd7ha0OX04Dav4jRy3CL/M/L54nWtMOGLwDZF1qo211KVRg==',
    '3BO4UKYBY2GJVGNTBM756HO6NQLUAKGQ',
    '89f42d2b-1af9-416a-8953-5abb02b523a1',
    '2020-01-01 12:34:56',
    '$(userId)',
    'Admin',
    'en-US',
    'admin@anduinxue.com',
    'usericon/default.png'
);

insert into UserEmails
(
    EmailAddress,
    OwnerId,
    Priority,
    Validated,
    LastSendTime
)
VALUES
(
    'admin@anduinxue.com',
    '$(userId)',
    1,
    0,
    '2020-01-01 12:34:56'
)

use Developer;

delete from AspNetUsers;

insert into AspNetUsers 
(
    AccessFailedCount,
    LockoutEnabled,
    TwoFactorEnabled,
    UserName,
    NormalizedUserName,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    AccountCreateTime,
    Id,
    NickName,
    PreferedLanguage,
    Email,
    EmailConfirmed,
    IconFilePath
)
VALUES
(
    0,
    1,
    0,
    'admin@anduinxue.com',
    'ADMIN@ANDUINXUE.COM',
    'AQAAAAEAACcQAAAAEHswFfciW8m5p2De3KzBd7ha0OX04Dav4jRy3CL/M/L54nWtMOGLwDZF1qo211KVRg==',
    '3BO4UKYBY2GJVGNTBM756HO6NQLUAKGQ',
    '89f42d2b-1af9-416a-8953-5abb02b523a1',
    '2020-01-01 12:34:56',
    '{{userId}}',
    'Admin',
    'en-US',
    'admin@anduinxue.com',
    1,
    'usericon/default.png'
);
