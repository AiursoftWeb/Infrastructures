using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public int MaxSize { get; set; }
        public int SizeInMB => MaxSize / 1024 / 1024;
    }
}
