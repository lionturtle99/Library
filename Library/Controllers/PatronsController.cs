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
  public class PatronsController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatronsController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }
    
    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userPatrons = _db.Patrons.Where(entry => entry.User.Id == currentUser.Id).ToList();
      ViewBag.PageTitle = "Patrons";
      return View(userPatrons);
    }
    public ActionResult Create()
    {
      ViewBag.PageTitle = "Create a Patron";
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create (Patron patron)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      patron.User = currentUser;
      _db.Patrons.Add(patron);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      ViewBag.PageTitle = "Patron details";
      var thisPatron = _db.Patrons
        .Include(patron => patron.JoinEntities)
        .ThenInclude(join => join.Book)
        .FirstOrDefault(patron => patron.PatronId == id);
      return View(thisPatron);
    }
    public ActionResult Edit(int id)
    {
      ViewBag.PageTitle = "Edit Patron Details";
      var thisPatron = _db.Patrons.FirstOrDefault(patron => patron.PatronId == id);
      return View(thisPatron);
    }
    [HttpPost]
    public ActionResult Edit(Patron patron)
    {
      _db.BookPatron.Add(new BookPatron() {PatronId = patron.PatronId});
      _db.Entry(patron).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult AddBook(int id)
    {
      var thisPatron = _db.Patrons.FirstOrDefault(patron => patron.PatronId == id);
      // ViewBag.BookId = new SelectList(_db.Books, "BookId", "Name");
      ViewBag.Books = _db.Books.ToList();
      ViewBag.PageTitle = ("Rent a book for " + thisPatron.Name);
      return View(thisPatron);
    }
    [HttpPost]
    public ActionResult AddBook(int PatronId, int BookId)
    {
      if (BookId != 0)
      {
        _db.BookPatron.Add(new BookPatron() {BookId = BookId, PatronId = PatronId});
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult Delete (int id)
    {
      
      var thisPatron = _db.Patrons.FirstOrDefault(patron => patron.PatronId == id);
      ViewBag.PageTitle = "Delete patron";
      return View(thisPatron);
    }
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisPatron = _db.Patrons.FirstOrDefault(patron => patron.PatronId == id);
      _db.Patrons.Remove(thisPatron);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    [HttpPost]
    public ActionResult DeleteBook (int joinId)
    {
      ViewBag.PageTitle = "Remove books";
      var joinEntry = _db.BookPatron.FirstOrDefault(entry => entry.BookPatronId == joinId);
      _db.BookPatron.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}

// dotnet publish -c Release