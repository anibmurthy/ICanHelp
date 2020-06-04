using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class RetroBoard
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public Heading Column1 { get; set; }
        public Heading Column2 { get; set; }
        public Heading Column3 { get; set; }
        public Heading Column4 { get; set; }

    }

    public class Heading
    {
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
    }
}
