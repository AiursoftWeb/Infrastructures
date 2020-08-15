using System.Collections.Generic;

namespace Aiursoft.WWW.Services.BingModels
{
    public class BingResponse
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public List<TranslationsItem> Translations { get; set; }
    }
}
