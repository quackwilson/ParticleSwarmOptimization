using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration;


namespace QZ1
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] good_value = new double[2001];
            //------Initialize the particles------
            int particleNum = 40, variableNum = 2;
            Random num = new Random();
            Particle particles = new Particle(num, variableNum, particleNum);
            double[,] x = new double[variableNum, particleNum];
            double[] y = new double[particleNum];
            double[,] v = new double[variableNum, particleNum];
            for (int i = 0; i < variableNum; i++)
            {
                for (int j = 0; j < particleNum; j++)
                {
                    x[i, j] = particles.Particles[i, j];
                }
            }
            for (int i = 0; i < particleNum; i++)
            {
                y[i] = ShubertFunction(x[0, i], x[1, i]);
            }
            //------Calculating Fitness------0
            double[] fitness = Fitness(y);
            double mostFit = fitness.Max();
            int mostFitIndex = fitness.ToList().IndexOf(mostFit);
            //------Find the gBest------0
            double gBestY = y[mostFitIndex];
            double[] gBestX = new double[variableNum];
            for (int i = 0; i < variableNum; i++)
            {
                gBestX[i] = x[i, mostFitIndex];
            }
            //------Find the pBest------0
            double[] pBestY = new double[particleNum];
            double[,] pBestX = new double[variableNum, particleNum];
            for (int i = 0; i < particleNum; i++)
            {
                pBestY[i] = y[i];
            }
            for (int i = 0; i < variableNum; i++)
            {
                for (int j = 0; j < particleNum; j++)
                {
                    pBestX[i, j] = x[i, j];
                }
            }
            //------Print------0
            Console.WriteLine($"{gBestX[0]} {gBestX[1]} {gBestY}");
            good_value[0] = gBestY;
            //------Generate Next Generation------0
            for (int k = 0; k < 2000; k++)
            {
                //Update Velocity
                v = Velocity(v, num, gBestX, pBestX, x);
                //Update Position
                for (int i = 0; i < variableNum; i++)
                {
                    for (int j = 0; j < particleNum; j++)
                    {
                        x[i, j] = x[i, j] + v[i, j];
                    }
                }
                for (int i = 0; i < variableNum; i++)
                {
                    for (int j = 0; j < particleNum; j++)
                    {
                        if (x[i, j] >= 500)
                        {
                            x[i, j] = 500;
                        }
                        else if (x[i, j] <= -500)
                        {
                            x[i, j] = -500;
                        }
                    }
                }
                for (int i = 0; i < particleNum; i++)
                {
                    y[i] = ShubertFunction(x[0, i], x[1, i]);
                }
                //------Calculating Fitness------1
                fitness = Fitness(y);
                mostFit = fitness.Max();
                mostFitIndex = fitness.ToList().IndexOf(mostFit);
                //------Find the gBest------1
                if (y[mostFitIndex] <= gBestY)
                {
                    gBestY = y[mostFitIndex];
                    gBestX = new double[variableNum];
                    for (int i = 0; i < variableNum; i++)
                    {
                        gBestX[i] = x[i, mostFitIndex];
                    }
                }
                //------Find the pBest------1
                for (int i = 0; i < particleNum; i++)
                {
                    if (y[i] >= pBestY[i])
                    {
                        pBestY[i] = y[i];
                    }
                }
                for (int i = 0; i < variableNum; i++)
                {
                    for (int j = 0; j < particleNum; j++)
                    {
                        if (y[i] >= pBestY[i])
                        {
                            pBestX[i, j] = x[i, j];
                        }
                    }
                }
                //------Print------1
                Console.WriteLine($"{gBestX[0]} {gBestX[1]} {gBestY}");
                good_value[k + 1] = gBestY;
            }
            for (int i = 0; i < 2001; i++)
            {
                var records = new List<DataOutput> { new DataOutput { Counter = i, Good_value = good_value[i] }, };
                bool append = true;
                var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                config.HasHeaderRecord = !append;
                using (var writer = new StreamWriter("C:/Users/quack/Desktop/QZ1Function.csv", append))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(records);
                }
            }
            Console.ReadLine();
        }
        static double ShubertFunction(double x0, double x1)
        {
            double a = 418.9829 * 2,
                b = x0 * Math.Sin(Math.Sqrt(x0)),
                c = x1 * Math.Sin(Math.Sqrt(x1));
            double ans = a - b - c;
            return ans;
        }
        static double[] Fitness(double[] y)
        {
            double[] fitness = new double[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                fitness[i] = -y[i];
            }
            return fitness;
        }
        static double NextDouble(Random rand, double minValue, double maxValue)
        {
            return rand.NextDouble() * (maxValue - minValue) + minValue;
        }
        static double[,] Velocity(double[,] v, Random num, double[] gBestX, double[,] pBestX, double[,] x)
        {
            int c1 = 2, c2 = 2;
            double vmax = 100d;
            for (int i = 0; i < v.GetLength(0); i++)
            {
                for (int j = 0; j < v.GetLength(1); j++)
                {
                    double ran = NextDouble(num, 0.00, 1.00),
                        ran1 = NextDouble(num, 0.00, 1.00);
                    v[i, j] = v[i, j] + c1 * ran * (pBestX[i, j] - x[i, j]) + c2 * ran1 * (gBestX[i] - x[i, j]);
                }
            }
            for (int i = 0; i < v.GetLength(0); i++)
            {
                for (int j = 0; j < v.GetLength(1); j++)
                {
                    if (v[i, j] >= vmax)
                    {
                        v[i, j] = vmax;
                    }
                    else if (v[i, j] <= -vmax)
                    {
                        v[i, j] = -vmax;
                    }
                }
            }
            return v;
        }
    }
    class Particle
    {
        public Particle(Random num, int variableNumber, int particleNumber)
        {
            double[,] particles = new double[variableNumber, particleNumber];
            for (int i = 0; i < variableNumber; i++)
            {
                for (int j = 0; j < particleNumber; j++)
                {
                    double ran = NextDouble(num, -500.00, 500.00);
                    particles[i, j] = ran;
                    Particles = particles;
                }
            }
        }
        public double[,] Particles;
        public double NextDouble(Random rand, double minValue, double maxValue)
        {
            return rand.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
    public class DataOutput
    {
        public int Counter { get; set; }
        public double Good_value { get; set; }
    }
}
