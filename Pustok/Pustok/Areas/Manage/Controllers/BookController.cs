using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModel;
using Pustok.DAL;
using Pustok.Helper;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context,IWebHostEnvironment env)
        {
            this._context = context;
            this._env = env;
        }
        public IActionResult Index()
        {
            BookViewModel bookVW = new BookViewModel
            {
                Books = _context.Book.Include(x => x.Author).Include(x=>x.Genre).ToList(),

            };
            return View(bookVW);
        }
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View();

        }
        [HttpPost]
        public IActionResult Create(Books book)
        {
            
            if (!ModelState.IsValid)
            {
                return View();
            }


            if (!_context.Authors.Any(x=>x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "This author is not exists");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }
            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("AuthorId", "This author is not exists");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }

            if (book.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "Post image is required");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();

            }
            else
            {
                if (book.PosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFiles", "File size must be less than 2MB");
                }
                if (book.PosterFile.ContentType != "image/png" && book.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFiles", "File format must be image/png or image/jpeg");
                }
                if (!ModelState.IsValid)
                {

                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Genres = _context.Genres.ToList();
                    return View();
                }
                BookImage bookImg = new BookImage
                {
                    Name = FileManager.Save(_env.WebRootPath, "upload/book", book.PosterFile),
                    PosterStatus = true
                };
                book.BookImages.Add(bookImg);
            }



            if (book.HoverFile == null)
            {
                ModelState.AddModelError("HoverFile","Hover image is required");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }
            else
            {
                if (book.HoverFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFiles", "File size must be less than 2MB");
                }
                if (book.HoverFile.ContentType != "image/png" && book.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFiles", "File format must be image/png or image/jpeg");
                }
                if (!ModelState.IsValid)
                {

                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Genres = _context.Genres.ToList();
                    return View();
                }
                BookImage bookImg = new BookImage 
                { 
                    Name= FileManager.Save(_env.WebRootPath,"upload/book",book.HoverFile),
                    PosterStatus=false,
                };
                book.BookImages.Add(bookImg);
            }

            if (book.ImageFiles != null)
            {
                foreach (var file in book.ImageFiles)
                {
                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "File size must be less than 2MB");
                    }
                    if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", "File format must be image/png or image/jpeg");
                    }
                    if (!ModelState.IsValid)
                    {
                        ViewBag.Authors = _context.Authors.ToList();
                        ViewBag.Genres = _context.Genres.ToList();
                        return View();
                    }
                }

                foreach (var file in book.ImageFiles)
                {
                    BookImage bookImg = new BookImage
                    {
                        Name = FileManager.Save(_env.WebRootPath, "upload/book", file),
                        PosterStatus = null
                    };
                    book.BookImages.Add(bookImg);
                }
            }
    
           
            _context.Book.Add(book);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int Id) 
        {
            Books isExists = _context.Book.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == Id);

            if (isExists == null)
            {
                return RedirectToAction("error", "home");
            }
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View(isExists);
        }
        /*public IActionResult Edit(Books book) 
        {
            Books isExists = _context.Book.Include(x =>x.BookImages).FirstOrDefault(x => x.Id == book.Id);
            if (isExists == null)
            {
                return RedirectToAction("error", "home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }


            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "This author is not exists");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }
            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("AuthorId", "This author is not exists");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }

            if (book.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "Post image is required");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();

            }
            else
            {
                if (book.PosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFiles", "File size must be less than 2MB");
                }
                if (book.PosterFile.ContentType != "image/png" && book.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFiles", "File format must be image/png or image/jpeg");
                }
                if (!ModelState.IsValid)
                {

                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Genres = _context.Genres.ToList();
                    return View();
                }
                BookImage bookImg = new BookImage
                {
                    Name = FileManager.Save(_env.WebRootPath, "upload/book", book.PosterFile),
                    PosterStatus = true
                };
                book.BookImages.Add(bookImg);
            }



            if (book.HoverFile == null)
            {
                ModelState.AddModelError("HoverFile", "Hover image is required");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }
            else
            {
                if (book.HoverFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFiles", "File size must be less than 2MB");
                }
                if (book.HoverFile.ContentType != "image/png" && book.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFiles", "File format must be image/png or image/jpeg");
                }
                if (!ModelState.IsValid)
                {

                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Genres = _context.Genres.ToList();
                    return View();
                }
                BookImage bookImg = new BookImage
                {
                    Name = FileManager.Save(_env.WebRootPath, "upload/book", book.HoverFile),

                    PosterStatus = false,
                };
                foreach (var bk in isExists.BookImages)
                {
                    if (bk.PosterStatus == false)
                    {
                        FileManager.Delete(_env.WebRootPath, "upload/slider", bk.Name);
                    }
                    bk.Name = bookImg.Name;
                    bk.PosterStatus = bookImg.PosterStatus;
                }
                
                
            }

            if (book.ImageFiles != null)
            {
                foreach (var file in book.ImageFiles)
                {
                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "File size must be less than 2MB");
                    }
                    if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", "File format must be image/png or image/jpeg");
                    }
                    if (!ModelState.IsValid)
                    {
                        ViewBag.Authors = _context.Authors.ToList();
                        ViewBag.Genres = _context.Genres.ToList();
                        return View();
                    }
                }

                foreach (var file in book.ImageFiles)
                {
                    BookImage bookImg = new BookImage
                    {
                        Name = FileManager.Save(_env.WebRootPath, "upload/book", file),
                        PosterStatus = null
                    };
                    *//*FileManager.Delete(_env.WebRootPath, "upload/slider", isExis);*//*
                    book.BookImages.Add(bookImg);
                }
            }

            _context.SaveChanges();
            return RedirectToAction("index");
        }*/
    }
}
