SET QUOTED_IDENTIFIER ON;

use Gateway;

delete from UserEmails;
delete from AspNetUsers;

insert into AspNetUsers(AccessFailedCount,LockoutEnabled,TwoFactorEnabled,UserName,NormalizedUserName,PasswordHash,SecurityStamp,ConcurrencyStamp,AccountCreateTime,Id,NickName,PreferedLanguage,Email,IconFilePath)
VALUES(0,1,0,'admin@{{domain}}','ADMIN@{{domainUpper}}','AQAAAAEAACcQAAAAEHswFfciW8m5p2De3KzBd7ha0OX04Dav4jRy3CL/M/L54nWtMOGLwDZF1qo211KVRg==','3BO4UKYBY2GJVGNTBM756HO6NQLUAKGQ','89f42d2b-1af9-416a-8953-5abb02b523a1','2020-01-01 12:34:56','{{userId}}','Admin','en-US','admin@{{domain}}','usericon/default.png');

insert into UserEmails(EmailAddress,OwnerId,Priority,Validated,LastSendTime)
VALUES('admin@{{domain}}','{{userId}}',1,1,'2020-01-01 12:34:56')

use Developer;

delete from AspNetUsers;
delete from Apps;

insert into AspNetUsers(AccessFailedCount,LockoutEnabled,TwoFactorEnabled,UserName,NormalizedUserName,PasswordHash,SecurityStamp,ConcurrencyStamp,AccountCreateTime,Id,NickName,PreferedLanguage,Email,EmailConfirmed,IconFilePath)
VALUES(0,1,0,'admin@{{domain}}','ADMIN@{{domainUpper}}','AQAAAAEAACcQAAAAEHswFfciW8m5p2De3KzBd7ha0OX04Dav4jRy3CL/M/L54nWtMOGLwDZF1qo211KVRg==','3BO4UKYBY2GJVGNTBM756HO6NQLUAKGQ','89f42d2b-1af9-416a-8953-5abb02b523a1','2020-01-01 12:34:56','{{userId}}','Admin','en-US','admin@{{domain}}',1,'usericon/default.png');

insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{archonAppId}}','{{archonAppSecret}}','{{Instance}} Archon','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Archon',0,7,0,0,0,0,'archon.{{domain}}',0,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{developerAppId}}','{{developerAppSecret}}','{{Instance}} Developer Center','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Developer Center',0,7,1,0,0,0,'developer.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{gatewayAppId}}','{{gatewayAppSecret}}','{{Instance}} Gateway','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Gateway',0,7,1,0,0,0,'gateway.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{stargateAppId}}','{{stargateAppSecret}}','{{Instance}} Stargate','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Stargate',0,7,1,0,0,0,'stargate.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{observerAppId}}','{{observerAppSecret}}','{{Instance}} Observer','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Observer',0,7,1,0,0,0,'observer.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{probeAppId}}','{{probeAppSecret}}','{{Instance}} Probe','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Probe',0,7,1,0,0,0,'probe.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{wrapgateAppId}}','{{wrapgateAppSecret}}','{{Instance}} Wrapgate','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Wrapgate',0,7,1,0,0,0,'wrapgate.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{wwwAppId}}','{{wwwAppSecret}}','{{Instance}} Home','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Home',0,7,1,0,0,0,'www.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{wikiAppId}}','{{wikiAppSecret}}','{{Instance}} Wiki Center','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Wiki Center',0,7,1,0,0,0,'wiki.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,ViewAuditLog,ManageSocialAccount,ChangeGrantInfo,TrustedApp)
values('{{accountAppId}}','{{accountAppSecret}}','{{Instance}} Account Center','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Account Center',0,7,1,0,0,0,'account.{{domain}}',1,'{{userId}}',1,1,1,1,1,1,1,1,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{statusAppId}}','{{statusAppSecret}}','{{Instance}} Status Center','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Status Center',0,7,1,0,0,0,'status.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{colossusAppId}}','{{colossusAppSecret}}','{{Instance}} Colossus','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Colossus',0,7,1,0,0,0,'colossus.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{wrapAppId}}','{{wrapAppSecret}}','{{Instance}} Wrap','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft Wrap',0,7,1,0,0,0,'wrap.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);
insert into Apps(AppId,AppSecret,AppName,AppCreateTime,IconPath,AppDescription,AppCategory,AppPlatform,EnableOAuth,ForceConfirmation,ForceInputPassword,DebugMode,[AppDomain],ViewOpenId,CreatorId,ViewPhoneNumber,ChangePhoneNumber,ConfirmEmail,ChangeBasicInfo,ChangePassword,TrustedApp)
values('{{eeAppId}}','{{eeAppSecret}}','{{Instance}} EE','2020-01-01 12:34:56','aiursoft-app-icons/appdefaulticon.png','Aiursoft EE',0,7,1,0,0,0,'ee.{{domain}}',1,'{{userId}}',0,0,0,0,0,1);

