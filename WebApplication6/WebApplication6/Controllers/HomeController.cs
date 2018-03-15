using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using Octokit;
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
                var model = new IndexViewModel(repositories);
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

        public async Task<JsonResult> GetAllLanguages()
        {
            var getLanguages = await _client.Repository.GetAllLanguages("wcardoso4k", "azureRestAPIGitHub");
            string json = JsonConvert.SerializeObject(getLanguages);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        // This is the Callback URL that the GitHub OAuth Login page will redirect back to.
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

        private string GetOauthLoginUrl()
        {
            string csrf = Membership.GeneratePassword(24, 1);
            Session["CSRF:State"] = csrf;

            // 1. Redirect users to request GitHub access
            var request = new OauthLoginRequest(clientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf
            };
            var oauthLoginUrl = _client.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginUrl.ToString();
        }
    }
}