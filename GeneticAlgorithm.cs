using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAlgorithms {
    class GeneticAlgorithm {

        private List<Item> items;
        private List<Individual> pop; //population that is going to evolve with time
        private List<double> fitnesses = new List<double>(); //fitness of every individual in the population
        private int popSize;
        private int nbGen;
        private double mutationProba;
        private double capacity; //max weight that can be carried
        private Random rand = new Random();

        public GeneticAlgorithm(List<Item> items, int popSize, int nbGen, double mutationProba, double capacity) {
            this.items = items;
            this.popSize = popSize;
            this.nbGen = nbGen;
            this.mutationProba = mutationProba;
            this.capacity = capacity;
        }

        public Individual Generate(string selectionMethod, bool showProcess = false) {
            /*
             The genetic algorithm that generates individuals based on the parameters given in the constructor
             */
            Console.WriteLine("Generation launched\n");

            pop = CreatePopulation();
            List<Individual> newPop; //population that will be updated in the loop

            for (int i = 0; i < nbGen; ++i) {

                if (showProcess)
                    Console.WriteLine("______________________\nGeneration " + i + "\n");

                else { //just to show that the algorithm is running
                    Console.Clear();
                    Console.WriteLine("Generation " + i + "\n");

                    if (fitnesses.Count > 0) //empty in first loop
                        Console.WriteLine("best fitness : " + fitnesses[0] + "\n"); //0 because the list will be sorted (desc)
                }

                fitnesses = CalculateFitnesses(showProcess);

                //check if there are at least 2 different exploitable solutions at the beginning of the algorithm to avoid running it with a bad "dataset"
                if (i == 0 && !CheckViability()) {
                    ShowBestIndividual(pop[0]);
                    return pop[0];
                }

                newPop = new List<Individual>();

                for (int j = 0; j < popSize/2; ++j) {
                    
                    //Selection
                    Individual parent1 = Selection(selectionMethod);
                    Individual parent2 = Selection(selectionMethod);

                    while(parent2 == parent1) { //to avoid getting the same individual twice
                        parent2 = Selection(selectionMethod);
                    }

                    //Crossover
                    var (child1, child2) = Crossover(parent1, parent2);
                    newPop.Add(child1);
                    newPop.Add(child2);

                    //Mutation
                    Mutation(child1, child2, showProcess);
                }

                //Insertion (remove individuals with the worst fitness to keep popSize individuals)
                pop.AddRange(newPop);
                pop = pop.OrderByDescending(indiv => indiv.GetFitness(capacity)).Take(pop.Count).ToList();
            }

            ShowBestIndividual(pop[0]);
            return pop[0];
        }

        public List<Individual> CreatePopulation() {
            /*
             Returns a population of size popSize with random bits
             */
            List<Individual> pop = new List<Individual>(popSize);

            for (int i = 0; i < popSize; ++i) { //generate n individuals
                pop.Add(new Individual(items));
                for (int j = 0; j < items.Count(); ++j) { //adding one by one a random bit
                    pop[i].AddItemValue(rand.Next(0, 2));
                }
            }

            return pop;
        }

        public List<double> CalculateFitnesses(bool showProcess) {
            /*
             Returns a list of the population's fitness
             */

            List<double> fitnessesTmp = new List<double>();
            for (int j = 0; j < popSize; ++j) {
                fitnessesTmp.Add(pop[j].GetFitness(capacity));

                if (showProcess)
                    Console.WriteLine(pop[j].ToString() + "| fitness : " + fitnessesTmp[j] + "\n");
            }

            return fitnessesTmp;
        }

        public bool CheckViability(int nbTests = 5) {
            /*
             Returns true if there are at least 2 different exploitable solutions
                else false (capacity probably too low)
             */

            for (int t = 0; t < nbTests; ++t){
                int nbSolutionsOK = 0;

                //check if current individuals are ok
                for(int i=0; i<fitnesses.Count(); ++i) {
                    if(fitnesses[i] > 0) {
                        nbSolutionsOK++;

                        if(nbSolutionsOK >= 2) { //2 different individuals needed for selection
                            return true;
                        }
                    }
                }

                //else : recreate the population
                pop = CreatePopulation();
                fitnesses = CalculateFitnesses(false);
            }

            //if after nbTests there is no exploitable population : it might be impossible to have a viable solution
            return false;
        }

        private Individual Selection(string selectionMethod) {
            /*
             Returns an individual based on the choosen selection method
             */
            if(selectionMethod == "tournament") {
                return TournamentSelection();
            }
            else{ //if(selectionMethod == "biasedRandom") {
                return BiasedRandomSelection();
            }
        }

        private Individual TournamentSelection() {
            /*
             Returns the best of both randomly selected individuals
             */
            int ind1 = rand.Next(0, popSize);
            int ind2 = rand.Next(0, popSize);
            Individual potentialParent1 = pop[ind1];
            Individual potentialParent2 = pop[ind2];

            while (potentialParent2 == potentialParent1) {
                ind2 = rand.Next(0, popSize);
                potentialParent2 = pop[ind2];
            }
            
            if (fitnesses[ind1] >= fitnesses[ind2]) {
                return potentialParent1;
            }

            return potentialParent2;
        }

        private Individual BiasedRandomSelection() {
            /*
             Returns an individual with a probability proportional to its fitness
             */
            double maxFitness = fitnesses.Sum(); //maximum value potentially attainable (if total weight <= capacity)
            double randSelector = rand.NextDouble();

            //same order needed
            pop.OrderByDescending(indiv => indiv.GetFitness(capacity));
            fitnesses.OrderByDescending(f => f);

            if(maxFitness > 0) {
                double probaSum = 0;
                for (int i=0; i< pop.Count; ++i) {
                    probaSum += fitnesses[i] / maxFitness; //fitnesses[i]/maxFitness <=> probability of being selected (summed => from 0-->1 with time)
                    if (randSelector <= probaSum) { 
                        return pop[i];
                    }
                }
            }
            return pop[rand.Next(0, pop.Count)]; //shouldn't happen unless all fitnesses are 0

        }

        private (Individual, Individual) Crossover(Individual parent1, Individual parent2) {
            /*
             Returns 2 children, each having a different half of genome from their parents
             */
            List<int> i1Sol = parent1.GetSolution();
            List<int> i2Sol = parent2.GetSolution();

            List<int> child1Sol = new List<int>(i1Sol.Count);
            List<int> child2Sol = new List<int>(i2Sol.Count);

            for (int i = 0; i < i1Sol.Count; ++i) {
                if (i <= (i1Sol.Count) / 2) {
                    child1Sol.Add(i1Sol[i]);
                    child2Sol.Add(i2Sol[i]);
                }
                else {
                    child1Sol.Add(i2Sol[i]);
                    child2Sol.Add(i1Sol[i]);
                }
            }

            Individual child1 = new Individual(items, child1Sol);
            Individual child2 = new Individual(items, child2Sol);
            return (child1, child2);
        }

        private void Mutation(Individual ind1, Individual ind2, bool showProcess) {
            /*
             With mutationProba % : 
                invert a random bit with value 0 to value 1
                (initially : invert a random bit from ind1 & ind2)
             */
            if (rand.NextDouble() <= mutationProba) { //mutate

                if (showProcess)
                    Console.WriteLine("mutation\n");

                int indexToChange = rand.Next(0, items.Count);
                while(ind1.GetSolution()[indexToChange] == 1) {
                    indexToChange = rand.Next(0, items.Count);
                }

                ind1.GetSolution()[indexToChange] = 1; //(ind1.GetSolution()[indexToChange] + 1) % 2;
                
                indexToChange = rand.Next(0, items.Count);
                while (ind2.GetSolution()[indexToChange] == 1) {
                    indexToChange = rand.Next(0, items.Count);
                }
                ind2.GetSolution()[indexToChange] = 1; // (ind2.GetSolution()[indexToChange] + 1) % 2;
            }
        }


        private void ShowBestIndividual(Individual bestIndiviual) {
            /*
             Show which items were chosen by the solution, its fitness, its weight ratio
             */
            List<int> bestSolution = bestIndiviual.GetSolution();
            Console.WriteLine("\n______________________________________________");
            Console.WriteLine("______________________________________________");
            Console.WriteLine("BEST SOLUTION FOUND : \n");

            if (bestIndiviual.GetFitness(capacity) > 0) {
                double usedWeight = 0, totalWeight = 0, valueSum = 0, bestCaseFitness = 0;
                for (int i = 0; i < bestSolution.Count; ++i) {
                    if (bestSolution[i] == 1) {
                        Console.WriteLine(items[i].ToString() + " X"); //choosen values are marked with an 'X'
                        usedWeight += items[i].GetWeight();
                        valueSum += items[i].GetValue();
                    }
                    else {
                        Console.WriteLine(items[i].ToString());
                        totalWeight += items[i].GetWeight();
                    }
                    bestCaseFitness += items[i].GetValue();

                }
                totalWeight += usedWeight;
                Console.WriteLine("\nFitness : " + valueSum + "$ (total value : " + bestCaseFitness + "$)"); //total value = sum of EVERY object's value
                Console.WriteLine("Weight used : " + usedWeight + "/" + capacity + "kg (total weight = " + totalWeight + "kg)\n");
            }
            else {
                Console.WriteLine("NONE");
                Console.WriteLine("Hint : Capacity is probably too low");
            }

        }

    }
}
