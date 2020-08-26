SET QUOTED_IDENTIFIER ON;

use Gateway;

delete from UserEmails;
delete from AspNetUsers;

insert into AspNetUsers(AccessFailedCount,LockoutEnabled,TwoFactorEnabled,UserName,NormalizedUserName,PasswordHash,SecurityStamp,ConcurrencyStamp,AccountCreateTime,Id,NickName,PreferedLanguage,Email,IconFilePath)
VALUES(0,1,0,'admin@{{domain}}','ADMIN@{{domainUpper}}','AQAAAAEAACcQAAAAEHswFfciW8m5p2De3KzBd7ha0OX04Dav4jRy3CL/M/L54nWtMOGLwDZF1qo211KVRg==','3BO4UKYBY2GJVGNTBM756HO6NQLUAKGQ','89f42d2b-1af9-416a-8953-5abb02b523a1','2020-01-01 12:34:56','{{userId}}','Admin','en-US','admin@{{domain}}','usericon/default.png');

insert into UserEmails(EmailAddress,OwnerId,Priority,Validated,LastSendTime)
VALUES('admin@{{domain}}','{{userId}}',1,0,'2020-01-01 12:34:56')

use Developer;

delete from AspNetUsers;
delete from Apps;

insert into AspNetUsers(AccessFailedCount,LockoutEnabled,TwoFactorEnabled,UserName,NormalizedUserName,PasswordHash,SecurityStamp,ConcurrencyStamp,AccountCreateTime,Id,NickName,PreferedLanguage,Email,EmailConfirmed,IconFilePath)
VALUES(0,1,0,'admin@{{domain}}','ADMIN@{{domainUpper}}','AQAAAAEAACcQAAAAEHswFfciW8m5p2De3KzBd7ha0OX04Dav4jRy3CL/M/L54nWtMOGLwDZF1qo211KVRg==','3BO4UKYBY2GJVGNTBM756HO6NQLUAKGQ','89f42d2b-1af9-416a-8953-5abb02b523a1','2020-01-01 12:34:56','{{userId}}','Admin','en-US','admin@{{domain}}',1,'usericon/default.png');

insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword)
values('{{developerAppId}}','{{developerAppSecret}}','{{Instance}} Developer Center','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Developer Center',0,7,1,0,0,0,'developer.{{domain}}',1,'{{userId}}',0,0,0,1,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword)
values('{{gatewayAppId}}','{{gatewayAppSecret}}','{{Instance}} Gateway','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Gateway',0,7,1,0,0,0,'gateway.{{domain}}',1,'{{userId}}',0,0,0,1,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword)
values('{{stargateAppId}}','{{stargateAppSecret}}','{{Instance}} Stargate','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Stargate',0,7,1,0,0,0,'stargate.{{domain}}',1,'{{userId}}',0,0,0,1,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword)
values('{{observerAppId}}','{{observerAppSecret}}','{{Instance}} Observer','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Observer',0,7,1,0,0,0,'observer.{{domain}}',1,'{{userId}}',0,0,0,1,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword)
values('{{probeAppId}}','{{probeAppSecret}}','{{Instance}} Probe','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Probe',0,7,1,0,0,0,'probe.{{domain}}',1,'{{userId}}',0,0,0,1,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword)
values('{{wrapgateAppId}}','{{wrapgateAppSecret}}','{{Instance}} Wrapgate','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Wrapgate',0,7,1,0,0,0,'wrapgate.{{domain}}',1,'{{userId}}',0,0,0,1,1);
