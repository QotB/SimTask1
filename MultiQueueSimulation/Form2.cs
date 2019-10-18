using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
namespace MultiQueueSimulation
{
    public partial class Form2 : Form
    {
        public Form2(MultiQueueModels.SimulationSystem sys)
        {
            InitializeComponent();
            for (int i = 1; i <= sys.NumberOfServers; i++)
                comboBox1.Items.Add(i.ToString());
        }
        public int ind;
        private void Form2_Load(object sender, EventArgs e)
        {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ind = int.Parse(comboBox1.SelectedItem.ToString());
            MultiQueueModels.Server ser = Form1.sys.Servers[ind-1];
            var series = new Series("Server " + comboBox1.SelectedItem.ToString());

            // Frist parameter is X-Axis and Second is Collection of Y- Axis
            int[] x;
            int[] y;
            x = new int[ser.x.Count()+5];
            y = new int[ser.y.Count()+5];
            for (int i = 0; i < ser.x.Count(); i++)
            {
                x[i] = ser.x[i];
                y[i] = ser.y[i];
            }
            series.Points.DataBindXY(x, y);
            chart1.Series.Clear();
            chart1.Series.Add(series);
            chart1.Series["Server " + comboBox1.SelectedItem.ToString()]["PointWidth"] = "1";

        }
    }
}
