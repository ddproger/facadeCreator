using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public interface KdSdkApi
    {
        ICollection<Facade> getFacades();
        void applyFacadeImage(Image img);
        bool OnFileOpenBeforeAfter(int lCallParamsBlock);
        bool OnPluginLoad(int lCallParamsBlock);

    }
}
