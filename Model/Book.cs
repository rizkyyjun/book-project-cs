using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookProject.Model
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(100)]
        public string ISBN { get; set; }

        public string Author { get; set; }

        public int? Pages { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Book()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
