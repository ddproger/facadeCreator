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
    public enum Action{
        SIZE_NS, SIZE_WE, SIZE_NWSE,SIZE_NWSE_WR,SHIFT, NO
    }
    public abstract class Figure
    {
        private const int MIN_WIDTH = 5;
        private const int MIN_HEIGHT = 5;

        protected static int LINE_SIZE = 2;
        protected Action currentAction = Action.NO; 
        protected bool selected = false;
        public int width { get; set; }
        public int height { get; set; }

        float resolution;
        public Figure(int width, int height)
        {
            this.width = width;
            this.height = height;
            updateResolution();
        }

        public void updateResolution()
        {
            resolution = height*1.0f / width;
        }

        public void selectFigure()
        {
            selected = true;
        }

        public void unselectFigure()
        {
            selected = false;
        }

        public bool isPointInFigure(int x, int y)
        {
            if (x < 0 || y < 0) return false;
            if (x <= width && y <= height) return true;
            else return false;
        }
        public void setSizeFromForm(Figure fig)
        {
            this.width = fig.width;
            this.height = fig.height;
        }
        public Figure getForm()
        {
            return new FigureForm(width, height);
        }
        public virtual void draw(Graphics context, int x, int y)
        {
            if (selected)
            {
                context.DrawRectangle(new Pen(Color.Black, LINE_SIZE), x, y, width, height);
                context.DrawRectangle(new Pen(Color.Black, LINE_SIZE), x + (width / 2) - LINE_SIZE, y + height - LINE_SIZE, LINE_SIZE * 2, LINE_SIZE * 2);
                context.DrawRectangle(new Pen(Color.Black, LINE_SIZE), x + width - LINE_SIZE, y + (height / 2) - LINE_SIZE, LINE_SIZE * 2, LINE_SIZE * 2);
                context.DrawRectangle(new Pen(Color.Black, LINE_SIZE), x + width - LINE_SIZE, y + height - LINE_SIZE, LINE_SIZE * 2, LINE_SIZE * 2);
            }
        }
    
        public Cursor getCursor(int x, int y, bool cntrl)
        {
            currentAction = getAction(x, y, cntrl);
            switch (currentAction)
            {
                case Action.SHIFT: return Cursors.SizeAll;
                case Action.SIZE_NS: return Cursors.SizeNS;
                case Action.SIZE_WE: return Cursors.SizeWE;
                case Action.SIZE_NWSE:
                case Action.SIZE_NWSE_WR: return Cursors.SizeNWSE;
                default: return Cursors.Default;
            }

        }
        internal void Scale(int x, int y)
        {
            switch (currentAction)
            {
                case Action.SIZE_WE:if(x >= MIN_WIDTH) width = x; break;
                case Action.SIZE_NS:if(y >= MIN_HEIGHT) height = y; break;
                case Action.SIZE_NWSE_WR:
                    if (x >= MIN_WIDTH) width = x;
                    if (y >= MIN_HEIGHT) height = y;
                    break;
                case Action.SIZE_NWSE:
                    if (x < MIN_WIDTH || y < MIN_HEIGHT) return;
                    if (x - width < y - height)
                    {
                        width = x;
                        height = (int)(width * resolution);
                    }
                    else
                    {
                        height = y;
                        width = (int)(height / resolution);
                    }
                    /*
                    float ratio = 0f;
                    if (width > height)
                        ratio = x * 1f / width;
                    else
                        ratio = y*1f / height;
                    width = (int)(width * ratio);
                    height = (int)(height * ratio);
                    */
                    break;
            }
        }

        public abstract Figure copy();

        public abstract Action getAction(int x, int y, bool cntrl);
        public Action getAction()
        {
            return currentAction;
        }
        public void setAction(Action action)
        {
            this.currentAction = action;
        }
    }
    class FigureForm : Figure
    {
        public FigureForm(int width, int height) : base(width, height)
        {
        }

        public override Figure copy()
        {
            return new FigureForm(width, height);
        }

        public override void draw(Graphics context, int x, int y)
        {
            base.draw(context, x, y);
            context.DrawRectangle(new Pen(Color.Gray, 3), x, y, width, height);
        }

        public override Action getAction(int x, int y, bool cntrl)
        {
            if (isPointInFigure(x, y))
            {
                return Action.SHIFT;
            }
            else return Action.NO;
        }
     
        
    }
}
