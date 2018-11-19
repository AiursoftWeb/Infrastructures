using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Aiursoft.Pylon.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Aiursoft.Wiki.Models
{
    public class WikiUser : AiurUserBase
    {
        [InverseProperty(nameof(Comment.CommentUser))]
        public List<Comment> Comments { get; set; }
    }

    public class Collection
    {
        public int CollectionId { get; set; }
        public string CollectionTitle { get; set; }
        [InverseProperty(nameof(Article.Collection))]
        public List<Article> Articles { get; set; }
        [NotMapped]
        public string DocAPIAddress { get; set; }
    }
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        [JsonIgnore]
        public string ArticleContent { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        [InverseProperty(nameof(Comment.Article))]
        [JsonIgnore]
        public List<Comment> Comments { get; set; }

        [JsonIgnore]
        public int? CollectionId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(CollectionId))]
        public Collection Collection { get; set; }
    }
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        public int ArticleId { get; set; }
        [ForeignKey(nameof(ArticleId))]
        public Article Article { get; set; }

        public string CommentUserId { get; set; }
        [ForeignKey(nameof(CommentUserId))]
        public WikiUser CommentUser { get; set; }
    }
}
