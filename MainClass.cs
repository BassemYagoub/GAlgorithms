using System;
using System.Collections.Generic;

namespace GAlgorithms {
    class MainClass {

        static void Main(string[] args) {
            //bags parameters
            const int nb_bags = 500;

            //values to randomize the bags' values/weights 
            const int minValue = 1;
            const int maxRandValue = 20;

            const uint minWeight = 2;
            const uint maxRandWeight = 30;

            //GA parameters
            const int popSize = 1000;
            const int nbGen = 200;
            const double mutationProba = 0.05;
            const double capacity = nb_bags*(maxRandWeight-minWeight)/2; //seems to be a good value to have something interesting 
            const string selectionMethod = "biasedRandom"; //two methods of selection : "tournament" / "biasedWRandom"
            const bool showProcess = false; //true to show process details


            //generating random knapsacks
            List<Knapsack> bags = new List<Knapsack>();
            for(int i=0; i<nb_bags; ++i) {
                bags.Add(new Knapsack(new Random().Next(minValue, (maxRandValue+1)), new Random().Next((int)minWeight, ((int)maxRandWeight +1) )));
                Console.WriteLine(bags[i].ToString());
            }

            GeneticAlgorithm ga = new GeneticAlgorithm(bags, popSize, nbGen, mutationProba, capacity);
            ga.Generate(selectionMethod, showProcess);

        }
    }
}