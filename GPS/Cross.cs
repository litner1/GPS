using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    public class Cross
    { 
        public int index;
        public int x;
        public int y;
        public List<int> neighbours = new List<int>();
        public List<double> distance = new List<double>();
        public List<double> velocity = new List<double>();
    }
}
