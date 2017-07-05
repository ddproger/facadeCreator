using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi.models
{
    [ComVisible(false)]
    public class BkgImage : Figure
    {
        private static int constScale=5;
        private Image img;
        public BkgImage(Image img) : base(img.Width, img.Height)
        {
            this.img = img;
        }

        public override Figure copy()
        {
            BkgImage newBkgImage = new BkgImage(img);
            newBkgImage.height = this.height;
            newBkgImage.width = this.height;
            newBkgImage.updateResolution();
            return newBkgImage;
        }

        public override void draw(Graphics context, int x, int y)
        {
            context.DrawImage(img,x,y,width,height);
            base.draw(context, x, y);
        }

        public override Action getAction(int x, int y, bool cntrl)
        {
            if (!isPointInFigure(x, y))
            {
                currentAction = Action.NO;
                return currentAction;
            }
            int c = 0;
            Action action = Action.SHIFT;
            if (Math.Abs(x - width) < 5)
            {
                action = Action.SIZE_WE;
                c++;
            }
            if (Math.Abs(y - height) < 5)
            {
                action = Action.SIZE_NS;
                c++;
            }
            if (c == 2) currentAction = Action.SIZE_NWSE;
            //else if (c == 2 && !cntrl) currentAction = Action.SIZE_NWSE_WR;
            else currentAction = action;
            return currentAction;
        }
        
    }
}
