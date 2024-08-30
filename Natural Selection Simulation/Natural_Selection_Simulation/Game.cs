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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D rectangle;
        Texture2D circle;
        int numCreatures;
        Vector2 gridSize;
        List<Creature> creatures;
        Grid grid;
        Rectangle[,] rectangleGrid;

        Random rng;
        int generation;
        int timer;
        double rateOfMutation;// 0 - 1

        bool pause;
        KeyboardState oldKb;

        SpriteFont title;

        Rectangle window;
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            gridSize = new Vector2(150,150);
            generation = 0;
            timer = 0;
            numCreatures = 400;
            rateOfMutation = .002;
            oldKb = Keyboard.GetState();
            pause = false;

            // TODO: Add your initialization logic here
            rng = new Random();

            grid = new Grid((int)gridSize.X,(int)gridSize.Y);

            rectangleGrid = new Rectangle[(int)gridSize.X, (int)gridSize.Y];
            for(int i = 0; i < rectangleGrid.GetLength(1); i++)
            {
                for(int j = 0; j < rectangleGrid.GetLength(0); j++)
                {
                    rectangleGrid[j, i] = new Rectangle(600/(int)gridSize.X * i, 600 / (int)gridSize.Y * j, 600 / (int)gridSize.X, 600 / (int)gridSize.Y);
                }
            }
            window = new Rectangle(0, 0, rectangleGrid[0, 0].Width * rectangleGrid.GetLength(0), rectangleGrid[0, 0].Width * rectangleGrid.GetLength(0));


            creatures = new List<Creature>();
            for (int i = 0; i < numCreatures; i++)
            {
                creatures.Add(new Creature(400, 20, 30, rng));
                //DEBUG
                /*
                creatures[i].genes.Clear();
                creatures[i].genes.Add(new Gene(creatures[i].senses[1], creatures[i].actions[0], 4, 3, 3, 3));*/
            }
            for (int i = 0; i < creatures.Count; i++)
            {
                int x = -1;
                int y = -1;
                bool cont;
                do
                {
                    cont = true;
                    x = rng.Next((int)gridSize.X);
                    y = rng.Next((int)gridSize.Y);
                    if (grid.grid[x, y].state != CellState.empty)
                        cont = false;
                } while (!cont);
                grid.setCell(y, x, creatures[i]);
                creatures[i].x = x;
                creatures[i].y = y;
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            rectangle = Content.Load<Texture2D>("rectangle");
            circle = Content.Load<Texture2D>("circle");
            title = Content.Load<SpriteFont>("Title");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            KeyboardState kb = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            if(kb.IsKeyDown(Keys.Space) && !oldKb.IsKeyDown(Keys.Space))
                pause = !pause;

            
            if (!pause)
            {
                if (generation % 5 == 1 || generation % 5 == 6)
                {
                    fastForward(generation + 14);
                }

                //update action neurons
                for (int i = 0; i < creatures.Count; i++)
                {
                    creatures[i].calculate(grid, rng);
                }
                //actions
                for (int i = 0; i < creatures.Count; i++)
                {
                    Creature c = creatures[i];
                    List<Action> a = c.actions;
                    for (int j = 0; j < a.Count; j++)
                    {

                        //move forward
                        if (a[j].action == ActionType.Mfd && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == Direction.Up)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                                if (moved)
                                    c.y -= 1;
                            }
                            if (c.face == Direction.Right)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                                if (moved)
                                    c.x += 1;
                            }
                            if (c.face == Direction.Down)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                                if (moved)
                                    c.y += 1;
                            }
                            if (c.face == Direction.Left)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                                if (moved)
                                    c.x -= 1;
                            }
                        }

                        //move reverse
                        if (a[j].action == ActionType.Mrv && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == Direction.Up)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                                if (moved)
                                    c.y += 1;
                            }
                            if (c.face == Direction.Right)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                                if (moved)
                                    c.x -= 1;
                            }
                            if (c.face == Direction.Down)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                                if (moved)
                                    c.y -= 1;
                            }
                            if (c.face == Direction.Left)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                                if (moved)
                                    c.x += 1;
                            }
                        }

                        //move random
                        if (a[j].action == ActionType.Mrn && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;

                            Direction move = (Direction)rng.Next(4);

                            if (move == Direction.Up)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                                if (moved)
                                    c.y -= 1;
                            }
                            if (move == Direction.Right)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                                if (moved)
                                    c.x += 1;
                            }
                            if (move == Direction.Down)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                                if (moved)
                                    c.y += 1;
                            }
                            if (move == Direction.Left)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                                if (moved)
                                    c.x -= 1;
                            }
                        }
                        if (a[j].action == ActionType.Rt && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == (Direction)3)
                                c.face = 0;
                            else
                                c.face = (Direction)((int)(c.face) + 1);
                        }
                        if (a[j].action == ActionType.Lt && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == 0)
                                c.face = (Direction)3;
                            else
                                c.face = (Direction)((int)(c.face) - 1);
                        }
                        if (a[j].action == ActionType.Ml && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                            if (moved)
                                c.x -= 1;
                        }
                        if (a[j].action == ActionType.Mr && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                            if (moved)
                                c.x += 1;
                        }
                        if (a[j].action == ActionType.Mu && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                            if (moved)
                                c.y -= 1;
                        }
                        if (a[j].action == ActionType.Md && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                            if (moved)
                                c.y += 1;
                        }

                    }

                }

                for (int i = 0; i < creatures.Count; i++)
                {
                    creatures[i].age++;
                }

                timer++;
                if (timer > 400)
                {
                    timer = 0;
                    for (int i = 0; i < creatures.Count; i++)
                    {
                        if (creatures[i].x < 50 || creatures[i].x > 100 || creatures[i].y < 50 || creatures[i].y > 100)
                        {
                            creatures.RemoveAt(i);
                            i--;
                        }
                    }

                    if (creatures.Count == 0)
                    {
                        creatures = new List<Creature>();
                        for (int i = 0; i < numCreatures; i++)
                        {
                            creatures.Add(new Creature(500, 20, 40, rng));

                            //DEBUG
                            /*
                            creatures[i].genes.Clear();
                            creatures[i].genes.Add(new Gene(creatures[i].senses[1], creatures[i].actions[0], 4, 3, 3, 3));*/
                        }
                    }

                    List<Creature> newCreatures = new List<Creature>();
                    int index = 0;
                    for (int j = 0; j < numCreatures; j++)
                    {

                        newCreatures.Add(new Creature(creatures[index], 400, rateOfMutation, rng));
                        index++;
                        if (index == creatures.Count)
                            index = 0;

                    }
                    grid.clearCreatures();
                    for (int i = 0; i < newCreatures.Count; i++)
                    {
                        int x = -1;
                        int y = -1;
                        bool cont;
                        do
                        {
                            cont = true;
                            x = rng.Next((int)gridSize.X);
                            y = rng.Next((int)gridSize.Y);
                            if (grid.grid[x, y].state != CellState.empty)
                                cont = false;
                        } while (!cont);
                        grid.setCell(y, x, newCreatures[i]);
                        newCreatures[i].x = x;
                        newCreatures[i].y = y;
                    }
                    creatures = newCreatures;

                    generation++;
                }
            }

            oldKb = kb;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            for(int i = 0; i < rectangleGrid.GetLength(1); i++)
            {
                for(int j = 0; j < rectangleGrid.GetLength(0); j++)
                {
                    if (grid.grid[j, i].state == CellState.creature)
                    {
                        spriteBatch.Draw(circle, rectangleGrid[j, i], grid.grid[j, i].creature.color);
                    }
                    else if (grid.grid[j, i].state == CellState.empty)
                        spriteBatch.Draw(rectangle, rectangleGrid[j, i], Color.White);
                    else if (grid.grid[j, i].state == CellState.block)
                        spriteBatch.Draw(rectangle, rectangleGrid[j, i], Color.Black);
                }
            }

            if(pause)
            {
                spriteBatch.Draw(rectangle, window, Color.Black*0.5f);
                spriteBatch.DrawString(title, "PAUSE", new Vector2(window.Width/2, window.Height/2), Color.White);
            }

            spriteBatch.DrawString(title, "Generation: " + generation, new Vector2(650, 0), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void fastForward(int gen)
        {
            int origin = generation;
            int count = 0;

            while(generation < gen)
            {
                //update action neurons
                for (int i = 0; i < creatures.Count; i++)
                {
                    creatures[i].calculate(grid, rng);
                }
                //actions
                for (int i = 0; i < creatures.Count; i++)
                {
                    Creature c = creatures[i];
                    List<Action> a = c.actions;
                    for (int j = 0; j < a.Count; j++)
                    {

                        //move forward
                        if (a[j].action == ActionType.Mfd && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == Direction.Up)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                                if (moved)
                                    c.y -= 1;
                            }
                            if (c.face == Direction.Right)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                                if (moved)
                                    c.x += 1;
                            }
                            if (c.face == Direction.Down)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                                if (moved)
                                    c.y += 1;
                            }
                            if (c.face == Direction.Left)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                                if (moved)
                                    c.x -= 1;
                            }
                        }

                        //move reverse
                        if (a[j].action == ActionType.Mrv && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == Direction.Up)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                                if (moved)
                                    c.y += 1;
                            }
                            if (c.face == Direction.Right)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                                if (moved)
                                    c.x -= 1;
                            }
                            if (c.face == Direction.Down)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                                if (moved)
                                    c.y -= 1;
                            }
                            if (c.face == Direction.Left)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                                if (moved)
                                    c.x += 1;
                            }
                        }

                        //move random
                        if (a[j].action == ActionType.Mrn && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;

                            Direction move = (Direction)rng.Next(4);

                            if (move == Direction.Up)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                                if (moved)
                                    c.y -= 1;
                            }
                            if (move == Direction.Right)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                                if (moved)
                                    c.x += 1;
                            }
                            if (move == Direction.Down)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                                if (moved)
                                    c.y += 1;
                            }
                            if (move == Direction.Left)
                            {
                                bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                                if (moved)
                                    c.x -= 1;
                            }
                        }
                        if (a[j].action == ActionType.Rt && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == (Direction)3)
                                c.face = 0;
                            else
                                c.face = (Direction)((int)(c.face) + 1);
                        }
                        if (a[j].action == ActionType.Lt && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            if (c.face == 0)
                                c.face = (Direction)3;
                            else
                                c.face = (Direction)((int)(c.face) - 1);
                        }
                        if (a[j].action == ActionType.Ml && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, 0, -1);
                            if (moved)
                                c.x -= 1;
                        }
                        if (a[j].action == ActionType.Mr && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, 0, 1);
                            if (moved)
                                c.x += 1;
                        }
                        if (a[j].action == ActionType.Mu && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, -1, 0);
                            if (moved)
                                c.y -= 1;
                        }
                        if (a[j].action == ActionType.Md && a[j].output() >= 0)
                        {
                            double rand = rng.NextDouble();

                            //action probability
                            if (rand > a[j].output())
                                continue;
                            bool moved = grid.moveCreature(c.y, c.x, 1, 0);
                            if (moved)
                                c.y += 1;
                        }

                    }

                }

                for (int i = 0; i < creatures.Count; i++)
                {
                    creatures[i].age++;
                }

                timer++;
                if (timer > 400)
                {
                    timer = 0;
                    for (int i = 0; i < creatures.Count; i++)
                    {
                        if (creatures[i].x < 50 || creatures[i].x > 100 || creatures[i].y < 50 || creatures[i].y > 100) //condition for creatures to die
                        {
                            creatures.RemoveAt(i);
                            i--;
                        }
                    }

                    if (creatures.Count == 0)
                    {
                        creatures = new List<Creature>();
                        for (int i = 0; i < numCreatures; i++)
                        {
                            creatures.Add(new Creature(500, 20, 40, rng));

                            //DEBUG
                            /*
                            creatures[i].genes.Clear();
                            creatures[i].genes.Add(new Gene(creatures[i].senses[1], creatures[i].actions[0], 4, 3, 3, 3));*/
                        }
                    }

                    List<Creature> newCreatures = new List<Creature>();
                    int index = 0;
                    for (int j = 0; j < numCreatures; j++)
                    {

                        newCreatures.Add(new Creature(creatures[index], 400, rateOfMutation, rng));
                        index++;
                        if (index == creatures.Count)
                            index = 0;

                    }
                    grid.clearCreatures();
                    for (int i = 0; i < newCreatures.Count; i++)
                    {
                        int x = -1;
                        int y = -1;
                        bool cont;
                        do
                        {
                            cont = true;
                            x = rng.Next((int)gridSize.X);
                            y = rng.Next((int)gridSize.Y);
                            if (grid.grid[x, y].state != CellState.empty)
                                cont = false;
                        } while (!cont);
                        grid.setCell(y, x, newCreatures[i]);
                        newCreatures[i].x = x;
                        newCreatures[i].y = y;
                    }
                    creatures = newCreatures;
                    generation++;
                    count++;
                    if ((gen - origin) / 100 == 0)
                        Console.WriteLine("Fast Forward Completion: " + (int)((double)count / (gen - origin) * 100) + "%");
                    else if (count % ((gen - origin) / 100.0) == 0)
                    {
                        Console.WriteLine("Fast Forward Completion: " + (int)((double)count / (gen - origin) * 100) + "%");
                    }
                }
            }
        }
    }
}
