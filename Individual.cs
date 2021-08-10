using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAlgorithms {
    class Individual {

        /*
         An individual is a list of binary values representing wether or not a bag is chosen in the solution
         */
        private List<int> solution = new List<int>();
        private List<Knapsack> bags;

        public Individual(List<Knapsack> b) {
            bags = b;
        }

        public Individual(List<Knapsack> b, List<int> s) {
            bags = b;
            solution = s;
        }

        public List<int> GetSolution() {
            return solution;
        }

        public double GetFitness(double capacity) {
            /*
             Returns the fitness of an individual's solution which is the sum of the chosen bags' values if their weight don't exceed capacity
             */
            double fitness = 0;
            double weightSum = 0;
            for(int i=0; i<solution.Count; ++i) {
                fitness += solution[i] * bags[i].GetValue();

                /*
                 * fitness with value/weight ratio can be something interesting too but provided worse solutions in general
                if(bags[i].GetWeight() > 0)
                    fitness += solution[i] * (bags[i].GetValue() / bags[i].GetWeight()); */

                weightSum += solution[i] * bags[i].GetWeight();

                if(weightSum > capacity) { //weight > capacity => fitness = 0
                    return 0;
                }
            }

            return fitness;
        }

        public void AddKnapsackValue(int v) {
            solution.Add(v);
        }

        public override string ToString() {
            string s = "";
            for (int i = 0; i < solution.Count; ++i) {
                s += solution[i]+"|";
            }
            return s;
        }
    }
}
