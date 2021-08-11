using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAlgorithms {
    class Item {
        private readonly float value;
        private readonly float weight;

        public Item(float value, float weight) {
            this.value = value;
            this.weight = weight;
        }

        public float GetValue() {
            return this.value;
        }
        public float GetWeight() {
            return this.weight;
        }

        public float GetRatio() { 
            return value / weight;
        }

        public override string ToString() {
            return value +"$|"+weight+"kg";
        }
    }
}
