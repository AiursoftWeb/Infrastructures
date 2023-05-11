using System.Net;

namespace Aiursoft.Probe.ViewModels.DownloadViewModels;

public class PlayerViewModel
{
    public PlayerViewModel(string src, string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            PlayUrl = $"{src}?token={WebUtility.UrlEncode(token)}";
        }
        else
        {
            PlayUrl = src;
        }
    }

    public string PlayUrl { get; set; }
    public string Title { get; set; }
}