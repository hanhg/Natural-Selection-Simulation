using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Natural_Selection_Simulation
{
    class Inner : Neuron
    {
        public int number; // which inner neuron
        public Inner(int n) : base(NeuronType.Inner) 
        {
            number = n;
        }
        public double output()
        {
            return Math.Tanh(base.sum);
        }
    }
}
