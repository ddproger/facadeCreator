using FacadeCreatorApi.models;
using System.Collections.Generic;

namespace FacadeCreatorApi.Services
{

    public interface KdSdkApi
    {
        void applyFacadeImage(IDictionary<Facade, string> facades);
        ICollection<FigureOnBoard> getFacades();
        string getScenesName();
        void applyFacadeImageHowSandblast(IDictionary<Facade, string> facades);
    }
}
