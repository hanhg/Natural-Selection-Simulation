using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Natural_Selection_Simulation
{
    class Sensory : Neuron
    {
        public SenseType sense;
        public Sensory(SenseType s) : base(NeuronType.Sensory)
        {
            sense = s;
        }

        public double output(Grid grid, Creature thisCreature, Random rng)
        {
            //age
            if (sense == SenseType.Age)
                return (double)thisCreature.age / thisCreature.lifetime;
            //random input
            if (sense == SenseType.Rnd)
                return rng.NextDouble();
            //nearest border in the x direction
            if (sense == SenseType.BDx)
                return Math.Min(thisCreature.x, grid.grid.GetLength(1) - thisCreature.x)/(grid.grid.GetLength(1)/2.0);
            //nearest border in the y direction
            if (sense == SenseType.BDy)
                return Math.Min(thisCreature.y, grid.grid.GetLength(0) - thisCreature.y) / (grid.grid.GetLength(0) / 2.0);
            //nearest border overall
            if (sense == SenseType.BD)
                return Math.Min(
                    Math.Min(thisCreature.x, grid.grid.GetLength(1) - thisCreature.x), 
                    Math.Min(thisCreature.y, grid.grid.GetLength(0) - thisCreature.y)
                    ) / (grid.grid.GetLength(0) / 2.0);
            //x position
            if (sense == SenseType.Lx)
                return  thisCreature.x / (grid.grid.GetLength(1) + 0.0);
            //y position
            if (sense == SenseType.Ly)
                return thisCreature.y / (grid.grid.GetLength(0) + 0.0);

            return 0.0;
        }
    }
}
