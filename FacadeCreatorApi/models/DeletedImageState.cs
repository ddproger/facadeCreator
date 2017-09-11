using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public class DeletedImageState : CanSaveState
    {
        private FigureOnBoard figure;
        private FiguresCollection collection;
        public DeletedImageState(FiguresCollection collection, FigureOnBoard figure)
        {
            this.figure = figure;
            this.collection = collection;
        }
        public void backToPrevious()
        {
            collection.add(figure);
        }

        public void saveState()
        {
            //throw new NotImplementedException();
        }
    }
}
