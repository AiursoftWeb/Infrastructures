using Aiursoft.Archon.SDK.Services;
using Aiursoft.Gateway.SDK.Models.API;
using Aiursoft.Gateway.SDK.Services.ToGatewayServer;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.Stargate.SDK.Models;
using Aiursoft.Stargate.SDK.Services.ToStargateServer;
using Aiursoft.Status.SDK.Models.EventViewModels;
using Aiursoft.Status.SDK.Services.ToStatusServer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class ViewAppViewModel : CreateAppViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewAppViewModel() { }
        public static async Task<ViewAppViewModel> SelfCreateAsync(
            DeveloperUser user,
            DeveloperApp appInDb,
            CoreApiService coreApiService,
            AppsContainer appsContainer,
            SitesService sitesService,
            EventService eventService,
            ChannelService channelService,
            int pageNumber)
        {
            var model = new ViewAppViewModel(user, appInDb);
            await model.Recover(user, appInDb, coreApiService, appsContainer, sitesService, eventService, channelService, pageNumber);
            return model;
        }

        public async Task Recover(
            DeveloperUser user,
            DeveloperApp appInDb,
            CoreApiService coreApiService,
            AppsContainer appsContainer,
            SitesService sitesService,
            EventService eventService,
            ChannelService channelService,
            int pageNumber)
        {
            RootRecover(user);
            var token = await appsContainer.AccessToken(appInDb.AppId, appInDb.AppSecret);

            Grants = await coreApiService.AllUserGrantedAsync(token, pageNumber, 15);

            var sites = await sitesService.ViewMySitesAsync(token);
            Sites = sites.Sites;

            var errorLogs = await eventService.ViewAsync(token);
            ErrorLogs = errorLogs.Logs;

            var channels = await channelService.ViewMyChannelsAsync(token);
            Channels = channels.Channels;

            Trusted = appInDb.TrustedApp;
        }

        private ViewAppViewModel(DeveloperUser user, DeveloperApp thisApp) : base(user)
        {
            if (thisApp.CreatorId != user.Id)
            {
                throw new InvalidOperationException("The app is not the user's app!");
            }
            AppName = thisApp.AppName;
            AppDescription = thisApp.AppDescription;
            AppCategory = thisApp.AppCategory;
            AppPlatform = thisApp.AppPlatform;
            AppId = thisApp.AppId;
            AppSecret = thisApp.AppSecret;
            EnableOAuth = thisApp.EnableOAuth;
            ForceInputPassword = thisApp.ForceInputPassword;
            ForceConfirmation = thisApp.ForceConfirmation;
            DebugMode = thisApp.DebugMode;
            PrivacyStatementUrl = thisApp.PrivacyStatementUrl;
            LicenseUrl = thisApp.LicenseUrl;
            IconPath = thisApp.IconPath;
            AppDomain = thisApp.AppDomain;
            AppFailCallbackUrl = thisApp.AppFailCallbackUrl;

            ViewOpenId = thisApp.ViewOpenId;
            ViewPhoneNumber = thisApp.ViewPhoneNumber;
            ChangePhoneNumber = thisApp.ChangePhoneNumber;
            ConfirmEmail = thisApp.ConfirmEmail;
            ChangeBasicInfo = thisApp.ChangeBasicInfo;
            ChangePassword = thisApp.ChangePassword;
            ChangeGrantInfo = thisApp.ChangeGrantInfo;
            ViewAuditLog = thisApp.ViewAuditLog;
            ManageSocialAccount = thisApp.ManageSocialAccount;
        }

        public bool JustHaveUpdated { get; set; } = false;
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        [Url]
        [Display(Name = "Privacy Statement Url")]
        public string PrivacyStatementUrl { get; set; }
        [Url]
        [Display(Name = "License Url")]
        public string LicenseUrl { get; set; }

        [Display(Name = "Enable OAuth")]
        public bool EnableOAuth { get; set; }
        [Display(Name = "Force Input Password")]
        public bool ForceInputPassword { get; set; }
        [Display(Name = "Force Confirmation")]
        public bool ForceConfirmation { get; set; }
        [Display(Name = "Debug Mode")]
        public bool DebugMode { get; set; }
        [Display(Name = "App Domain")]
        [ValidDomainName]
        public string AppDomain { get; set; }
        [Url]
        [Display(Name = "App fail OAuth callback URL")]
        public string AppFailCallbackUrl { get; set; }

        // Permissions
        [Display(Name = "View user's basic identity info")]
        public bool ViewOpenId { get; set; } = true;
        [Display(Name = "View user's phone number")]
        public bool ViewPhoneNumber { get; set; }
        [Display(Name = "Change user's phone number")]
        public bool ChangePhoneNumber { get; set; }
        [Display(Name = "Change user's Email confirmation status")]
        public bool ConfirmEmail { get; set; }
        [Display(Name = "Change user's basic info like nickname and bio")]
        public bool ChangeBasicInfo { get; set; }
        [Display(Name = "Change the user's password based on source password")]
        public bool ChangePassword { get; set; }
        [Display(Name = "Change user's other applications' grant status")]
        public bool ChangeGrantInfo { get; set; }
        [Display(Name = "View user's sign in log.")]
        public bool ViewAuditLog { get; set; }
        [Display(Name = "Manage user's social account.")]
        public bool ManageSocialAccount { get; set; }

        public IEnumerable<Site> Sites { get; set; }
        public IEnumerable<LogCollection> ErrorLogs { get; set; }
        public IEnumerable<ChannelDetail> Channels { get; set; }
        public bool Trusted { get; internal set; }
        public AiurPagedCollection<Grant> Grants { get; set; }
    }
}
