using FileUploadProject.Data;
using FileUploadProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileUploadProject.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Images.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Images p)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() == 1)
                {
                    if (files[0].Length <= 3148576)
                    {
                        var ext = Path.GetExtension(files[0].FileName);
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        {
                            using var image = Image.FromStream(files[0].OpenReadStream());
                            using var resized = new Bitmap(image, new Size(256, 256));
                            using var imageStream = new MemoryStream();
                            resized.Save(imageStream, ImageFormat.Jpeg);
                            string fileName = Guid.NewGuid().ToString();
                            var upload = Path.Combine(_environment.WebRootPath, @"images");
                            using (var filesStream = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                            {
                                imageStream.Seek(0L, SeekOrigin.Begin);
                                imageStream.CopyTo(filesStream);
                            }
                            p.ImagePath = @"\images\" + fileName + ext;
                        }
                        else
                        {
                               //ext error
                        }
                    }
                    else
                    {
                            //size error
                    }
                }
                else
                {
                            // file count error
                }
                _context.Add(p);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var images = await _context.Images.FindAsync(id);
            if (images == null)
            {
                return NotFound();
            }
            return View(images);
        }
        [HttpPost]
        public IActionResult Edit(Images p)
        {
            var data = _context.Images.Where(x => x.Id == p.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                data.ImagePath = p.ImagePath;
                if (files.Count() == 1)
                {
                    if (files[0].Length <= 3148576)
                    {
                        var ext = Path.GetExtension(files[0].FileName);
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        {
                            if (data.ImagePath != null)
                            {
                                var imagePath = Path.Combine(_environment.WebRootPath, data.ImagePath.TrimStart('\\'));
                                if (System.IO.File.Exists(imagePath))
                                {
                                    System.IO.File.Delete(imagePath);
                                }
                            }
                            using var image = Image.FromStream(files[0].OpenReadStream());
                            using var resized = new Bitmap(image, new Size(256, 256));
                            using var imageStream = new MemoryStream();
                            resized.Save(imageStream, ImageFormat.Jpeg);
                            string fileName = Guid.NewGuid().ToString();
                            var upload = Path.Combine(_environment.WebRootPath, @"images");
                            using (var filesStream = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                            {
                                imageStream.Seek(0L, SeekOrigin.Begin);
                                imageStream.CopyTo(filesStream);
                            }
                            data.ImagePath = @"\images\" + fileName + ext;
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
                else
                {

                }
                _context.Update(data);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }
        public IActionResult Delete(int id)
        {
            var data = _context.Images.FirstOrDefault(x => x.Id == id);
            if (data.ImagePath != null)
            {
                var imagePath = Path.Combine(_environment.WebRootPath, data.ImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

            }
            _context.Remove(data);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
