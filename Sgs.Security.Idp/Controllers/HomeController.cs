using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Security.Idp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Security.Idp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IMapper mapper,ILogger<HomeController> logger) 
            : base(mapper,logger)
        {
        }

        public ActionResult Index()
        {
            ViewData["StatusMessage"] = this.StatusMessage;
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
