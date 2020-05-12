using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class PointingTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public int TotalVoters { get; set; }
        public int VotesRecorded { get; set; }
        public List<User> Users { get; set; }
        public User Owner { get; set; }
        public bool IsComplete { get; set; }
        //public bool IsComplete
        //{
        //    get
        //    {
        //        if (Users.Any() && this.Users.Where(u => u.Vote == 0).Any())
        //            return false;
        //        else
        //            return true;
        //    }
        //}
    }
}
