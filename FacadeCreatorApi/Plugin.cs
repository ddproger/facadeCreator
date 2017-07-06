using FacadeCreatorApi.Forms;
using FacadeCreatorApi.models;
using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi
{
    public class Plugin
    {
        KD.SDK.Appli _appli;

        public Plugin()
        {
            _appli = new KD.SDK.Appli();
        }

        public KD.SDK.Appli CurrentAppli
        {
            get
            {
                return _appli;
            }
        }

        public bool OnPluginLoad(int iCallParamsBlock)
        {
            KdSdkApiImpl kdApi = new KdSdkApiImpl(iCallParamsBlock);
            //kdApi.updatePalitra();
            AddMenu(iCallParamsBlock);
            return true;
        }
        public bool OnPluginUnload(int lCallParamBlock)
        {
            return true;
        }

        public bool AddMenu(int iCallParamsBlock)
        {
            //System.Windows.Forms.MessageBox.Show("AddMenu");
            string MENU_ITEM_TEXT = "Создать изображение на фасаде";
            string MENU_ITEM_PLUGIN_NAME = "FacadeCreatorApi.dll"; // can be an external plugin dll like "KD.Plugin.Tiny.dll" must be declared in SPACE.INI in dll case
                                                                 //string MENU_ITEM_PLUGIN_NAME = "KD.Plugin.Tiny.dll"; // can be an external plugin dll like "KD.Plugin.Tiny.dll" must be declared in SPACE.INI in dll case
            string MENU_ITEM_PLUGIN_METHOD_NAME = "CallMe"; // can be an external plugin dll like "KD.Plugin.Tiny.dll" must be declared in SPACE.INI in dll case
            int menuID = CurrentAppli.InsertMenuItemFromId(MENU_ITEM_TEXT, /* menuItemText */
                                                            KD.SDK.AppliEnum.ControlKey.CK_CONTROL, /* accelControlKey */
                                                            KD.SDK.AppliEnum.VirtualKeyCode.VirtualKey_M, /* accelKeyCode */
                                                            String.Empty, /* iconFileName */
                                                            true, /* InsertBefore */
                                                            (int)KD.SDK.AppliEnum.ObjectMenuItemsId.INFOS, /* menuPosition */
                                                            MENU_ITEM_PLUGIN_NAME, /* pluginDllFileName, */
                                                            "Plugin", /* className */
                                                            MENU_ITEM_PLUGIN_METHOD_NAME/* functionName */);

            if (menuID <= 0)
            {
                return false;
            }

           // System.Windows.Forms.MessageBox.Show("InsertMenuItemFromId was succesful." + Environment.NewLine + "Check for menu item Place | \"" + MENU_ITEM_TEXT + "\"" + Environment.NewLine + "Method " + MENU_ITEM_PLUGIN_METHOD_NAME + "() must exist in " + MENU_ITEM_PLUGIN_NAME);

            return true;
        }

        public bool CallMe(int iCallParamsBlock)
        {
            MainForm frm = new MainForm();    

            KdSdkApi kdApi = new KdSdkApiImpl(iCallParamsBlock);
            Scenes scenes = new Scenes(frm,kdApi);
            ICollection<FigureOnBoard> facades = kdApi.getFacades();
            foreach (FigureOnBoard item in facades)
            {
                scenes.addFigure(item.figure, item.x, item.y);
            }
            //scenes.addFigure(new Facade(21, 100, 100), 0, 0);
            frm.ShowDialog();
            return true;
        }
    }
}
