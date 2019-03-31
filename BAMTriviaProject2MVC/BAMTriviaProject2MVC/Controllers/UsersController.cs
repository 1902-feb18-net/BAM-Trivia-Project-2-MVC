using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BAMTriviaProject2MVC.Controllers
{
    public class UsersController : AServiceController
    {
        public UsersController(HttpClient httpClient, IConfiguration configuration)
            : base(httpClient, configuration)
        { }

        // GET: Users
        public ActionResult Login()
        {
            return View();
        }
        
    }
}