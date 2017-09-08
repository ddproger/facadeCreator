using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public class FigureOnBoard:CanSaveState
    {
        Stack<System.Drawing.Point> savedStates = new Stack<System.Drawing.Point>();
        public int x { get; set; }
        public int y { get; set; }
        public Figure figure { get; set; }

        public FigureOnBoard(Figure figure, int x, int y)
        {
            this.figure = figure;
            this.x = x;
            this.y = y;
        }

        public void saveState()
        {
            figure.saveState();
            savedStates.Push(new System.Drawing.Point(this.x, this.y));
        }

        public void backToPrevious()
        {
            if (savedStates.Count == 0) return;
            System.Drawing.Point point = savedStates.Pop();
            this.x = point.X;
            this.y = point.Y;
            figure.backToPrevious();
        }
    }
}
