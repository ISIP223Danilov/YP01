using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YP01
{
    [Table("BookGenreMap")]
    public class BookGenreMap
    {
        [Key]
        [Column(Order = 0)]
        public int BookId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int GenreId { get; set; }

        [ForeignKey("BookId")]
        public virtual LibraryBooks LibraryBooks { get; set; }

        [ForeignKey("GenreId")]
        public virtual GenreCatalog GenreCatalog { get; set; }
    }
}