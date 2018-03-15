using System.Collections.Generic;
using Octokit;

namespace WebApplication6.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(IEnumerable<Repository> repositories)
        {
            Repositories = repositories;
        }

        public IEnumerable<Repository> Repositories { get; private set; }
    }
}