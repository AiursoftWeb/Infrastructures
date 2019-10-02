using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Aiursoft.Pylon.Services.ToProbeServer;
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
            App thisApp,
            CoreApiService coreApiService,
            AppsContainer appsContainer,
            SitesService sitesService)
        {
            var model = new ViewAppViewModel(user, thisApp);
            await model.Recover(user, thisApp, coreApiService, appsContainer, sitesService);
            return model;
        }

        public async Task Recover(
            DeveloperUser user,
            App thisApp,
            CoreApiService coreApiService,
            AppsContainer appsContainer,
            SitesService sitesService)
        {
            RootRecover(user, 1);
            var token = await appsContainer.AccessToken(thisApp.AppId, thisApp.AppSecret);


            var grants = await coreApiService.AllUserGrantedAsync(token);
            Grants = grants.Grants;

            var sites = await sitesService.ViewMySitesAsync(token);
            Sites = sites.Sites;
        }

        private ViewAppViewModel(DeveloperUser user, App thisApp) : base(user)
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
            ViewOpenId = thisApp.ViewOpenId;
            ViewPhoneNumber = thisApp.ViewPhoneNumber;
            ChangePhoneNumber = thisApp.ChangePhoneNumber;
            ConfirmEmail = thisApp.ConfirmEmail;
            ChangeBasicInfo = thisApp.ChangeBasicInfo;
            ChangePassword = thisApp.ChangePassword;
            ChangeGrantInfo = thisApp.ChangeGrantInfo;
            ViewAuditLog = thisApp.ViewAuditLog;
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
        public string AppDomain { get; set; }
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

        public IEnumerable<Site> Sites { get; set; }
        public IEnumerable<Grant> Grants { get; set; }
    }
}
