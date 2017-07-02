using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi.Services
{
    public class ContextMenuBuilder
    {
        ContextMenuStrip mnu = new ContextMenuStrip();
        public ContextMenuBuilder(IEnumerable<MenuItem> menuItems)
        {
            mnu = new ContextMenuStrip();
            ToolStripMenuItem toolStrip;
            foreach (MenuItem item in menuItems)
            {
                toolStrip = new ToolStripMenuItem(item.getTitle(),item.getImage(),item.getListener(),item.getName());
                mnu.Items.Add(toolStrip);

            }
        }
        public void addToExistingStrip(string existMenuName, string newMenuName, EventHandler eventListener)
        {
            ToolStripMenuItem existingMenu = (ToolStripMenuItem)mnu.Items.Find(existMenuName, true).First();
            if (existingMenu == null) return;
            ToolStripMenuItem newMenu = new ToolStripMenuItem(newMenuName);
            newMenu.Click += eventListener;
            existingMenu.DropDownItems.Add(newMenu);
        }
        public ContextMenuStrip getContext()
        {
            return mnu;
        }
    }
}
