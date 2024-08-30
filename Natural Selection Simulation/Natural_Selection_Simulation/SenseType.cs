using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Natural_Selection_Simulation
{
    enum SenseType
    {
        Age, //age of creature
        Rnd, //randum input
        BDx, //Border distance east/west
        BDy, //Border distance north/south
        BD, //nearest border distance overall
        Lx, //location east/west
        Ly, //location north/south
    }
}
