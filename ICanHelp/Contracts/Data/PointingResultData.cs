using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class PointingResultData
    {
        public List<string> Labels { get; set; }
        public List<PointingDataSet> datasets { get; set; }
    }

    public class PointingDataSet
    {
        public List<int> data { get; set; }
        public List<string> backgroundColor { get; set; }
    }
}
