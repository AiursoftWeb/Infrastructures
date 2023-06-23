using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Warpgate.Repositories;
using Aiursoft.Warpgate.SDK.Models.AddressModels;
using Aiursoft.Warpgate.SDK.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Warpgate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class RecordsController : ControllerBase
{
    private readonly AppRepo _appRepo;
    private readonly RecordRepo _recordRepo;

    public RecordsController(
        AppRepo appRepo,
        RecordRepo recordRepo)
    {
        _appRepo = appRepo;
        _recordRepo = recordRepo;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewRecord(CreateNewRecordAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var createdRecord = await _recordRepo.CreateRecord(model.NewRecordName, model.Type, appid, model.TargetUrl,
            model.Enabled, model.Tags);
        return this.Protocol(Code.Success,
            $"Successfully created your new record: '{createdRecord.RecordUniqueName}' at {createdRecord.CreationTime}.");
    }

    [Produces(typeof(ViewMyRecordsViewModel))]
    public async Task<IActionResult> ViewMyRecords(ViewMyRecordsAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var records = await _recordRepo.GetAllRecordsUnderApp(appid, model.Tag);
        var viewModel = new ViewMyRecordsViewModel
        {
            AppId = appid,
            Records = records,
            Code = Code.Success,
            Message = "Successfully get all your records!"
        };
        return this.Protocol(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRecordInfo(UpdateRecordInfoAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var record = await _recordRepo.GetRecordByNameUnderApp(model.OldRecordName, appid);
        // Conflict = Name changed, and new name already exists.
        var conflict = model.NewRecordName.ToLower() != model.OldRecordName.ToLower() &&
                       await _recordRepo.GetRecordByName(model.NewRecordName) != null;
        if (conflict)
        {
            return this.Protocol(Code.Conflict,
                $"There is already a record with name: '{model.NewRecordName}'. Please try another new name.");
        }

        record.RecordUniqueName = model.NewRecordName.ToLower();
        record.Type = model.NewType;
        record.TargetUrl = model.NewUrl;
        record.Enabled = model.Enabled;
        record.Tags = model.Tags;
        await _recordRepo.UpdateRecord(record);
        return this.Protocol(Code.Success, "Successfully updated your Record!");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRecord(DeleteRecordAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var record = await _recordRepo.GetRecordByNameUnderApp(model.RecordName, appid);
        await _recordRepo.DeleteRecord(record);
        return this.Protocol(Code.Success, "Successfully deleted your Record!");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
    {
        var app = await _appRepo.GetApp(model.AccessToken);
        if (app.AppId != model.AppId)
        {
            return this.Protocol(Code.Unauthorized,
                "The app you try to delete is not the access token you granted!");
        }

        await _appRepo.DeleteApp(app);
        return this.Protocol(Code.Success, $"The app with ID: {app.AppId} was successfully deleted!");
    }
}