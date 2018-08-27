using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public long MaxSize { get; set; }
        public int SizeInMB()
        {
            return Convert.ToInt32(MaxSize / 1024 / 1024);
        }
    }
}
