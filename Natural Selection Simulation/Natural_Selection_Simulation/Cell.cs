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
    class Cell
    {
        public CellState state;
        public double pheromone;
        public Creature creature;
        public Cell(CellState state, double pheromone)
        {
            this.state = state;
            this.pheromone = pheromone;
            creature = null;
        }
    }
}
