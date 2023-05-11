namespace Aiursoft.WWW.Services.BingModels;

public class BingResponse
{
    public DetectedLanguage DetectedLanguage { get; set; }
    public TranslationsItem[] Translations { get; set; }
}