using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class TimeDistribution
    {
        public int Time { get; set; }
        public decimal Probability { get; set; }
        public decimal CummProbability { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
       public TimeDistribution(int t, decimal p)
        {
            Time = t;
            Probability = p;
            CummProbability = 0;
            MinRange = 0;
            MaxRange = 0;
        }
        ~TimeDistribution()
        { }
    }
}
