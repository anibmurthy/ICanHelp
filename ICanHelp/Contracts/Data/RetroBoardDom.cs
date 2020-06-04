using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class RetroBoard1 : RetroCreate
    {
        public int Id { get; set; }
        public int DivCount { get; set; }
        public Dictionary<string, List<string>> Divisions { get; set; } = new Dictionary<string, List<string>>();
    }

    public class RetroCreate
    {
        public string TeamName { get; set; }
        public List<string> Headings { get; set; }
    }
}
