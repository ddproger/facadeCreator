using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public class filledBkgImages:CanSaveState
    {
        FiguresCollection collection;
        deletedManyImageState deletedManyImagesState;
        public filledBkgImages(FiguresCollection collection)
        {
            deletedManyImagesState = new deletedManyImageState(collection);
            this.collection = collection;
        }

        public void backToPrevious()
        {
            collection.removeAll();
            deletedManyImagesState.backToPrevious();
        }

        public void saveState()
        {
            deletedManyImagesState.saveState();
        }
    }
}
