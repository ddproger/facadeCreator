using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public class deletedManyImageState : CanSaveState
    {
        private FiguresCollection collection;
        private FiguresCollection savedCollection;
        public deletedManyImageState(FiguresCollection collection)
        {
            this.collection = collection;
            
        }
        public void backToPrevious()
        {
            foreach (FigureOnBoard item in savedCollection)
            {
                collection.add(item);
            }
        }

        public void saveState()
        {
            this.savedCollection = new FiguresCollectionImpl();
            foreach (FigureOnBoard item in collection)
            {
                savedCollection.add(item);
            }
        }
    }
}
