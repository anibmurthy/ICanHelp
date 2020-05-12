using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class User
    {
        public int Vote { get; set; }
        public bool IsOwner { get; set; }
        public bool ShowPoints { get; set; }
        public bool ClearPoints { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVoting { get; set; }
        public int TableId { get; set; }
    }
}
