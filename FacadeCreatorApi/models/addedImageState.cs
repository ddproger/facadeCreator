using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public class addedImageState:CanSaveState        
    {
        FigureOnBoard image;

        FiguresCollection collection;
        public addedImageState(FiguresCollection collection, FigureOnBoard image)
        {
            this.collection = collection;
            this.image = image;
        }

        public void backToPrevious()
        {
            collection.remove(image);
        }

        public void saveState()
        {
            
        }
    }
}
