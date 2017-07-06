using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{

    public interface KdSdkApi
    {
        void applyFacadeImage(IDictionary<Facade, string> facades);
        ICollection<FigureOnBoard> getFacades();
        string getScenesName();
    }
}
