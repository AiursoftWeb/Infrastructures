using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Services.ToDirectoryServer;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.SDK.Models.EventViewModels;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.Stargate.SDK.Models;
using Aiursoft.Stargate.SDK.Services.ToStargateServer;
using Aiursoft.Warpgate.SDK.Models;
using Aiursoft.Warpgate.SDK.Services.ToWarpgateServer;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class ViewAppViewModel : CreateAppViewModel
{
    [Obsolete("This method is only for framework", true)]
    public ViewAppViewModel()
    {
    }

    public ViewAppViewModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps) : base(user, hisApps)
    {
    }
    
    public bool JustHaveUpdated { get; set; }
    public string AppId { get; set; }
    public string AppSecret { get; set; }

    [Url]
    [Display(Name = "Privacy Statement Url")]
    public string PrivacyStatementUrl { get; set; }

    [Url] [Display(Name = "License Url")] public string LicenseUrl { get; set; }

    [Display(Name = "Enable OAuth")] public bool EnableOAuth { get; set; }

    [Display(Name = "Force Input Password")]
    public bool ForceInputPassword { get; set; }

    [Display(Name = "Force Confirmation")] public bool ForceConfirmation { get; set; }

    [Display(Name = "Debug Mode")] public bool DebugMode { get; set; }

    [Display(Name = "App Domain")]
    [ValidDomainName]
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

    [Display(Name = "Manage user's social account.")]
    public bool ManageSocialAccount { get; set; }

    public IEnumerable<Site> Sites { get; set; }
    public IEnumerable<LogCollection> ErrorLogs { get; set; }
    public IEnumerable<Channel> Channels { get; set; }
    public IEnumerable<WarpRecord> Records { get; set; }
    public bool Trusted { get; internal set; }
    public AiurPagedCollection<Grant> Grants { get; set; }

    public static async Task<ViewAppViewModel> SelfCreateAsync(
        PortalUser user,
        IReadOnlyCollection<DirectoryApp> hisApps, 
        DirectoryApp app,
        AppsService coreApiService,
        AppsContainer appsContainer,
        SitesService sitesService,
        ObserverService eventService,
        ChannelService channelService,
        RecordsService recordsService,
        int pageNumber)
    {
        var model = new ViewAppViewModel(user, hisApps);
        await model.Recover(user, hisApps, app, coreApiService, appsContainer, sitesService, eventService, channelService,
            recordsService, pageNumber);
        return model;
    }

    public async Task Recover(
        PortalUser user,
        IReadOnlyCollection<DirectoryApp> hisApps, 
        DirectoryApp appInDb,
        AppsService coreApiService,
        AppsContainer appsContainer,
        SitesService sitesService,
        ObserverService eventService,
        ChannelService channelService,
        RecordsService recordsService,
        int pageNumber)
    {
        RootRecover(user, hisApps);
        AppName = appInDb.AppName;
        AppDescription = appInDb.AppDescription;
        AppId = appInDb.AppId;
        AppSecret = appInDb.AppSecret;
        EnableOAuth = appInDb.EnableOAuth;
        ForceInputPassword = appInDb.ForceInputPassword;
        ForceConfirmation = appInDb.ForceConfirmation;
        DebugMode = appInDb.DebugMode;
        PrivacyStatementUrl = appInDb.PrivacyStatementUrl;
        LicenseUrl = appInDb.LicenseUrl;
        IconPath = appInDb.IconPath;
        AppDomain = appInDb.AppDomain;

        ViewOpenId = appInDb.ViewOpenId;
        ViewPhoneNumber = appInDb.ViewPhoneNumber;
        ChangePhoneNumber = appInDb.ChangePhoneNumber;
        ConfirmEmail = appInDb.ConfirmEmail;
        ChangeBasicInfo = appInDb.ChangeBasicInfo;
        ChangePassword = appInDb.ChangePassword;
        ChangeGrantInfo = appInDb.ChangeGrantInfo;
        ViewAuditLog = appInDb.ViewAuditLog;
        ManageSocialAccount = appInDb.ManageSocialAccount;
        
        var token = await appsContainer.GetAccessTokenWithAppInfoAsync(appInDb.AppId, appInDb.AppSecret);

        // TODO: After migration to Directory, load from DB.
        Grants = await coreApiService.AllUserGrantedAsync(token, pageNumber, 15);

        var sites = await sitesService.ViewMySitesAsync(token);
        Sites = sites.Sites;

        var errorLogs = await eventService.ViewAsync(token);
        ErrorLogs = errorLogs.Logs;

        var channels = await channelService.ViewMyChannelsAsync(token);
        Channels = channels.Channels;

        var records = await recordsService.ViewMyRecordsAsync(token);
        Records = records.Records;

        Trusted = appInDb.TrustedApp;
    }
}