using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.OSS.ApiAddressModels;
using Aiursoft.Pylon.Models.OSS.ApiViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToOSSServer
{
    public class OSSApiService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public OSSApiService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AiurProtocal> DeleteAppAsync(string AccessToken, string AppId)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "DeleteApp", new { });
            var form = new AiurUrl(string.Empty, new DeleteAppAddressModel
            {
                AccessToken = AccessToken,
                AppId = AppId
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success && jResult.Code != ErrorType.HasDoneAlready)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewMyBucketsViewModel> ViewMyBucketsAsync(string AccessToken)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "ViewMyBuckets", new ViewMyBucketsAddressModel
            {
                AccessToken = AccessToken
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<ViewMyBucketsViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<CreateBucketViewModel> CreateBucketAsync(string AccessToken, string BucketName, bool OpenToRead, bool OpenToUpload)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "CreateBucket", new { });
            var form = new AiurUrl(string.Empty, new CreateBucketAddressModel
            {
                AccessToken = AccessToken,
                BucketName = BucketName,
                OpenToRead = OpenToRead,
                OpenToUpload = OpenToUpload
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<CreateBucketViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocal> EditBucketAsync(string AccessToken, int BucketId, string NewBucketName, bool OpenToRead, bool OpenToUpload)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "EditBucket", new { });
            var form = new AiurUrl(string.Empty, new EditBucketAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId,
                NewBucketName = NewBucketName,
                OpenToRead = OpenToRead,
                OpenToUpload = OpenToUpload
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewBucketViewModel> ViewBucketDetailAsync(int BucketId)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "ViewBucketDetail", new ViewBucketDetailAddressModel
            {
                BucketId = BucketId
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<ViewBucketViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocal> DeleteBucketAsync(string AccessToken, int BucketId)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "DeleteBucket", new { });
            var form = new AiurUrl(string.Empty, new DeleteBucketAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewOneFileViewModel> ViewOneFileAsync(int FileKey)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "ViewOneFile", new ViewOneFileAddressModel
            {
                FileKey = FileKey
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<ViewOneFileViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<UploadFileViewModel> UploadFile(string AccessToken, int BucketId, string FilePath, int AliveDays)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "UploadFile", new UploadFileAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId,
                AliveDays = AliveDays
            });
            var result = await _http.PostFile(url, FilePath);
            var jResult = JsonConvert.DeserializeObject<UploadFileViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewAllFilesViewModel> ViewAllFilesAsync(string AccessToken, int BucketId)
        {
            var path = new AiurUrl(_serviceLocation.OSS, "api", "viewallfiles", new CommonAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId
            });
            var result = await _http.Get(path, true);
            var jResult = JsonConvert.DeserializeObject<ViewAllFilesViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocal> DeleteFileAsync(string AccessToken, int FileKey, int BucketId)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "DeleteFile", new { });
            var form = new AiurUrl(string.Empty, new DeleteFileAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId,
                FileKey = FileKey
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
