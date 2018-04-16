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
    public static class ApiService
    {
        public static async Task<AiurProtocal> DeleteAppAsync(string AccessToken, string AppId)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "DeleteApp", new { });
            var form = new AiurUrl(string.Empty, new DeleteAppAddressModel
            {
                AccessToken = AccessToken,
                AppId = AppId
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success && jResult.Code != ErrorType.HasDoneAlready)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<ViewMyBucketsViewModel> ViewMyBucketsAsync(string AccessToken)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "ViewMyBuckets", new ViewMyBucketsAddressModel
            {
                AccessToken = AccessToken
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<ViewMyBucketsViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public static async Task<CreateBucketViewModel> CreateBucketAsync(string AccessToken, string BucketName, bool OpenToRead, bool OpenToUpload)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "CreateBucket", new { });
            var form = new AiurUrl(string.Empty, new CreateBucketAddressModel
            {
                AccessToken = AccessToken,
                BucketName = BucketName,
                OpenToRead = OpenToRead,
                OpenToUpload = OpenToUpload
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<CreateBucketViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<AiurProtocal> EditBucketAsync(string AccessToken, int BucketId, string NewBucketName, bool OpenToRead, bool OpenToUpload)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "EditBucket", new { });
            var form = new AiurUrl(string.Empty, new EditBucketAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId,
                NewBucketName = NewBucketName,
                OpenToRead = OpenToRead,
                OpenToUpload = OpenToUpload
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<ViewBucketViewModel> ViewBucketDetailAsync(int BucketId)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "ViewBucketDetail", new ViewBucketDetailAddressModel
            {
                BucketId = BucketId
            });
            var result = await httpContainer.Get(url);
            var jResult = JsonConvert.DeserializeObject<ViewBucketViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<AiurProtocal> DeleteBucketAsync(string AccessToken, int BucketId)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "DeleteBucket", new { });
            var form = new AiurUrl(string.Empty, new DeleteBucketAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<ViewOneFileViewModel> ViewOneFileAsync(int FileKey)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "ViewOneFile", new ViewOneFileAddressModel
            {
                FileKey = FileKey
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<ViewOneFileViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public static async Task<UploadFileViewModel> UploadFile(string AccessToken, int BucketId, string FilePath, int AliveDays)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "UploadFile", new UploadFileAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId,
                AliveDays = AliveDays
            });
            var result = await httpContainer.PostFile(url, FilePath);
            var jResult = JsonConvert.DeserializeObject<UploadFileViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<ViewAllFilesViewModel> ViewAllFilesAsync(string AccessToken, int BucketId)
        {
            var http = new HTTPService();
            var path = new AiurUrl(Values.OssServerAddress, "api", "viewallfiles", new CommonAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId
            });
            var result = await http.Get(path);
            var jResult = JsonConvert.DeserializeObject<ViewAllFilesViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<AiurProtocal> DeleteFileAsync(string AccessToken, int FileKey, int BucketId)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "api", "DeleteFile", new { });
            var form = new AiurUrl(string.Empty, new DeleteFileAddressModel
            {
                AccessToken = AccessToken,
                BucketId = BucketId,
                FileKey = FileKey
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
