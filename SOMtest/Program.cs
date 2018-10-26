using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using System.IO;

namespace SOMtest
{
    class Program
    {

        static void Main(string[] args)
        {
            Again:
            Console.Write("Please input neuron count a (sqrt(a) € N): ");
            int neuronCount = Convert.ToInt32(Console.ReadLine());
            int dimension = (int)Math.Sqrt(neuronCount);
            Console.Write("Please input learning rate: ");
            double learningRate = Convert.ToDouble(Console.ReadLine());
            Console.Write("Please input radius: ");
            double radius = Convert.ToDouble(Console.ReadLine());
            Console.Write("Please input epoch count: ");
            int epochs = Convert.ToInt32(Console.ReadLine());

            int iterations = 13300;
            //double learningRate = 0.1;
            //double radius = 100;
            Console.WriteLine("Press any key to start training!");
            Console.ReadKey();
            
            int dimensionCount = 8;
            Neuron.RandRange = new Range(0, 10);
            DistanceNetwork mynet = new DistanceNetwork(dimensionCount, neuronCount);
            mynet.Randomize();

            SOMLearning trainer = new SOMLearning(mynet, dimension, dimension);
            double fixedLearningRate = learningRate / 10;
            double driftingLearningRate = fixedLearningRate * 9;
            int i = 0;
            //int epochs = 200;
            for (int j = 0; j < epochs; j++)
            {
                var reader = new StreamReader("ResultsUnique.csv");
                trainer.LearningRate = driftingLearningRate * (epochs - j) / epochs + fixedLearningRate;
                trainer.LearningRadius = (double)radius * (epochs - j) / epochs;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    double[] doubles = Array.ConvertAll(values, Double.Parse);


                    trainer.Run(doubles);
                    i++;
                    if (i == iterations)
                    {
                        i = 0;
                        break;
                    }
                }
                if (j % 25 == 0)
                {
                    Console.WriteLine("Epoch " + j);
                }
            }

            Console.Beep();
            Console.WriteLine();
            Console.WriteLine("Training complete! Learning rate: " + learningRate + ", Radius: " + radius);
            Console.WriteLine();
            int neuroncounter = 1;
            foreach (var neuron in mynet.Layers[0].Neurons)
            {
                int weightcounter = 1;
                Console.Write("Neuron {0:00.##}: [",neuroncounter);
                foreach (var weight in neuron.Weights)
                {
                    if (weightcounter == 8)
                    Console.Write("{0:00.00}", weight);
                    else
                    Console.Write("{0:00.00} ", weight);
                    weightcounter++;
                }
                Console.WriteLine("]");
                neuroncounter++;
            }

            Console.WriteLine();
            Console.WriteLine("Neuron distance matrix!");
            Console.WriteLine();
            DisplayDistanceMatrix(mynet.Layers[0]);
            

            Console.WriteLine("Press any key to start over!");
            Console.ReadKey();
            goto Again;
            
        }

        private static void DisplayDistanceMatrix(Layer layer)
        {
            for (int i = 0; i < layer.Neurons.Count(); i++)
            {
                Console.Write("\n");
                for (int j = 0; j < layer.Neurons.Count(); j++)
                {
                    Console.Write("{0:00.00}\t", GetEuclideanDistance(layer.Neurons[i], layer.Neurons[j]));
                }
            Console.Write("\n\n");
            }
        }

        public static double GetEuclideanDistance(Neuron a, Neuron b)
        {
            double sumSquares = 0;
            for (int i = 0; i < a.Weights.Count();i++)
            {
                sumSquares = sumSquares + Math.Pow((a.Weights[i] - b.Weights[i]), 2);
            }
            return Math.Sqrt(sumSquares);
        }
    }
}
