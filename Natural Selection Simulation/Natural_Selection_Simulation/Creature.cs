using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Natural_Selection_Simulation
{
    class Creature
    {
        public Color color;
        public Direction face;
        public int age; // current steps of creature
        public int lifetime; // maximum steps of creature
        public int x;
        public int y;

        //Neurons
        public List<Sensory> senses;
        public List<Inner> inners;
        public List<Action> actions;

        //Genes
        public List<Gene> genes;

        public Creature(int life, int numberOfInnerNeurons, int numberOfGenes, Random rng)
        {
            Converter convert = new Converter();
            face = (Direction)(rng.Next(4));
            age = 0;
            lifetime = life;

            senses = new List<Sensory>();
            inners = new List<Inner>();
            actions = new List<Action>();

            genes = new List<Gene>();

            //add senses (must change loop's max if new sense type is added)
            for(int i = 0; i < 7; i++)
            {
                senses.Add(new Sensory((SenseType)i));
            }
            //add inners
            for (int i = 0; i < numberOfInnerNeurons; i++)
            {
                inners.Add(new Inner(i));
            }
            //add senses (must change loop's max if new sense type is added)
            for (int i = 0; i < 9; i++)
            {
                actions.Add(new Action((ActionType)i));
            }
            
            for(int i = 0; i < numberOfGenes; i++)
            {
                Neuron from = null;
                Neuron to = null;
                do {
                    //choose random of inner or sensory neuron 
                    NeuronType neuronType = (NeuronType)rng.Next(2);

                    neuronType = (NeuronType)rng.Next(2);
                    if (neuronType == NeuronType.Sensory)
                        from = senses[rng.Next(senses.Count)];

                    else if (neuronType == NeuronType.Inner)
                        from = inners[rng.Next(numberOfInnerNeurons)];
                    

                    //choose random of inner or action neuron
                    neuronType = (NeuronType)rng.Next(2) + 1;
                    if (neuronType == NeuronType.Inner)
                        to = inners[rng.Next(numberOfInnerNeurons)];

                    else if (neuronType == NeuronType.Action)
                        to = actions[rng.Next(actions.Count)];

                    for(int j = 0; j < genes.Count; j++)
                    {
                        if (genes[j].isSimilar(from, to) || genes[j].isSimilar(to, from))
                            continue;
                    }

                } while (false);
                genes.Add(new Gene(from, to, rng.NextDouble() * 8 - 4, senses.Count, inners.Count, actions.Count));
            }

            //color based on genes
            int r = 0;
            int g = 0;
            int b = 0;

            for (int i = 0; i < genes.Count; i++)
            {
                Color c = convert.FromHex(genes[i].code);
                r += c.R;
                g += c.G;
                b += c.B;
            }
            color = makeColor();
        }
        public Creature(Creature parent, int life, double rateOfMutation, Random rng)
        {
            Converter convert = new Converter();

            face = (Direction)(rng.Next(4));
            age = 0;
            lifetime = life;

            senses = new List<Sensory>();
            inners = new List<Inner>();
            actions = new List<Action>();

            genes = new List<Gene>();

            //add senses (must change loop's max if new sense type is added)
            for (int i = 0; i < parent.senses.Count; i++)
            {
                senses.Add(new Sensory((SenseType)i));
            }
            //add inners
            for (int i = 0; i < parent.inners.Count; i++)
            {
                inners.Add(new Inner(i));
            }
            //add senses (must change loop's max if new sense type is added)
            for (int i = 0; i < parent.actions.Count; i++)
            {
                actions.Add(new Action((ActionType)i));
            }

            
            for(int i = 0; i < parent.genes.Count; i++)
            {
                //mutate
                if (rng.NextDouble() < rateOfMutation)
                {
                    int rand;
                    Neuron from = parent.genes[i].from;
                    Neuron to = parent.genes[i].to;
                    double strength = parent.genes[i].strength;

                    rand = rng.Next(3);
                    if(rand == 0)
                    {
                        rand = rng.Next(2);
                        if(rand == 0)
                        {
                            rand = rng.Next(senses.Count);
                            from = senses[rand];
                        }
                        if (rand == 1)
                        {
                            rand = rng.Next(inners.Count);
                            from = inners[rand];
                        }
                    }
                    else if (rand == 1)
                    {
                        rand = rng.Next(2);
                        if (rand == 0)
                        {
                            rand = rng.Next(inners.Count);
                            to = inners[rand];
                        }
                        if (rand == 1)
                        {
                            rand = rng.Next(actions.Count);
                            to = actions[rand];
                        }
                    }
                    else if(rand == 2)
                    {
                        strength = rng.NextDouble() * 8 - 4;
                    }
                    genes.Add(new Gene(from, to, strength, senses.Count, inners.Count, actions.Count));
                }
                else
                    genes.Add(new Gene(parent.genes[i], senses, inners, actions));
            }

            //color based on genes
            color = makeColor();
        }

        public String getNeuralNetwork()
        {
            String rtn = "";
            for(int i = 0; i < genes.Count; i++)
            {
                rtn += "\n" + genes[i].from.type + ": ";
                if (genes[i].from.type == NeuronType.Sensory)
                    rtn += ((Sensory)genes[i].from).sense;
                else if (genes[i].from.type == NeuronType.Inner)
                    rtn += ((Inner)genes[i].from).number;
                else if (genes[i].from.type == NeuronType.Action)
                    rtn += ((Action)genes[i].from).action;

                rtn += " ---> " + genes[i].to.type + ": ";
                if (genes[i].to.type == NeuronType.Sensory)
                    rtn += ((Sensory)genes[i].to).sense;
                else if (genes[i].to.type == NeuronType.Inner)
                    rtn += ((Inner)genes[i].to).number;
                else if (genes[i].to.type == NeuronType.Action)
                    rtn += ((Action)genes[i].to).action;
            }
            return rtn;
        }

        public void calculate(Grid grid, Random rng)
        {
            Queue<Gene> order = new Queue<Gene>();
            List<Gene> copy = new List<Gene>();
            //copy gene list
            for(int i = 0; i < genes.Count(); i++)
            {
                copy.Add(genes[i]);
            }

            //order of queue
            //genes with same inner on from and to
            for(int i = 0; i < copy.Count; i++)
            {
                if(copy[i].from.type == NeuronType.Inner && copy[i].to.type == NeuronType.Inner)
                {
                    if(((Inner)copy[i].from).number == ((Inner)copy[i].to).number)
                    {
                        order.Enqueue(copy[i]);
                        copy.RemoveAt(i);
                        i--;
                    }
                }
            }

            //genes with sensory on from
            for (int i = 0; i < copy.Count; i++)
            {
                if (copy[i].from.type == NeuronType.Sensory)
                {
                    order.Enqueue(copy[i]);
                    copy.RemoveAt(i);
                    i--;
                }
            }

            //genes with inner neuron to different inner neuron
            for (int i = 0; i < copy.Count; i++)
            {
                if (copy[i].from.type == NeuronType.Inner && copy[i].to.type == NeuronType.Inner)
                {
                    order.Enqueue(copy[i]);
                    copy.RemoveAt(i);
                    i--;
                }
            }

            //genes with action neurons as to
            for (int i = 0; i < copy.Count; i++)
            {
                if (copy[i].to.type == NeuronType.Action)
                {
                    order.Enqueue(copy[i]);
                    copy.RemoveAt(i);
                    i--;
                }
            }

            //shouldnt be anything left but just in case
            for (int i = 0; i < copy.Count; i++)
            {
                order.Enqueue(copy[i]);
            }


            //calculate sums based on queue order

            while(order.Count != 0)
            {
                Gene g = order.Dequeue();

                if (g.from.type == NeuronType.Sensory)
                {
                    g.to.sum += g.strength * ((Sensory)g.from).output(grid, this, rng);
                }
                else if (g.from.type == NeuronType.Inner)
                    g.to.sum += g.strength * ((Inner)g.from).output();
            }
        }

        public void resetNeurons()
        {
            for(int i = 0; i < senses.Count; i++)
            {
                senses[i].sum = 0;
            }
            for (int i = 0; i < inners.Count; i++)
            {
                inners[i].sum = 0;
            }
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].sum = 0;
            }
        }

        public Color makeColor()
        {
            Converter convert = new Converter();
            int[] hex = new int[6];
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < genes.Count; j++)
                {
                    hex[i] += convert.hexToInt(genes[j].code.Substring(i, 1));
                }
                hex[i] = hex[i] / genes.Count;
            }

            string c = "";
            for (int i = 0; i < 6; i++)
            {
                c += convert.binaryToHex(convert.intToBinary(hex[i], 4));
            }

            return convert.FromHex(c);

        }


    }
}
