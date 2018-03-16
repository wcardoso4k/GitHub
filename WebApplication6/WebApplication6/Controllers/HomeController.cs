using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using Octokit;
using WebApplication6.db;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        #region Token GitHub

        readonly string clientId = "62b415d60c347d71ef6a";
        readonly string clientSecret = "4d975c797efef28884d63ba312be1c6804ac66cb";

        #endregion

        #region Iniciliza GitHubClient

        readonly GitHubClient _client = new GitHubClient(new ProductHeaderValue("Connect"));

        #endregion

        #region Inicilização do entity framework

        private readonly GiHubContext _context;

        #endregion

        #region GitHub Info

        private const string Ownwer = "wcardoso4k";
        private const string RepositoryName = "GitHub";

        #endregion

        #region Construtor

        /// <summary>
        /// Construtor
        /// </summary>
        public HomeController()
        {
            if (_context == null)
                _context = new GiHubContext();
        }

        #endregion

        #region View

        /// <summary>
        /// Página principal
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            var accessToken = Session["OAuthToken"] as string;
            if (accessToken != null)
            {
                // This allows the client to make requests to the GitHub API on the user's behalf
                // without ever having the user's OAuth credentials.
                _client.Credentials = new Credentials(accessToken);
            }

            try
            {
                // The following requests retrieves all of the user's repositories and
                // requires that the user be logged in to work.
                var repositories = await _client.Repository.GetAllForCurrent();

                // Salva os detalhes de cada repositorio no BD
                var newRepository = await SaveRepositorys(repositories);

                // Load no objeto para visualizar a view
                var model = new IndexViewModel(newRepository);

                // Carrega view com o objeto
                return View(model);
            }
            catch (AuthorizationException)
            {
                // Either the accessToken is null or it's invalid. This redirects
                // to the GitHub OAuth login page. That page will redirect back to the
                // Authorize action.
                return Redirect(GetOauthLoginUrl());
            }
        }

        /// <summary>
        /// Redireciona para página localhost após autenticar no gitbug e obter os parâmetros do método.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<ActionResult> Authorize(string code, string state)
        {
            if (String.IsNullOrEmpty(code))
                return RedirectToAction("Index");

            var expectedState = Session["CSRF:State"] as string;
            if (state != expectedState) throw new InvalidOperationException("SECURITY FAIL!");
            Session["CSRF:State"] = null;

            var token = await _client.Oauth.CreateAccessToken(new OauthTokenRequest(clientId, clientSecret, code));
            Session["OAuthToken"] = token.AccessToken;

            return RedirectToAction("Index");
        }


        #endregion

        #region JsonResult

        /// <summary>
        /// Busca as linguagens de programação no repositorios
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetAllLanguages()
        {
            var getLanguages = await _client.Repository.GetAllLanguages(Ownwer, RepositoryName);
            var json = JsonConvert.SerializeObject(getLanguages);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Autenticação Oauth

        /// <summary>
        /// Retorna endereço URL localhost
        /// </summary>
        /// <returns></returns>
        private string GetOauthLoginUrl()
        {
            string csrf = Membership.GeneratePassword(24, 1);
            Session["CSRF:State"] = csrf;

            // 1. Redirect users to request GitHub access
            var request = new OauthLoginRequest(clientId)
            {
                Scopes = {"user", "notifications"},
                State = csrf
            };
            var oauthLoginUrl = _client.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginUrl.ToString();
        }

        #endregion

        #region Entity framework salva os objetos

        /// <summary>
        /// Salva no banco os os detalhes de cada repositório
        /// </summary>
        /// <param name="repositories"></param>
        /// <returns></returns>
        public async Task<List<RepositoryGitHub>> SaveRepositorys(IEnumerable<Octokit.Repository> repositories)
        {
            if (_context == null)
                throw new Exception("Banco de dados não iniciado corretamente.");

            try
            {
                var lst = new List<RepositoryGitHub>();
                var enumerable = repositories as IList<Repository> ?? repositories.ToList();
                Parallel.ForEach(enumerable, repository =>
                {
                    var newRepositoryGitHub = new RepositoryGitHub
                    {
                        Name = repository.Name,
                        Description = repository.Description,
                        ForksCount = repository.ForksCount,
                        CloneUrl = repository.CloneUrl,
                        HtmlUrl = repository.HtmlUrl,
                        Created = repository.CreatedAt
                    };
                    lst.Add(newRepositoryGitHub);
                });
                _context.Repositories.AddRange(lst);
                await _context.SaveChangesAsync();
                return lst;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Erro ao inserir repositório. {0}", ex.Message));
            }
        }

        #endregion
    }
}