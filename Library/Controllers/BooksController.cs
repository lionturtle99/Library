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
using System;

namespace Library.Controllers
{
  // [Authorize]
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
    public async Task<ActionResult> Create(Book book)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      book.User = currentUser;
      _db.Books.Add(book);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [AllowAnonymous]
    public JsonResult Details(int id)
    {
      //Book thisBook = _db.Books.FirstOrDefault(b => b.BookId == id);
      IEnumerable<Book> thisBook = new List<Book>();
      thisBook = _db.Books.Where(b => b.BookId == id).Select(x =>
                  new Book()
                  {
                    Title = x.Title,
                    BookId = x.BookId,
                    Genre = x.Genre,
                    Author = x.Author,
                    Pages = x.Pages
                  });
      var listOfJoins = _db.BookPatron.ToList().Where(b => b.BookId == id);
      // Console.WriteLine("First: " + listOfJoins.Count());
      var listOfRenters = new List<Patron>{};
      foreach (var join in listOfJoins) {
        // Console.WriteLine("Second: " + join.Patron.Name);
        listOfRenters.Add(
                new Patron() {
                  Name = join.Patron.Name,
                  PatronId = join.Patron.PatronId
                }
          );
      }
      return Json(new {thisBook = thisBook, listOfRenters = listOfRenters});
      // return View(thisBook);
    }

    [HttpPost]
    public ActionResult Delete(int bookId)
    {
      var thisBook = _db.Books.FirstOrDefault(b => b.BookId == bookId);
      _db.Books.Remove(thisBook);
      _db.SaveChanges();
      string message = "SUCCESS";
      return Json(new { Message = message });
    }

    public ActionResult Edit(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      return View(thisBook);
    }
    [HttpPost]
    public ActionResult Edit(Book book)
    {
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      string message = "SUCCESS";
      return Json(new { Message = message });
    }

    public JsonResult GetPatrons()
    {
      IEnumerable<Patron> jsonPatron = new List<Patron>();
      jsonPatron = _db.Patrons.ToList().Select(x =>
                  new Patron()
                  {
                    Name = x.Name,
                    PatronId = x.PatronId
                  });
      return Json(jsonPatron);
    }

    [HttpPost]
    public ActionResult RentToPatron(int patronId, int bookId)
    {
      Patron thisPatron = _db.Patrons.FirstOrDefault(book => book.PatronId == patronId);
      if (thisPatron.PatronId != 0)
      {
        _db.BookPatron.Add(new BookPatron() {BookId = bookId, PatronId = thisPatron.PatronId});
      }
      _db.SaveChanges();
      string message = "SUCCESS";
      return Json(new { Message = message, thisPatron = thisPatron });
      // return RedirectToAction("Index");
    }

  }
}
