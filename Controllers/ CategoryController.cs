using System.Collections.Generic;
using System.Net.Mime;
using KidusGranite.Data;
using KidusGranite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KidusGranite.Controllers
{
    [Route("[controller]")]
    public class  CategoryController : Controller
    {
        private readonly ILogger< CategoryController> _logger;
        private readonly ApplicationDbContext _context;

        public  CategoryController(ILogger< CategoryController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories= _context.Categories;
            return View(categories);
        }       
    }
}