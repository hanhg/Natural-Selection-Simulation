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
    class Gene
    {
        public Neuron from;
        public Neuron to;
        public double strength;
        public string code;
        public Gene(Neuron from, Neuron to, double strength, int senseMax, int numberMax, int actionMax)
        {
            this.from = from;
            this.to = to;
            this.strength = strength;

            code = getCode(senseMax, numberMax, actionMax);
        }

        //copy the gene
        public Gene(Gene copy, List<Sensory> senses, List<Inner> inners, List<Action> actions)
        {
            if(copy.from.type == NeuronType.Sensory)
                from = senses[(int)((Sensory)copy.from).sense];
            else
                from = inners[(int)((Inner)copy.from).number];

            if (copy.to.type == NeuronType.Action)
                to = actions[(int)((Action)copy.to).action];
            else
                to = inners[(int)((Inner)copy.to).number];
            strength = copy.strength;
            code = copy.code;
        }

        public double output(Grid grid, Creature thisCreature, Random rng)
        {
            if (from.type == NeuronType.Sensory)
            {
                Sensory neuron = (Sensory)from;
                return strength * neuron.output(grid, thisCreature, rng);
            }
            if (from.type == NeuronType.Inner)
            {
                Inner neuron = (Inner)from;
                return strength * neuron.output();
            }
            if (from.type == NeuronType.Action)
            {
                Action neuron = (Action)from;
                return strength * neuron.output();
            }
            return 0.0;
        }

        //if two genes are connected to the same neurons on the same ends
        public bool isSimilar(Gene other)
        {
            bool isFromSame = false;
            bool isToSame = false;
            if (from.type == other.from.type)
            {
                if(from.type == NeuronType.Sensory)
                {
                    Sensory f = (Sensory)from;
                    Sensory fOther = (Sensory)other.from;
                    if (f.sense == fOther.sense)
                        isFromSame = true;
                }
                else if(from.type == NeuronType.Inner)
                {
                    Inner f = (Inner)from;
                    Inner fOther = (Inner)other.from;
                    if (f.number == fOther.number)
                        isFromSame = true;
                }
                else if (from.type == NeuronType.Action)
                {
                    Action f = (Action)from;
                    Action fOther = (Action)other.from;
                    if (f.action == fOther.action)
                        isFromSame = true;
                }
            }
            if (to.type == other.to.type)
            {
                if (to.type == NeuronType.Sensory)
                {
                    Sensory t = (Sensory)to;
                    Sensory tOther = (Sensory)other.to;
                    if (t.sense == tOther.sense)
                        isToSame = true;
                }
                else if (to.type == NeuronType.Inner)
                {
                    Inner t = (Inner)to;
                    Inner tOther = (Inner)other.to;
                    if (t.number == tOther.number)
                        isToSame = true;
                }
                else if (to.type == NeuronType.Action)
                {
                    Action t = (Action)to;
                    Action tOther = (Action)other.to;
                    if (t.action == tOther.action)
                        isToSame = true;
                }
            }

            return isFromSame && isToSame;
        }

        public bool isSimilar(Neuron otherFrom, Neuron otherTo)
        {
            bool isFromSame = false;
            bool isToSame = false;
            if (from.type == otherFrom.type)
            {
                if (from.type == NeuronType.Sensory)
                {
                    Sensory f = (Sensory)from;
                    Sensory fOther = (Sensory)otherFrom;
                    if (f.sense == fOther.sense)
                        isFromSame = true;
                }
                else if (from.type == NeuronType.Inner)
                {
                    Inner f = (Inner)from;
                    Inner fOther = (Inner)otherFrom;
                    if (f.number == fOther.number)
                        isFromSame = true;
                }
                else if (from.type == NeuronType.Action)
                {
                    Action f = (Action)from;
                    Action fOther = (Action)otherFrom;
                    if (f.action == fOther.action)
                        isFromSame = true;
                }
            }
            if (to.type == otherTo.type)
            {
                if (to.type == NeuronType.Sensory)
                {
                    Sensory t = (Sensory)to;
                    Sensory tOther = (Sensory)otherTo;
                    if (t.sense == tOther.sense)
                        isToSame = true;
                }
                else if (to.type == NeuronType.Inner)
                {
                    Inner t = (Inner)to;
                    Inner tOther = (Inner)otherTo;
                    if (t.number == tOther.number)
                        isToSame = true;
                }
                else if (to.type == NeuronType.Action)
                {
                    Action t = (Action)to;
                    Action tOther = (Action)otherTo;
                    if (t.action == tOther.action)
                        isToSame = true;
                }
            }

            return isFromSame && isToSame;
        }

        public string getCode(int senseMax, int numberMax, int actionMax)
        {
            Converter convert = new Converter();
            string binary = "";
            //first bit
            if(from.type == NeuronType.Sensory)
            {
                binary += 0;
                //next 7 bits
                int s = (int)(Math.Pow(2, 7)-1) / senseMax;
                Sensory neuron = (Sensory)from;
                binary += convert.intToBinary((int)neuron.sense * s,7);
            }
            else
            {
                binary += 1;
                //next 7 bits
                int i = (int)(Math.Pow(2, 7) - 1) / numberMax;
                Inner neuron = (Inner)from;
                binary += convert.intToBinary((int)neuron.number * i, 7);
            }

            //next bit
            if (to.type == NeuronType.Action)
            {
                binary += 0;
                //next 7 bits
                int a = (int)(Math.Pow(2, 7) - 1) / actionMax;
                Action neuron = (Action)to;
                binary += convert.intToBinary((int)neuron.action * a, 7);
            }
            else
            {
                binary += 1;
                //next 7 bits
                int i = (int)(Math.Pow(2, 7) - 1) / numberMax;
                Inner neuron = (Inner)to;
                binary += convert.intToBinary((int)neuron.number * i, 7);
            }

            //next 8 bits
            int w = (int)(Math.Pow(2, 8) - 1) / 8;
            double w0 = strength + 4;
            binary += convert.intToBinary((int)(w*w0), 8);

            return convert.binaryToHex(binary);
        }
    }
}

