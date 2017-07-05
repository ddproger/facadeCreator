using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public interface MenuItem
    {
        string getName();
        string getTitle();
        Image getImage(); 
        EventHandler getListener();
    }
}
