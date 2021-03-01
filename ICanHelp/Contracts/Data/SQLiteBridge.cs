using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class SQLiteBridge
    {
        [Key]
        public int BoardId { get; set; }
        [StringLength(15000)]
        public string Data { get; set; }
    }
}
