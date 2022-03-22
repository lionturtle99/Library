using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Library.Controllers
{
  [Authorize]
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }
    [AllowAnonymous]
    public ActionResult Index()
    {
      List<Book> model = _db.Books.OrderBy(b => b.Title).ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Book book)
    {
      _db.Books.Add(book);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    // public ActionResult Details(int id)
    // {
    //     var thisBook = _db.Books
    //         .Include(book => book.JoinEntities)
    //         .ThenInclude(join => join.Patron)
    //         .FirstOrDefault(book => book.BookId == id);
    //     return View(thisBook);
    // }
    
    public JsonResult Details(int id)
    {
        var thisBook = _db.Books
            .Include(book => book.JoinEntities)
            .ThenInclude(join => join.Patron)
            .FirstOrDefault(book => book.BookId == id);
      // return Json(new { bookTitle = thisBook.Title, bookGenre = thisBook.Genre, bookPages = thisBook.Pages, bookId = thisBook.BookId }); 
      return Json(thisBook);
      
      
    }

    public ActionResult Delete(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(b => b.BookId == id);
      return View(thisBook);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(b => b.BookId == id);
      _db.Books.Remove(thisBook);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Edit(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      return View(thisBook);
    }
    [HttpPost]
    public IActionResult Edit(string title, string genre, int pages, int bookId)
    {
      //Book thisBook = _db.Books.FirstOrDefault(b => b.BookId == bookId);
      Book thisBook = new Book();
      thisBook.BookId = bookId;
      thisBook.Genre = genre;
      thisBook.Pages = pages;
      thisBook.Title = title;
      // _db.Entry(thisBook).State = EntityState.Modified;
      _db.Update(thisBook);
      _db.SaveChanges();
      string message = "SUCCESS";  
      return Json(new {Message = message});
    }
  }
}

              // Title:$("#bookTitleInput").val(),
//               Genre: $("#bookGenre").text(),
//               Pages: $("#bookPages").text(),
//               BookId: $("#bookId").val()