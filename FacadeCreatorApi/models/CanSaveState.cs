using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public interface CanSaveState
    {
        void saveState();
        void backToPrevious();
    }
}
