using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Library.Models;
using System.Linq;
using System;

namespace Library.Controllers
{
  public class HomeController : Controller
  {
    private readonly LibraryContext _db;

    public HomeController(LibraryContext db)
    {
      _db = db;
    }

    [HttpGet("/")]
    public ActionResult Index()
    {
      return View();
    }

  }
}