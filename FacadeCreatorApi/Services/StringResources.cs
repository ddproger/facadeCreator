using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public class StringResources
    {
        private static KD.SDK.Appli kdApi;
        private static string ABSOLUTE_PATH;
        private static string CATALOGS_PATH;
        private static string SCENES_PATH;
        static StringResources(){
            kdApi = new KD.SDK.Appli();
             ABSOLUTE_PATH = kdApi.AppliGetInfo(KD.SDK.AppliEnum.Info.EXE_DIR);
            CATALOGS_PATH = kdApi.AppliGetInfo(KD.SDK.AppliEnum.Info.CATALOGS_DIR);
            SCENES_PATH = kdApi.AppliGetInfo(KD.SDK.AppliEnum.Info.SCENES_DIR);
            
            kdApi = null;
        }

        internal static string getPathToSpaceIni()
        {
            return getAbsolutePath() + "\\space.ini";
        }

        public static string getAbsolutePath()
        {
            return ABSOLUTE_PATH;
        }
        public static string getCatalogsPath()
        {
            return CATALOGS_PATH;
        }
        public static string getObjectLabelText()
        {
            return "PANPLACE";
        }
        public static string getResourcesPath()
        {
            string result = IniFileReader.getValueFromFile("InSitu", "TexturesDir"); 
            if (result == null || result.Equals("")) result = IniFileReader.getValueFromFile("Local", "TexturesDir");
            
            return result; //getAbsolutePath()+"\\Textures";   
        }
        public static string getImageDirectoryName()
        {
            return "images";
        }
    }
}
