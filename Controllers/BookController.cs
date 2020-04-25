using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bookstore.Models;
using Bookstore.Repositories;
using Bookstore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookStoreRepository<Book> bookRepository;
        private readonly IBookStoreRepository<Author> authorRepository;
        private readonly IHostingEnvironment hosting;

        public BookController(IBookStoreRepository<Book> bookRepository, 
            IBookStoreRepository<Author> authorRepository,
            IHostingEnvironment hosting)
        {
            this.bookRepository = bookRepository;
            this.authorRepository = authorRepository;
            this.hosting = hosting;
        }
        // GET: Book
        public ActionResult Index()
        {
            var books = bookRepository.List();
            return View(books);
        }

        // GET: Book/Details/5
        public ActionResult Details(int id)
        {
            var books = bookRepository.Find(id);
            return View(books);
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            return View(GetAllAuthors());
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookAuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string fileName = UploadFile(model.File) ?? string.Empty;
                    
                    if (model.AuthorId == -1)
                    {
                        ViewBag.Message = "Please select an author from th list";

                        return View(GetAllAuthors());
                    }
                    var author = authorRepository.Find(model.AuthorId);

                    Book book = new Book
                    {
                        Id = model.BookId,
                        Title = model.Title,
                        Description = model.Description,
                        Author = author,
                        ImageUrl =fileName
                    };
                    bookRepository.Add(book);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }

            }
            ModelState.AddModelError("", "You have to fill all the required fields");
            return View(GetAllAuthors());

        }

        // GET: Book/Edit/5
        public ActionResult Edit(int id)
        {
            var book = bookRepository.Find(id);
            var authorId = book.Author == null ? book.Author.Id = 0 : book.Author.Id;
            
            var viewModel = new BookAuthorViewModel
            {
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.Author.Id,
                Authors = authorRepository.List().ToList(),
                ImageUrl = book.ImageUrl
            };
            return View(viewModel);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BookAuthorViewModel viewModel)
        {
            try
            {
                string fileName = UploadFile(viewModel.File, viewModel.ImageUrl);
          
                var author = authorRepository.Find(viewModel.AuthorId);

                Book book = new Book
                {
                    Id = viewModel.BookId,
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Author = author,
                    ImageUrl = fileName
                };
                bookRepository.Update(viewModel.BookId,book);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int id)
        {
            var book =bookRepository.Find(id);
            return View(book);
        }
        
        // POST: Book/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            try
            {
                bookRepository.Delete(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        List<Author> FilleSelectList()
        {
            var author = authorRepository.List().ToList();
            author.Insert(0,new Author {Id=-1, FulName="Please select an author" });
            return author;
        
        }
        BookAuthorViewModel GetAllAuthors()
        {
            var vmodel = new BookAuthorViewModel
            {
                Authors = FilleSelectList()
            };
            return vmodel;
        }

        string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "uploads");
                string fullPath = Path.Combine(uploads, file.FileName);
                file.CopyTo(new FileStream(fullPath, FileMode.Create));
                return file.FileName;
            }
            return null;
        }
        string UploadFile(IFormFile file, string imageUrl)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "uploads");
                
                string newPath = Path.Combine(uploads, file.FileName);
                string oldPath = Path.Combine(uploads, imageUrl);
                
                if (oldPath != newPath)
                {
                    //Delete the old file
                    System.IO.File.Delete(oldPath);
                    //Save the new File   
                    file.CopyTo(new FileStream(newPath, FileMode.Create));
                }
                
                return file.FileName;
            }
            
            return imageUrl;
        }

        public ActionResult Search(string term)
        {
            var result = bookRepository.Search(term);

            return View("Index",result);

        }

    }
}