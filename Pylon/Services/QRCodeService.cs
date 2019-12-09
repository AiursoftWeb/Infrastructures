using Aiursoft.Pylon.Interfaces;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Aiursoft.Pylon.Services
{
    public class QRCodeService : ITransientDependency
    {
        public string ToQRCodeBase64(string source)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(source, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            string imagebase64 = string.Empty;
            using (var qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, true))
            {
                imagebase64 = ToBase64String(qrCodeImage, ImageFormat.Png);
            }
            return ("data:image/png;base64," + imagebase64);
        }

        private string ToBase64String(Bitmap bmp, ImageFormat imageFormat)
        {
            string base64String = string.Empty;
            using (var memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, imageFormat);
                memoryStream.Position = 0;
                byte[] byteBuffer = memoryStream.ToArray();
                memoryStream.Close();
                base64String = Convert.ToBase64String(byteBuffer);
                byteBuffer = null;
            }
            return base64String;
        }
    }
}
