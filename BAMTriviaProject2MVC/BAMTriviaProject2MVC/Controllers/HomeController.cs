using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BAMTriviaProject2MVC.BLLModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using BAMTriviaProject2MVC.AuthModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BAMTriviaProject2MVC.ApiModels;
using BAMTriviaProject2MVC.ViewModels;

namespace BAMTriviaProject2MVC.Controllers
{
    public class HomeController : AServiceController
    {
        public HomeController(HttpClient httpClient, IConfiguration configuration,
            ILogger<HomeController> logger)
            : base(httpClient, configuration)
        { _logger = logger; }

        private readonly ILogger<HomeController> _logger;

        public ActionResult Login()
        {
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AuthLogin login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            HttpRequestMessage request = CreateRequestToService(HttpMethod.Post,
                "api/Users/Login", login);

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch
            {
                ModelState.AddModelError("", "Unexpected server error");
                return View(login);
            }

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // login failed because bad credentials
                    ModelState.AddModelError("", "Login or password incorrect.");
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ModelState.AddModelError("", "User does not exist.");
                }
                else
                {
                    ModelState.AddModelError("", "Unexpected server error");
                }
                return View(login);
            }

            var success = PassCookiesToClient(response);
            if (!success)
            {
                ModelState.AddModelError("", "Unexpected server error");
                return View(login);
            }

            // login success
            return RedirectToAction("UserAccount", "Home");
        }

        [HttpGet]
        [HttpPost]
        public async Task<ActionResult> UserAccount()
        {
            var request = CreateRequestToService(HttpMethod.Get, $"/api/Users/Account");
            var response = await HttpClient.SendAsync(request);

            var jsonString = await response.Content.ReadAsStringAsync();
            ApiUsersModel user = JsonConvert.DeserializeObject<ApiUsersModel>(jsonString);

            UsersViewModel viewModel = new UsersViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PW = user.PW,
                Username = user.Username,
                CreditCardNumber = user.CreditCardNumber,
                PointTotal = user.PointTotal,
                AccountType = user.AccountType
             };

            return View(viewModel);
        }

        [HttpGet]
        [HttpPost]
        public async Task<ActionResult> AccountDetails()
        {
            var request = CreateRequestToService(HttpMethod.Get, $"/api/Users/Account");
            var response = await HttpClient.SendAsync(request);

            var jsonString = await response.Content.ReadAsStringAsync();
            ApiUsersModel user = JsonConvert.DeserializeObject<ApiUsersModel>(jsonString);

            UsersViewModel viewModel = new UsersViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PW = user.PW,
                Username = user.Username,
                CreditCardNumber = user.CreditCardNumber,
                PointTotal = user.PointTotal,
                AccountType = user.AccountType
            };

            return View(viewModel);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            HttpRequestMessage request = CreateRequestToService(HttpMethod.Post,
                "api/Users/Logout");

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (HttpRequestException)
            {
                return View("Error", new ErrorViewModel());
            }

            if (!response.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel());
            }

            var success = PassCookiesToClient(response);
            if (!success)
            {
                return View("Error", new ErrorViewModel());
            }

            // logout success
            return RedirectToAction("Login", "Home");
        }

        // GET: /Home/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Home/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AuthRegister register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            HttpRequestMessage request = CreateRequestToService(HttpMethod.Post,
                "api/Users/Register", register);

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Http request error");
                return View(register);
            }

            if (!response.IsSuccessStatusCode)
            {
                if ((int) response.StatusCode == 403)
                {
                    ModelState.AddModelError("", $"Error api response; username has been used already");
                }
                else
                {
                    ModelState.AddModelError("", $"Error api response");
                }
                return View(register);
            }

            var success = PassCookiesToClient(response);
            if (!success)
            {
                ModelState.AddModelError("", "Cookie error");
                return View(register);
            }

            // login success
            return RedirectToAction("Login", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<ActionResult> Edit(UsersViewModel usersModel)
        {
            HttpRequestMessage request = CreateRequestToService(HttpMethod.Put,
                "api/Users", usersModel);

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (HttpRequestException)
            {
                return View("Error", new ErrorViewModel());
            }

            if (!response.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel());
            }

            // logout success
            return RedirectToAction("UserAccount", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<ActionResult> Delete(UsersViewModel usersModel)
        {
            HttpRequestMessage request = CreateRequestToService(HttpMethod.Delete,
                "api/Users", usersModel);

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (HttpRequestException)
            {
                return View("Error", new ErrorViewModel());
            }

            if (!response.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel());
            }

            var success = PassCookiesToClient(response);
            if (!success)
            {
                ModelState.AddModelError("", "Unexpected server error");
                return View("Error", new ErrorViewModel());
            }

            // logout success
            return RedirectToAction("Login", "Home");
        }

        private bool PassCookiesToClient(HttpResponseMessage apiResponse)
        {
            // the header value contains both the name and value of the cookie itself
            if (apiResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values) &&
                values.FirstOrDefault(x => x.StartsWith(Configuration["ServiceCookieName"])) is string headerValue)
            {
                // copy that cookie to the response we will send to the client
                Response.Headers.Add("Set-Cookie", headerValue);
                return true;
            }
            return false;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
