using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAlgorithms {
    class Knapsack {
        private readonly float value;
        private readonly float weight;

        public Knapsack(float value, float weight) {
            this.value = value;
            this.weight = weight;
        }

        public float GetValue() {
            return this.value;
        }
        public float GetWeight() {
            return this.weight;
        }

        /*returns the ratio value/weigth */
        public float GetRatio() { 
            return value / weight;
        }

        public override string ToString() {
            return value +"$|"+weight+"kg";
        }
    }
}
