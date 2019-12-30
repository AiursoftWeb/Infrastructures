using System.ComponentModel.DataAnnotations;

namespace Aiursoft.XelNaga.Interfaces
{
    public interface IPageable
    {
        /// <summary>
        /// Default is 10
        /// </summary>
        [Range(1, 100)]
        int PageSize { get; set; }
        /// <summary>
        /// Starts from 1.
        /// </summary>
        [Range(1, int.MaxValue)]
        int PageNumber { get; set; }
    }
}