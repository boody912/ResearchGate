using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResearchGateProject.Controllers
{
    public class LikeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
