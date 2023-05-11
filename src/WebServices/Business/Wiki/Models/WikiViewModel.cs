using System.Collections.Generic;

namespace Aiursoft.Wiki.Models;

public class WikiViewModel
{
    public IEnumerable<Collection> Collections { get; set; }
    public Collection CurrentCollection { get; set; }
    public Article CurrentArticle { get; set; }
    public string RenderedContent { get; set; }
}