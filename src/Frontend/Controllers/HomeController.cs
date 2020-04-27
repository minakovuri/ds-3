using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Frontend.Models;
using BackendApi;
using Grpc.Net.Client;

namespace Frontend.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        async public Task<IActionResult> HandleAddTaskRequest(String description)
        {
            using var channel = GrpcChannel.ForAddress("http://" + Environment.GetEnvironmentVariable("BACKEND_API_HOST") + ":" + Environment.GetEnvironmentVariable("BACKEND_API_PORT"));
            var client = new Job.JobClient(channel);
            var reply = await client.RegisterAsync(new RegisterRequest { Description = description });

            ViewBag.Id = reply.Id;
            ViewBag.Description = description;

            return View("Task");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
