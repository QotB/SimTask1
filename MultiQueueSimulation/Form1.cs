using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace MultiQueueSimulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Form1_Load();
        }
        public static SimulationSystem sys;
        public static PerformanceMeasures per;
        public static decimal total_run = 0;
        public void Form1_Load()
        {
            init();
            read_file();
            build_tables();
            start();
            dataGridView1.DataSource = sys.SimulationTable;
            dataGridView2.DataSource = sys.InterarrivalDistribution;
            string test = TestingManager.Test(sys, Constants.FileNames.TestCase1);
            MessageBox.Show(test);

        }
        void start()
        {
            int cid = 0;
            per.AverageWaitingTime = 0;
            per.WaitingProbability = 0;
            per.MaxQueueLength = 0;
            int queue = 0;
            List<int> queu = new List<int>();

            Random rnd = new Random();
            Random rnd2 = new Random();
            while (true)
            {
                cid++;
                if (sys.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
                {
                    if (stop(cid))
                        break;
                }
                SimulationCase cas = new SimulationCase(cid);
                cas.RandomInterArrival = rnd.Next(1, 101);
                cas.InterArrival = get1(cas.RandomInterArrival);
                if (cid == 1)
                {
                    cas.ArrivalTime = 0;
                }
                else
                {
                    cas.ArrivalTime = cas.InterArrival + sys.SimulationTable[cid - 2].ArrivalTime;
                }
                if (sys.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
                {
                    if (stop(cas.ArrivalTime)) break;
                }
                cas.RandomService = rnd2.Next(1, 101);
                cas.TimeInQueue = -1;
                if (sys.SelectionMethod == Enums.SelectionMethod.HighestPriority)
                {
                    for (int i = 0; i < sys.NumberOfServers; i++)
                    {
                        if (sys.Servers[i].FinishTime <= cas.ArrivalTime)
                        {
                            sys.Servers[i].idle += Math.Abs(cas.ArrivalTime - sys.Servers[i].FinishTime);
                            cas.ServiceTime = get2(cas.RandomService, sys.Servers[i]);
                            cas.TimeInQueue = 0;
                            sys.Servers[i].FinishTime = cas.ArrivalTime + cas.ServiceTime;
                            cas.StartTime = cas.ArrivalTime;
                            cas.EndTime = cas.ArrivalTime + cas.ServiceTime;
                            cas.AssignedServer = sys.Servers[i];
                            sys.Servers[i].TotalWorkingTime += cas.ServiceTime;
                            sys.Servers[i].customers++;
                            queue = 0;
                            cas.idd = sys.Servers[i].ID;

                            break;
                        }
                    }
                    if (cas.TimeInQueue == -1)
                    {
                        for (int i = 0; i < queu.Count; i++)
                        {
                            if (cas.ArrivalTime >= queu[i])
                                queu.RemoveAt(i);
                        }
                        int mn = 1000000000, ind = 0;
                        queue++;
                        for (int i = 0; i < sys.NumberOfServers; i++)
                        {
                            if (sys.Servers[i].FinishTime < mn)
                            {
                                mn = sys.Servers[i].FinishTime;
                                ind = i;
                            }
                        }
                        queu.Add(mn);
                        cas.ServiceTime = get2(cas.RandomService, sys.Servers[ind]);
                        cas.TimeInQueue = sys.Servers[ind].FinishTime - cas.ArrivalTime;
                        sys.Servers[ind].FinishTime = mn + cas.ServiceTime;
                        cas.StartTime = mn;
                        cas.EndTime = mn + cas.ServiceTime;
                        cas.AssignedServer = sys.Servers[ind];
                        per.AverageWaitingTime += cas.TimeInQueue;
                        per.WaitingProbability++;
                        sys.Servers[ind].TotalWorkingTime += cas.ServiceTime;
                        sys.Servers[ind].customers++;
                        cas.idd = sys.Servers[ind].ID;

                    }
                    per.MaxQueueLength = Math.Max(per.MaxQueueLength, queu.Count);
                }
                else if (sys.SelectionMethod == Enums.SelectionMethod.LeastUtilization)
                {
                    List<Server> sers = new System.Collections.Generic.List<Server>();
                    decimal mn = 10000000;
                    int mnn = 10000000;
                    foreach (Server ser in sys.Servers)
                    {
                        ser.Utilization = (decimal)(ser.TotalWorkingTime);
                        if (ser.FinishTime <= cas.ArrivalTime)
                        {
                            mn = Math.Min(mn, ser.Utilization);
                            sers.Add(ser);
                        }
                        mnn = Math.Min(mnn, ser.FinishTime);

                    }
                    if (sers.Count > 0)
                    {
                        int ind = 0;
                        foreach (Server ser in sers)
                        {
                            if (ser.Utilization == mn)
                            {
                                ind = ser.ID - 1;
                                break;
                            }
                        }
                        sys.Servers[ind].idle += Math.Abs(cas.ArrivalTime - sys.Servers[ind].FinishTime);
                        cas.ServiceTime = get2(cas.RandomService, sys.Servers[ind]);
                        cas.TimeInQueue = 0;
                        sys.Servers[ind].FinishTime = cas.ArrivalTime + cas.ServiceTime;
                        cas.StartTime = cas.ArrivalTime;
                        cas.EndTime = cas.ArrivalTime + cas.ServiceTime;
                        cas.AssignedServer = sys.Servers[ind];
                        sys.Servers[ind].TotalWorkingTime += cas.ServiceTime;
                        sys.Servers[ind].customers++;
                        cas.idd = sys.Servers[ind].ID;

                        queue = 0;
                    }
                    else
                    {
                        sers = new System.Collections.Generic.List<Server>();
                        mn = 10000000;
                        foreach (Server ser in sys.Servers)
                        {
                            if (ser.FinishTime == mnn)
                            {
                                mn = Math.Min(mn, ser.Utilization);
                                sers.Add(ser);
                            }
                        }
                        int ind = 0;
                        foreach (Server ser in sers)
                        {
                            if (ser.Utilization == mn)
                            {
                                ind = ser.ID - 1;
                                break;
                            }
                        }
                        for (int i = 0; i < queu.Count; i++)
                        {
                            if (cas.ArrivalTime >= queu[i])
                                queu.RemoveAt(i);
                        }

                        queu.Add(mnn);
                        cas.ServiceTime = get2(cas.RandomService, sys.Servers[ind]);
                        cas.TimeInQueue = sys.Servers[ind].FinishTime - cas.ArrivalTime;
                        sys.Servers[ind].FinishTime = mnn + cas.ServiceTime;
                        cas.StartTime = mnn;
                        cas.EndTime = mnn + cas.ServiceTime;
                        cas.AssignedServer = sys.Servers[ind];
                        per.AverageWaitingTime += cas.TimeInQueue;
                        per.WaitingProbability++;
                        sys.Servers[ind].TotalWorkingTime += cas.ServiceTime;
                        sys.Servers[ind].customers++;
                        cas.idd = sys.Servers[ind].ID;
                    }
                    per.MaxQueueLength = Math.Max(per.MaxQueueLength, queu.Count);

                }

                sys.SimulationTable.Add(cas);
                total_run = Math.Max(total_run, cas.EndTime);

            }
            per.WaitingProbability = per.WaitingProbability / (sys.SimulationTable.Count);
            per.AverageWaitingTime = per.AverageWaitingTime / (sys.SimulationTable.Count);
            sys.PerformanceMeasures = per;
            get_per();

        }
        public void get_per() // calculate performance of servers
        {
            foreach (Server ser in sys.Servers)
            {
                ser.AverageServiceTime = (decimal)(ser.TotalWorkingTime) / ser.customers;
                ser.IdleProbability = ((total_run - ser.FinishTime) + (decimal)ser.idle) / total_run;
                ser.Utilization = (decimal)(ser.TotalWorkingTime) / total_run;
            }

        }
        public int get1(int x) // get arrival time for customers
        {
            foreach (TimeDistribution tim in sys.InterarrivalDistribution)
            {
                if (x >= tim.MinRange && x <= tim.MaxRange)
                {
                    return tim.Time;
                }
            }
            return 0;
        }
        public int get2(int x, Server ser) // get arrival time for servers
        {
            foreach (TimeDistribution tim in ser.TimeDistribution)
            {
                if (x >= tim.MinRange && x <= tim.MaxRange)
                {
                    return tim.Time;
                }
            }
            return 0;
        }
        public bool stop(int x)
        {
            if (x <= sys.StoppingNumber) return false;
            return true;
        }
        public void init()
        {
            sys = new SimulationSystem();
            per = new PerformanceMeasures();
            total_run = 0;
        }
        public void build_tables()
        {
            // interarrival table ...
            sys.InterarrivalDistribution[0].CummProbability = sys.InterarrivalDistribution[0].Probability;
            sys.InterarrivalDistribution[0].MinRange = 1;
            sys.InterarrivalDistribution[0].MaxRange = (int)(sys.InterarrivalDistribution[0].Probability * 100);
            for (int i = 1; i < 4; i++)
            {
                sys.InterarrivalDistribution[i].CummProbability =
                    sys.InterarrivalDistribution[i - 1].CummProbability + sys.InterarrivalDistribution[i].Probability;
                sys.InterarrivalDistribution[i].MinRange = sys.InterarrivalDistribution[i - 1].MaxRange + 1;
                sys.InterarrivalDistribution[i].MaxRange = (int)(sys.InterarrivalDistribution[i].CummProbability * 100);
            }

            // systems table 
            for (int ii = 0; ii < sys.NumberOfServers; ii++)
            {
                sys.Servers[ii].TimeDistribution[0].CummProbability = sys.Servers[ii].TimeDistribution[0].Probability;
                sys.Servers[ii].TimeDistribution[0].MinRange = 1;
                sys.Servers[ii].TimeDistribution[0].MaxRange = (int)(sys.Servers[ii].TimeDistribution[0].Probability * 100);
                for (int i = 1; i < 4; i++)
                {
                    sys.Servers[ii].TimeDistribution[i].CummProbability =
                         sys.Servers[ii].TimeDistribution[i - 1].CummProbability + sys.Servers[ii].TimeDistribution[i].Probability;
                    sys.Servers[ii].TimeDistribution[i].MinRange = sys.Servers[ii].TimeDistribution[i - 1].MaxRange + 1;
                    sys.Servers[ii].TimeDistribution[i].MaxRange = (int)(sys.Servers[ii].TimeDistribution[i].CummProbability * 100);
                }
            }


        }
        public void read_file()
        {
            string[] lines = System.IO.File.ReadAllLines("TestCase1.txt"); // read lines 
            sys.NumberOfServers = int.Parse(lines[1]);
            sys.StoppingNumber = int.Parse(lines[4]);
            if (lines[7] == "1")
            {
                sys.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
            }
            else
            {
                sys.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;
            }
            if (lines[10] == "1")
            {
                sys.SelectionMethod = Enums.SelectionMethod.HighestPriority;
            }
            else if (lines[10] == "2")
            {
                sys.SelectionMethod = Enums.SelectionMethod.Random;
            }
            else
            {
                sys.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
            }
            int i = 13;
            for (; true; i++)
            {
                if (lines[i] == "")
                    break;
                string[] line = lines[i].Split(',', ' ');
                int t = int.Parse(line[0]);
                decimal p = decimal.Parse(line[2]);
                TimeDistribution tim = new TimeDistribution();
                tim.Time = t;
                tim.Probability = p;
                sys.InterarrivalDistribution.Add(tim);
            }
            for (int j = i + 2, ii = 1; ii <= sys.NumberOfServers; j += 6, ii++)
            {
                int a = j;
                Server ser = new Server(ii);

                for (int k = a; k < a + 4; k++)
                {
                    string[] line = lines[k].Split(',', ' ');
                    int t = int.Parse(line[0]);
                    decimal p = decimal.Parse(line[2]);
                    TimeDistribution tim = new TimeDistribution();
                    tim.Time = t;
                    tim.Probability = p;
                    ser.TimeDistribution.Add(tim);
                }
                sys.Servers.Add(ser);
            }
        }

    }
}