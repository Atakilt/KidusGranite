using System.Collections.Generic;
using KidusGranite.Data;
using KidusGranite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using KidusGranite.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace KidusGranite.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("[action]")]
        public IActionResult Index()
        {         
            IEnumerable<Product> products = _context.Products.Include(c => c.Category).Include(a=>a.ApplicationType);
            return View(products);
        }

        [HttpGet("[action]")]
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> categoryDropDown = _context.Categories.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            //ViewBag.CategoryDropDown = categoryDropDown;

            //Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product=new Product(),
                CategorySelectList = _context.Categories.Select(i=>new SelectListItem
                {
                    Text=i.Name,
                    Value=i.Id.ToString()
                }),
                ApplicationTypeSelectList = _context.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                //this is for create                
                return View(productVM);
            }
            else
            {
                productVM.Product = _context.Products.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(productVM.Product.Id==0)
                {
                    //creating
                    string upload = webRootPath + WebConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using(var fileStream = new FileStream(Path.Combine(upload,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream); 
                    }

                    productVM.Product.Image = fileName + extension;
                    _context.Products.Add(productVM.Product);                    
                }
                else
                {
                    //updating
                    var objFromDb = _context.Products.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _context.Products.Update(productVM.Product);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _context.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.ApplicationTypeSelectList = _context.ApplicationTypes.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(productVM);
        }

        [HttpGet("[action]")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }            
            
            Product product = _context.Products.Include(u=>u.Category)
                                               .Include(u=>u.ApplicationType)
                                               .FirstOrDefault(u=>u.Id==id);
                       
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost("[action]"),ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;            

            var oldFile = Path.Combine(upload, product.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}