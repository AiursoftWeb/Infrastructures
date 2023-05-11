namespace Aiursoft.Developer.SDK.Services;

public class DeveloperLocator
{
    public DeveloperLocator(string endpoint)
    {
        Endpoint = endpoint;
    }

    public string Endpoint { get; }
}