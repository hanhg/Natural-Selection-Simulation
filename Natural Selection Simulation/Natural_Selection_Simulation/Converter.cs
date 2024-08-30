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
    class Converter
    {
        public Converter()
        {}

        public Color FromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception("Color not valid");

            return new Color(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }
        public string intToBinary(int value, int numOfBits)
        {
            string rtn = "";
            for(int i = numOfBits-1; i >= 0; i--)
            {
                if (value - Math.Pow(2, i) < 0)
                    rtn += "0";
                else
                {
                    rtn += "1";
                    value -= (int)Math.Pow(2, i);
                }
            }
            return rtn;
        }
        public int binaryToInt(string binary)
        {
            int rtn = 0;
            for (int i = binary.Length - 1; i >= 0; i--)
            {
                if (binary.Substring(binary.Length - 1 - i, 1).Equals("1"))
                    rtn += (int)Math.Pow(2,i);
            }
            return rtn;
        }

        public string binaryToHex(string binary) // must have a length of a multiple of 4
        {
            string rtn = "";
            for(int i = 0; i < binary.Length; i += 4)
            {
                string temp = "";
                for(int j = 0; j < 4; j++)
                {
                    temp += binary.Substring(i+j, 1);
                }
                int part = binaryToInt(temp);
                if (part < 10)
                    rtn += part;
                else if (part == 10)
                    rtn += "A";
                else if (part == 11)
                    rtn += "B";
                else if (part == 12)
                    rtn += "C";
                else if (part == 13)
                    rtn += "D";
                else if (part == 14)
                    rtn += "E";
                else if (part == 15)
                    rtn += "F";
            }
            return rtn;
        }

        public int hexToInt(string hex)
        {
            int rtn = 0;
            for(int i = 0; i < hex.Length; i++)
            {
                if(hex.Substring(i,1).Equals("0"))
                    rtn += 0;
                else if (hex.Substring(i, 1).Equals("1"))
                    rtn += 1;
                else if (hex.Substring(i, 1).Equals("2"))
                    rtn += 2;
                else if (hex.Substring(i, 1).Equals("3"))
                    rtn += 3;
                else if (hex.Substring(i, 1).Equals("4"))
                    rtn += 4;
                else if (hex.Substring(i, 1).Equals("5"))
                    rtn += 5;
                else if (hex.Substring(i, 1).Equals("6"))
                    rtn += 6;
                else if (hex.Substring(i, 1).Equals("7"))
                    rtn += 7;
                else if (hex.Substring(i, 1).Equals("8"))
                    rtn += 8;
                else if (hex.Substring(i, 1).Equals("9"))
                    rtn += 9;
                else if (hex.Substring(i, 1).Equals("A"))
                    rtn += 10;
                else if (hex.Substring(i, 1).Equals("B"))
                    rtn += 11;
                else if (hex.Substring(i, 1).Equals("C"))
                    rtn += 12;
                else if (hex.Substring(i, 1).Equals("D"))
                    rtn += 13;
                else if (hex.Substring(i, 1).Equals("E"))
                    rtn += 14;
                else if (hex.Substring(i, 1).Equals("F"))
                    rtn += 15;
            }
            return rtn;
        }
    }
}
