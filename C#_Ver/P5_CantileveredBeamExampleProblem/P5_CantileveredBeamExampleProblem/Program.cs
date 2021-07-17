using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration;

namespace P5_CantileveredBeamExampleProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] good_value = new double[501];
            //------Initialize the particles------
            int particleNum = 1000, variableNum = 5;
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
                y[i] = BeamFunction(x[0, i], x[1, i], x[2, i], x[3, i], x[4, i]);
            }
            //------Calculating Fitness------0
            double mostFit = y.Min();
            int mostFitIndex = y.ToList().IndexOf(mostFit);
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
            Console.WriteLine($"{gBestX[0]} {gBestX[1]} {gBestX[2]} {gBestX[3]} {gBestX[4]} {gBestY}");
            good_value[0] = gBestY;
            //------Generate Next Generation------0
            int k = 0;
            do
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
                        if (x[i, j] >= 50d)
                        {
                            x[i, j] = 50d;
                        }
                        else if (x[i, j] <= 0.5d)
                        {
                            x[i, j] = 0.5d;
                        }
                    }
                }
                for (int i = 0; i < particleNum; i++)
                {
                    y[i] = BeamFunction(x[0, i], x[1, i], x[2, i], x[3, i], x[4, i]);
                }
                //------Calculating Fitness------1
                mostFit = y.Min();
                mostFitIndex = y.ToList().IndexOf(mostFit);
                //------Find the gBest------1
                if (y[mostFitIndex] <= gBestY)
                {
                    gBestY = y[mostFitIndex];
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
                Console.WriteLine($"{gBestX[0]} {gBestX[1]} {gBestX[2]} {gBestX[3]} {gBestX[4]} {gBestY}");
                good_value[k+1] = gBestY;
                k++;
            } while (k < 500);
            for (int i = 0; i < 501; i++)
            {
                var records = new List<DataOutput> { new DataOutput { Counter = i, Good_value = good_value[i] }, };
                bool append = true;
                var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                config.HasHeaderRecord = !append;
                using (var writer = new StreamWriter("C:/Users/quack/Desktop/CantileveredBeamExampleProblem.csv", append))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(records);
                }
            }
            Console.ReadLine();
        }
        static public double BeamFunction(double x1, double x2, double x3, double x4, double x5)
        {
            double result = 0;
            double P = 50000, L = 500, sigma = 14000;
            double[] b = new double[5] { x1, x2, x3, x4, x5 };
            double[] h = new double[5];
            for (int i = 0; i < 5; i++)
            {
                h[i] = Math.Sqrt(6 * P * (L - (i * L / 5)) / (sigma * b[i]));
            }
            for (int i = 0; i < 5; i++)
            {
                result += b[i] * h[i] * L / 5;
            }
            return result;
        }
        static double NextDouble(Random rand, double minValue, double maxValue)
        {
            return rand.NextDouble() * (maxValue - minValue) + minValue;
        }
        static double[,] Velocity(double[,] v, Random num, double[] gBestX, double[,] pBestX, double[,] x)
        {
            double c1 = 3d, c2 = 4d;
            double vmax = 5d;
            double w = 1.4d;
            for (int i = 0; i < v.GetLength(0); i++)
            {
                for (int j = 0; j < v.GetLength(1); j++)
                {
                    double ran = NextDouble(num, 0.00d, 1.00d),
                        ran1 = NextDouble(num, 0.00d, 1.00d);
                    v[i, j] = w * v[i, j] + c1 * ran * (pBestX[i, j] - x[i, j]) + c2 * ran * (gBestX[i] - x[i, j]);
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
                    double ran = NextDouble(num, 0.5d, 50d);
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