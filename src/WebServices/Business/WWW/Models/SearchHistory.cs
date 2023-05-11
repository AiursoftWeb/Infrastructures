using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.WWW.Models;

public class SearchHistory
{
    public int Id { get; set; }
    public string Question { get; set; }
    public int Page { get; set; }
    public DateTime SearchTime { get; set; } = DateTime.UtcNow;

    public string TriggerUserId { get; set; }

    [ForeignKey(nameof(TriggerUserId))] public WWWUser TriggerUser { get; set; }
}