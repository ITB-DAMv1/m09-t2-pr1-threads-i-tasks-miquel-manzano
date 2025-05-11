using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidePerConsola
{
    class Spaceship
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Spaceship(int x, int y) => (X, Y) = (x, y);
    }
}
