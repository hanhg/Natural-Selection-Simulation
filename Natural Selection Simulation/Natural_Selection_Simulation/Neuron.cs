using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Natural_Selection_Simulation
{
    abstract class Neuron
    {
        public double sum;
        public NeuronType type;
        public Neuron(NeuronType type)
        {
            sum = 0;
            this.type = type;
        }
    }
}
