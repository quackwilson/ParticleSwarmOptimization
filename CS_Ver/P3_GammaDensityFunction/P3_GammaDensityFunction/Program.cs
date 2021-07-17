using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace P3_GammaDensityFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] good_value = new double[10001];
            //------Read Data------
            var reader = new StreamReader(File.OpenRead(@"..\..\Pulse_data.csv"));
            List<string> t = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                t.Add(values[0]);
            }
            string[] actual_data = new string[40];
            for (int i = 0; i < 40; i++)
            {
                actual_data[i] = t[i];
            }
            double[] actual_data_double = new double[40];
            for (int i = 0; i < 40; i++)
            {
                actual_data_double[i] = Convert.ToDouble(actual_data[i]);
            }
            //------Initialize the particles------
            int particleNum = 100;
            Random num = new Random();
            Particle particlesAlpha = new Particle(num, particleNum, 1.00d, 5.00d);
            Particle particlesBeta = new Particle(num, particleNum, 1.00d, 5.00d);
            Particle particlesGamma = new Particle(num, particleNum, 18.00d, 22.00d);
            Particle particlesA = new Particle(num, particleNum, 1.00d, 300.00d);
            Particle particlesB = new Particle(num, particleNum, 1.00d, 200.00d);
            double[,] x = new double[5, particleNum];
            double[,] y = new double[particleNum, 40];
            double[,] v = new double[5, particleNum];
            for (int i = 0; i < particleNum; i++)
            {
                x[0, i] = particlesAlpha.Particles[i];
                x[1, i] = particlesBeta.Particles[i];
                x[2, i] = particlesGamma.Particles[i];
                x[3, i] = particlesA.Particles[i];
                x[4, i] = particlesB.Particles[i];
            }
            for (int i = 0; i < particleNum; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    y[i, j] = Gamma(x[0, i], x[1, i], x[2, i], x[3, i], x[4, i])[j];
                }
            }
            //------Calculating Fitness------0
            double[] Fitness = fitness(y, actual_data_double, particleNum);
            double mostFit = Fitness.Min();
            int mostFitIndex = Fitness.ToList().IndexOf(mostFit);
            //------Find the gBest------0
            double gBestFitness;
            gBestFitness = mostFit;
            double[] gBestX = new double[5];
            for (int i = 0; i < 5; i++)
            {
                gBestX[i] = x[i, mostFitIndex];
            }
            //------Find the pBest------0
            double[,] pBestY = new double[particleNum, 40];
            double[,] pBestX = new double[5, particleNum];
            double[] pBestFitness = new double[particleNum];
            for (int i = 0; i < particleNum; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    pBestY[i, j] = y[i, j];
                }
                pBestFitness[i] = Fitness[i];
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < particleNum; j++)
                {
                    pBestX[i, j] = x[i, j];
                }
            }
            Console.WriteLine($"{gBestX[0]} {gBestX[1]} {gBestX[2]} {gBestX[3]} {gBestX[4]} {gBestFitness}");
            good_value[0] = gBestFitness;
            //------Generate Next Generation------0
            int k = 0;
            do
            {
                //Update Velocity
                v = Velocity(v, num, gBestX, pBestX, x);
                //Update Position
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < particleNum; j++)
                    {
                        x[i, j] = x[i, j] + v[i, j];
                    }
                }
                for (int j = 0; j < particleNum; j++)
                {
                    if (x[0, j] >= 5d)
                    {
                        x[0, j] = 5d;
                    }
                    else if (x[0, j] <= 1d)
                    {
                        x[0, j] = 1d;
                    }
                    if (x[1, j] >= 5d)
                    {
                        x[1, j] = 5d;
                    }
                    else if (x[1, j] <= 1d)
                    {
                        x[1, j] = 1d;
                    }
                    if (x[2, j] >= 22d)
                    {
                        x[2, j] = 22d;
                    }
                    else if (x[2, j] <= 18d)
                    {
                        x[2, j] = 18d;
                    }
                    if (x[3, j] >= 300d)
                    {
                        x[3, j] = 300d;
                    }
                    else if (x[3, j] <= 1d)
                    {
                        x[3, j] = 1d;
                    }
                    if (x[4, j] >= 200d)
                    {
                        x[4, j] = 200d;
                    }
                    else if (x[4, j] <= 1d)
                    {
                        x[4, j] = 1d;
                    }
                }
                for (int i = 0; i < particleNum; i++)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        y[i, j] = Gamma(x[0, i], x[1, i], x[2, i], x[3, i], x[4, i])[j];
                    }
                }
                //------Calculating Fitness------1
                Fitness = fitness(y, actual_data_double, particleNum);
                mostFit = Fitness.Min();
                mostFitIndex = Fitness.ToList().IndexOf(mostFit);
                //------Find the gBest------1
                if (mostFit <= gBestFitness)
                {
                    gBestFitness = mostFit;
                    gBestX = new double[5];
                    for (int i = 0; i < 5; i++)
                    {
                        gBestX[i] = x[i, mostFitIndex];
                    }
                }
                //------Find the pBest------1
                for (int i = 0; i < particleNum; i++)
                {
                    if (Fitness[i] >= pBestFitness[i])
                    {
                        pBestFitness[i] = Fitness[i];
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < particleNum; j++)
                    {
                        if (Fitness[i] >= pBestFitness[i])
                        {
                            pBestX[i, j] = x[i, j];
                        }
                    }
                }
                Console.WriteLine($"{gBestX[0]} {gBestX[1]} {gBestX[2]} {gBestX[3]} {gBestX[4]} {gBestFitness}");
                good_value[k+1] = gBestFitness;
                k++;
            } while (k < 10000);
            for (int i = 0; i < 10001; i++)
            {
                var records = new List<DataOutput> { new DataOutput { Counter = i, Good_value = good_value[i] }, };
                bool append = true;
                var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                config.HasHeaderRecord = !append;
                using (var writer = new StreamWriter("C:/Users/quack/Desktop/Gamma.csv", append))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(records);
                }
            }
            Console.ReadLine();
        }  
        static public double[] Gamma(double alpha, double beta, double delta, double A, double B)
        {
            double[] y_value = new double[40];
            for (int i = 0; i < 40; i++)
            {
                if ((i + 1) < delta)
                {
                    y_value[i] = A * Math.Pow((i + 1), alpha) * Math.Exp(((-beta) * (i + 1)) / 10);
                }
                else
                {
                    y_value[i] = (A * Math.Pow((i + 1), alpha) * Math.Exp(((-beta) * (i + 1)) / 10)) + (B * Math.Pow(((i + 1) - delta), alpha) * Math.Exp(((-beta) * (i + 1 - delta)) / 10));
                }
            }
            return y_value;
        }
        static public double[] fitness(double[,] y_value, double[] y_actual, int particleNum)
        {
            //Calculate RMSE
            double[] RMSE = new double[particleNum];
            double[] Operator = new double[40];
            for (int i = 0; i < particleNum; i++)
            {
                double c = 0;
                for (int j = 0; j < 40; j++)
                {
                    Operator[j] = (y_value[i, j] - y_actual[j]) * (y_value[i, j] - y_actual[j]);
                    c = c + Operator[j];
                }
                RMSE[i] = Math.Sqrt(c / 40);
            }
            return RMSE;
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
        public Particle(Random num, int particleNumber,double lower,double upper)
        {
            double[] particles = new double[particleNumber];
            for (int i = 0; i < particleNumber; i++)
            {
                double ran = NextDouble(num, lower, upper);
                particles[i] = ran;
                Particles = particles;
            }
        }
        public double[] Particles;
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