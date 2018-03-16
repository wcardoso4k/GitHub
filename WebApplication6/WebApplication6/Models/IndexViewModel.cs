using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(IEnumerable<RepositoryGitHub> repositories)
        {
            Repositories = repositories;
        }

        public IEnumerable<RepositoryGitHub> Repositories { get; private set; }
    }
}