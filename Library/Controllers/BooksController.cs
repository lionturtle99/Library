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
    public async Task<ActionResult> Create(Book book)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      book.User = currentUser;
      _db.Books.Add(book);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    
    public ActionResult Details(int id)
    {
      Book thisBook = _db.Books
        .Include(b => b.JoinEntities)
        .ThenInclude(join => join.Patron)
        .FirstOrDefault(b => b.BookId == id);
      Console.WriteLine(thisBook.Title);
      return Json(new {thisBook});
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
      List<Patron> patronList = _db.Patrons.ToList();
      return Json(patronList);
    }

    [HttpPost]
    public ActionResult AddPatron(Patron patron, int bookId)
    {
      if (patron.PatronId != 0)
      {
        _db.BookPatron.Add(new BookPatron() {BookId = bookId, PatronId = patron.PatronId});
      }
      _db.SaveChanges();
      //string message = "SUCCESS";
      // return Json(new { Message = message });
      return RedirectToAction("Index");
    }
  }
}
