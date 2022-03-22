using System.Collections.Generic;

namespace Library.Models
{
  public class Patron
  {
    public Patron()
    {
      this.JoinEntities = new HashSet<BookPatron>();
      // this.History = new HashSet<BookPatron>();
    }
    
    public int PatronId {get; set;}
    public string Name {get; set;}
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<BookPatron> JoinEntities { get; set; }
    // public virtual ICollection<BookPatron> History { get; set; }
  }
}