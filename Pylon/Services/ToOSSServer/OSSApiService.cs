using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.OSS;
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

        public async Task<AiurProtocol> DeleteAppAsync(string accessToken, string appId)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "DeleteApp", new { });
            var form = new AiurUrl(string.Empty, new DeleteAppAddressModel
            {
                AccessToken = accessToken,
                AppId = appId
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewMyBucketsViewModel> ViewMyBucketsAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "ViewMyBuckets", new ViewMyBucketsAddressModel
            {
                AccessToken = accessToken
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<ViewMyBucketsViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<CreateBucketViewModel> CreateBucketAsync(string accessToken, string bucketName, bool openToRead, bool openToUpload)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "CreateBucket", new { });
            var form = new AiurUrl(string.Empty, new CreateBucketAddressModel
            {
                AccessToken = accessToken,
                BucketName = bucketName,
                OpenToRead = openToRead,
                OpenToUpload = openToUpload
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<CreateBucketViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> EditBucketAsync(string accessToken, int bucketId, string newBucketName, bool openToRead, bool openToUpload)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "EditBucket", new { });
            var form = new AiurUrl(string.Empty, new EditBucketAddressModel
            {
                AccessToken = accessToken,
                BucketId = bucketId,
                NewBucketName = newBucketName,
                OpenToRead = openToRead,
                OpenToUpload = openToUpload
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewBucketViewModel> ViewBucketDetailAsync(int bucketId)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "ViewBucketDetail", new ViewBucketDetailAddressModel
            {
                BucketId = bucketId
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<ViewBucketViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> DeleteBucketAsync(string accessToken, int bucketId)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "DeleteBucket", new { });
            var form = new AiurUrl(string.Empty, new DeleteBucketAddressModel
            {
                AccessToken = accessToken,
                BucketId = bucketId
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewOneFileViewModel> ViewOneFileAsync(int fileKey)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "ViewOneFile", new ViewOneFileAddressModel
            {
                FileKey = fileKey
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<ViewOneFileViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurCollection<OSSFile>> ViewMultiFilesAsync(int[] ids)
        {
            // Get ids arg.
            var idsArg = string.Empty;
            foreach (var id in ids)
            {
                idsArg += id + ",";
            }
            idsArg = idsArg.Trim(',');
            if (string.IsNullOrEmpty(idsArg))
            {
                idsArg = ",";
            }
            // Send Request
            var path = new AiurUrl(_serviceLocation.OSS, "api", "ViewMultiFiles", new ViewMultiFilesAddressModel
            {
                Ids = idsArg
            });
            var result = await _http.Get(path, true);
            var jResult = JsonConvert.DeserializeObject<AiurCollection<OSSFile>>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<UploadFileViewModel> UploadFile(string accessToken, int bucketId, string filePath, int aliveDays)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "UploadFile", new UploadFileAddressModel
            {
                AccessToken = accessToken,
                BucketId = bucketId,
                AliveDays = aliveDays
            });
            var result = await _http.PostFile(url, filePath);
            var jResult = JsonConvert.DeserializeObject<UploadFileViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewAllFilesViewModel> ViewAllFilesAsync(string accessToken, int bucketId)
        {
            var path = new AiurUrl(_serviceLocation.OSS, "api", "viewallfiles", new CommonAddressModel
            {
                AccessToken = accessToken,
                BucketId = bucketId
            });
            var result = await _http.Get(path, true);
            var jResult = JsonConvert.DeserializeObject<ViewAllFilesViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> DeleteFileAsync(string accessToken, int fileKey)
        {
            var url = new AiurUrl(_serviceLocation.OSS, "api", "DeleteFile", new { });
            var form = new AiurUrl(string.Empty, new DeleteFileAddressModel
            {
                AccessToken = accessToken,
                FileKey = fileKey
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
