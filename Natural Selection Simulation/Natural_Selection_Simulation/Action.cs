using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Natural_Selection_Simulation
{
    class Action : Neuron
    {
        public ActionType action;
        public Action(ActionType t) : base(NeuronType.Action)
        {
            action = t;
        }

        public double output()
        {
            return Math.Tanh(base.sum);
        }
    }
}
