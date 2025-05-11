using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidePerConsola
{
    class Asteroid
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Asteroid(int x, int y) => (X, Y) = (x, y);
    }
}
