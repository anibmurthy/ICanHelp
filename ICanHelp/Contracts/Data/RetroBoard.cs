using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    [Table("RetroBoards")]
    public class RetroBoard
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [StringLength(25)]
        public string TeamName { get; set; }
        [StringLength(500)]
        public Heading Column1 { get; set; }
        [StringLength(500)]
        public Heading Column2 { get; set; }
        [StringLength(500)]
        public Heading Column3 { get; set; }
        [StringLength(500)]
        public Heading Column4 { get; set; }

    }

    public class Heading
    {
        public int HeadingId { get; set; }
        public Heading(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        [ForeignKey("HeadingId")]
        public int HeadingId { get; set; }
    }
}
