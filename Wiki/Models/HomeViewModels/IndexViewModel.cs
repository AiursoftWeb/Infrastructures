
using System.Collections.Generic;

namespace Aiursoft.Wiki.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Collection> Collections { get; set; }
        public int CurrentCollectionId { get; set; }
        public int CurrentArticleId { get; set; }
        public string Content { get; set; }
    }
}