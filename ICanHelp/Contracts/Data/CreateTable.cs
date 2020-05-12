using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class CreateTable
    {
        public string TableName { get; set; }
        public string UserName { get; set; }
        public bool IsVoting { get; set; }
    }
}
