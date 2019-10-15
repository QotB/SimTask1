using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class Server
    {
        public Server()
        {
            this.TimeDistribution = new List<TimeDistribution>();
        }
        
        public int ID { get; set; }
        public decimal IdleProbability { get; set; }
        public decimal AverageServiceTime { get; set; } 
        public decimal Utilization { get; set; }

        public List<TimeDistribution> TimeDistribution;

        //optional if needed use them
        public int FinishTime { get; set; }
        public int TotalWorkingTime { get; set; }
        public decimal customers { get; set; }
        public decimal idle { get; set; }
        public Server(int id)
        {
            ID = id;
            IdleProbability = 0;
            AverageServiceTime = 0;
            Utilization = 0;
            FinishTime = 0;
            TotalWorkingTime = 0 ;
            this.TimeDistribution = new List<TimeDistribution>();
            customers = 0;
            idle = 0;
        }
    }
}
