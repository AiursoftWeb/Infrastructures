using System;
using System.Collections.Generic;

namespace Aiursoft.Handler.Models
{
    public class AiurPagedCollection<T> : AiurCollection<T>
    {
        [Obsolete("This method is only for framework", true)]
        public AiurPagedCollection() { }
        public AiurPagedCollection(List<T> items) : base(items) { }

        public int TotalCount { get; set; }
        /// <summary>
        /// Starts from 1.
        /// </summary>
        public int CurrentPage { get; set; }

        public int CurrentPageSize { get; set; }
    }
}
