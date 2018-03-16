using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public class RepositoryGitHub
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ForksCount { get; set; }
        public string CloneUrl { get; set; }
        public string HtmlUrl { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}