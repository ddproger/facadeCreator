using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public class FigureOnBoard
    {
        public int x { get; set; }
        public int y { get; set; }
        public Figure figure { get; set; }

        public FigureOnBoard(Figure figure, int x, int y)
        {
            this.figure = figure;
            this.x = x;
            this.y = y;
        }
        
    }
}
