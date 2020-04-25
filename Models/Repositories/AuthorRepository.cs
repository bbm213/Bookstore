using Bookstore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Models.Repositories
{
    public class AuthorRepository : IBookStoreRepository<Author>

    {
        IList<Author> authors;
        public AuthorRepository()
        {
            authors = new List<Author>() { 
                new Author{ 
                    Id=1, FulName="Amine"
                
                },
                new Author{
                    Id=2, FulName="Ismahane"

                },
                new Author{
                    Id=3, FulName="Slimane"

                },
            };
        }
        public void Add(Author entity)
        {
            entity.Id = authors.Max(b => b.Id) + 1;
            authors.Add(entity);
        }

        public void Delete(int id)
        {
            var author = Find(id);
            authors.Remove(author);
        }

        public Author Find(int id)
        {
            var author = authors.SingleOrDefault(b=>b.Id==id);
            return author;
        }

        public IList<Author> List()
        {
            return authors;
        }

        public List<Author> Search(string term)
        {
            return authors.Where(a => a.FulName.Contains(term)).ToList();
        }

        public void Update(int id, Author newAuthor)
        {
            var author = Find(id);
            author.FulName = newAuthor.FulName;
        }
    }
}
