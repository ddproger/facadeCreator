using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public class StringResources
    {
        public static string getAbsolutePath()
        {
            return "C:\\InSitu";
        }
        public static string getCatalogsPath()
        {
            return "C:\\InSitu\\Catalogs";
        }
        public static string getObjectLabelText()
        {
            return "PANPLACE";
        }
        public static string getResourcesPath()
        {
            return getAbsolutePath()+"\\Textures\\images";   
        }
        public static string getResourcePathWithoutAbsolute()
        {
            return "images";
        }
    }
}
