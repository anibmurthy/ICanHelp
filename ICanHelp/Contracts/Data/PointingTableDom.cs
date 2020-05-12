using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class PointingTableDom
    {
        public PointingTableDom(PointingTable data)
        {
            Id = data.Id;
            Name = data.Name;
            Description = data.Description;
            TotalVoters = data.TotalVoters;
            VotesRecorded = data.VotesRecorded;
            Users = new List<User>();
            Users.AddRange(data.Users);
            Owner = data.Owner;
            IsComplete = data.IsComplete;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public int TotalVoters { get; set; }
        public int VotesRecorded { get; set; }
        public List<User> Users { get; set; }
        public User Owner { get; set; }
        public bool IsComplete { get; set; }
        public User CurrentUser { get; set; }
    }
}
