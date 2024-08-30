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
    class Grid
    {
        public Cell[,] grid;

        public Grid(int width, int height)
        {
            grid = new Cell[height, width];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = new Cell(CellState.empty, 0);
                }
            }
        }
        public Cell getCell(int x, int y)
        {
            return grid[x, y];
        }
        public void setCell(int x, int y, Cell cell)
        {
            grid[x, y] = cell;
        }
        public void setCell(int x, int y, CellState state)
        {
            grid[x, y].state = state;
        }
        public void setCell(int x, int y, Creature creature)
        {
            grid[x, y].state = CellState.creature;
            grid[x, y].creature = creature;
        }
        public void setCell(int x, int y, double pheromone)
        {
            grid[x, y].pheromone = pheromone;
        }

        public bool moveCreature(int x, int y, int relativeX, int relativeY)
        {
            int newX = -1;
            if (x + relativeX < 0)
                newX = 0;
            else if (x + relativeX >= grid.GetLength(0))
                newX = grid.GetLength(0)-1;
            else
                newX = x + relativeX;

            int newY = -1;
            if (y + relativeY < 0)
                newY = 0;
            else if (y + relativeY >= grid.GetLength(1))
                newY = grid.GetLength(1) - 1;
            else
                newY = y + relativeY;

            if (relativeX != 0 && newX == x)
                return false;
            if (relativeY != 0 && newY == y)
                return false;

            if (grid[newX, newY].state == CellState.empty)
            {
                Cell temp = grid[newX, newY];
                grid[newX, newY] = grid[x, y];
                grid[x, y] = temp;
                return true;
            }


            return false;// was not able to move;

        }

        public void clearCreatures()
        {
            for(int i = 0; i < grid.GetLength(0); i++)
            {
                for(int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[j, i].state == CellState.creature)
                    {
                        grid[j, i].state = CellState.empty;
                        grid[j, i].creature = null;
                    }
                    grid[j, i].pheromone = 0;
                }
            }
        }

        
    }
}
