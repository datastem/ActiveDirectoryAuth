using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActiveDirectoryAuth.Models;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using Microsoft.Extensions.Configuration;

namespace ActiveDirectoryAuth.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(LoginModel model)
        {
            model.UserValidated = ValidateUser(model.Username, model.Password);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string ValidateUser(string username, string password)
        {
            string returnresult = "";
            string domain = config
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, "", username, password))
            {
                using (UserPrincipal user = new UserPrincipal(context))
                {
                    try
                    {
                        user.SamAccountName = username;
                        using (var searcher = new PrincipalSearcher(user))
                        {
                            var result = searcher.FindOne();
                            DirectoryEntry de = (DirectoryEntry) result.GetUnderlyingObject();
                            returnresult = "Password: " + de.Properties["password"].Value;

                                //Console.WriteLine("Last Name : " + de.Properties["sn"].Value);
                                //Console.WriteLine("SAM account name   : " + de.Properties["samAccountName"].Value);
                                //Console.WriteLine("User principal name: " + de.Properties["userPrincipalName"].Value);
                                //Console.WriteLine("Mail: " + de.Properties["mail"].Value);

                                //PrincipalSearchResult<Principal> groups = result.GetGroups();

                                //foreach (Principal item in groups)
                                //{
                                //    Console.WriteLine("Groups: {0}: {1}", item.DisplayName, item.Name);
                                //}
                                //Console.WriteLine();
                        }
                    }
                    catch (Exception ex)
                    {
                        returnresult = ex.Message;
                    }
                }
            }
            return returnresult;
        }
    }
}
