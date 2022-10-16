using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KidusGranite.Models;
using KidusGranite.Data;

namespace KidusGranite.Controllers
{
    [Route("[controller]")]
    public class ApplicationTypeController : Controller
    {
        private readonly ILogger<ApplicationTypeController> _logger;
        private readonly ApplicationDbContext _context;

        public ApplicationTypeController(ApplicationDbContext context, ILogger<ApplicationTypeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("[action]")]
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> applicationTypes = _context.ApplicationTypes;
            return View(applicationTypes);
        }

        [HttpGet("[action]")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            _context.ApplicationTypes.Add(applicationType);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet("[action]")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var applicationType = _context.ApplicationTypes.Find(id);
            if (applicationType == null)
            {
                return NotFound();
            }

            return View(applicationType);
        }

        [HttpGet("[action]")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var applicationType = _context.ApplicationTypes.Find(id);
            if (applicationType == null)
            {
                return NotFound();
            }

            return View(applicationType);
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var applicationType = _context.ApplicationTypes.Find(id);
            if (applicationType == null)
            {
                return NotFound();
            }

            _context.ApplicationTypes.Remove(applicationType);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                _context.ApplicationTypes.Update(applicationType);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationType);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}