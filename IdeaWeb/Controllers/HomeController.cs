using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdeaWeb.Models;
using IdeaWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IdeaContext _context;

        public HomeController(IdeaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ideas = await _context.Ideas.ToListAsync();
            return View(ideas);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
