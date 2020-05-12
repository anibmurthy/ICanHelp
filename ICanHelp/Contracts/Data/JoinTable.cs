using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class JoinTable
    {
        public string UserName { get; set; }
        public int TableId { get; set; }
        public bool IsVoting { get; set; }
    }
}
