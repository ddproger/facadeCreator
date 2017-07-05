
using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FacadeCreatorApi.Services
{
    
    [ ComVisible(true)]
    [ProgId("FacadeCreatorApi.Services.KdSdkApiImpl"), 
        ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("415A6DF1-42B1-3AAF-B1FA-EBD7A7C04418")]
    public class KdSdkApiImpl
    {
        private int sessionId;
        private string languageCode;
        private int bOk;
        [ComVisible(true)]
        public static void Main(String[] args)
        {
            System.Windows.Forms.MessageBox.Show("Main");
        }
        [ComVisible(false)]
        public void applyFacadeImage(Image img)
        {
            throw new NotImplementedException();
        }
        [ComVisible(false)]
        public ICollection<Facade> getFacades()
        {
            throw new NotImplementedException();
        }
        [ComVisible(true)]
        public  bool OnFileOpenBeforeAfter(int lCallParamsBlock)
        {
            System.Windows.Forms.MessageBox.Show("OnFileOpenBeforeAfter");
            return true;
        }
        [ComVisible(true)]
        public bool OnAppStartAfter(int lCallParamsBlock)
        {
            KD.SDK.Appli appli = new KD.SDK.Appli();
            sessionId = appli.StartSessionFromCallParams(lCallParamsBlock);
            if (sessionId != 0)
            {
                languageCode = appli.GetLanguage();

                bOk = appli.InsertMenuItem("", KD.SDK.AppliEnum.ControlKey.CK_CONTROL, KD.SDK.AppliEnum.VirtualKeyCode.VirtualKey_F, "", 8, 11, "FacadeKreatorApi.dll", "FacadeCreatorApi.Services.KdSdkApiImpl", "Main");
                appli.EndSession();
            }
            System.Windows.Forms.MessageBox.Show("OnAppStartAfter");
            return true;
        }
        [ComVisible(true)]
        public bool OnPluginLoad(int iCallParamsBlock)
        {
            System.Windows.Forms.MessageBox.Show("OnPluginLoad");
            return true;
        }

    }
}
