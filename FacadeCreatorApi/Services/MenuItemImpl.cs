using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    class MenuItemImpl : MenuItem
    {
        private string name, title;
        private EventHandler listener;
        private Image image;
        public MenuItemImpl(string name, string title,Image image, EventHandler listener)
        {
            this.name = name;
            this.title = title;
            this.image = image;
            this.listener = listener;
        }

        public Image getImage()
        {
            return image;
        }

        public EventHandler getListener()
        {
            return listener;
        }

        public string getName()
        {
            return name;
        }

        public string getTitle()
        {
            return title;
        }
    }
}
