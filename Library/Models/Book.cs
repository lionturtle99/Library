using System.Collections.Generic;

namespace Library.Models
{
  public class Book
  {
    public Book()
    {
      this.JoinEntities = new HashSet<BookPatron>();
      // this.History = new HashSet<BookPatron>();
    }

    public string Title { get; set; }
    public string Genre { get; set; }
    public int Pages { get; set; }
    public int BookId { get; set; }
    public virtual ApplicationUser User {get; set;}
    public virtual ICollection<BookPatron> JoinEntities { get; set; }
    // public virtual ICollection<BookPatron> History { get; set; }
  }
}