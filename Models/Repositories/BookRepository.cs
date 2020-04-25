using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Repositories
{
    public class BookRepository : IBookStoreRepository<Book>
    {
        List<Book> books;
        public BookRepository()
        {
            books = new List<Book>() {
               new Book
               {
                   Id=1,
                   Title="C programming Language",
                   Description="DevProg",
                   ImageUrl="c.png",
                   Author =new Author()
               },
               new Book
               {
                   Id=2,
                   Title="Java programming Language",
                   Description="DevProg",
                   ImageUrl="java.png",
                   Author =new Author()
               },
               new Book
               {
                   Id=3,
                   Title="R programming Language",
                   Description="DevProg",
                   ImageUrl="r.png",
                   Author =new Author()
               }


            };
        }
        public void Add(Book entity)
        {
            entity.Id = books.Max(b => b.Id) + 1;
            books.Add(entity);
        }

        public void Delete(int id)
        {
            var book = Find(id);
            books.Remove(book);
        }

        public Book Find(int id)
        {
            var book = books.SingleOrDefault(b=>b.Id==id);
            return book;
        }

        public IList<Book> List()
        {
            return books;
        }

        public List<Book> Search(string term)
        {
            return books.Where(a => a.Title.Contains(term)).ToList();
        }

        public void Update(int id,Book newBook)
        {
            var book = Find(id);
            book.Title = newBook.Title;
            book.Description = newBook.Description;
            book.Author = newBook.Author;
            book.ImageUrl = newBook.ImageUrl;
        }
    }
}
