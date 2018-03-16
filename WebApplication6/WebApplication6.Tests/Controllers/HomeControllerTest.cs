using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication6.Controllers;
using WebApplication6.db;
using WebApplication6.Models;

namespace WebApplication6.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public async Task Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SaveRepositorys()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            GiHubContext context = new GiHubContext();

            context.Database.CreateIfNotExists();

            context.Repositories.Add(new RepositoryGitHub
            {
                Name =  "repo",
                Created = DateTimeOffset.Now,
                CloneUrl = "url/test",
                Description = "Description 123",
                ForksCount = 1,
                HtmlUrl = "url/test2222"
            });
            context.SaveChanges();
        }

        [TestMethod]
        public void GetAllLanguages()
        {
            // Arrange
            HomeController controller = new HomeController();

            var languages =  controller.GetAllLanguages();

            Assert.IsNotNull(languages);
        }

        [TestMethod]
        public void GetRepositoriesAzure()
        {
            // Arrange
            var context = new GiHubContext();

            Assert.IsTrue(context.Repositories.Any());

            foreach (var repository in context.Repositories)
            {
                Debug.WriteLine("Id : {0}", repository.Id);
            }
        }
    }
}
