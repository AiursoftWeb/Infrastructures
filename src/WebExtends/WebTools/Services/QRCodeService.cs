using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Tools;
using QRCoder;

namespace Aiursoft.WebTools.Services;

public class QRCodeService : ITransientDependency
{
    public byte[] ToQRCodePngBytes(string source)
    {
        var qrCodeData = new QRCodeGenerator().CreateQrCode(source, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new QRCode(qrCodeData);
        using var qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, true);
        using var memoryStream = new MemoryStream();
        qrCodeImage.Save(memoryStream, ImageFormat.Png);
        var byteBuffer = memoryStream.ToArray();
        memoryStream.Close();
        return byteBuffer;
    }

    public string ToQRCodeBase64(string source)
    {
        var qrCode = ToQRCodePngBytes(source);
        return "data:image/png;base64," + qrCode.BytesToBase64();
    }
}