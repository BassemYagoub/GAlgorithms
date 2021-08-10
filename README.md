# GAlgorithms
*A simple genetic algorithm implementation for the knapsack problem (that might evolve in multiple GA algorithms for various problems/games)*

## *V1.0*
First implemented algorithm :
- A solution is a list of binaries acting as booleans to state if a bag is chosen or not
- Two selection methods : Tournament / Biased Random
- Crossover : Each parent gives a half of genome its to each children
- Mutation : Add a random bag to the solution (some 0 in the solution becomes a 1)
- Parameters can be played with in the MainClass.cs file