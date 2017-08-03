using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public class SavedState
    {
        private Dictionary<FigureOnBoard,FigureOnBoard> figures;

        public SavedState(IEnumerable<FigureOnBoard> figures)
        {
            this.figures = new Dictionary<FigureOnBoard, FigureOnBoard>();
            foreach (var item in figures)
            {
                this.figures.Add(item, new FigureOnBoard(item.figure.copy(), item.x, item.y));
            }
        }
        public void returnToState()
        {
            FigureOnBoard currentFigude;
            foreach (var item in figures)
            {
                currentFigude = item.Key;
                currentFigude.x = item.Value.x;
                currentFigude.y = item.Value.y;
                
            }
        }
    }
}
