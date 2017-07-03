using FacadeCreatorApi.models;
using FacadeCreatorApi.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi
{
    public class Scenes
    {
        Control canvas;
        int offsetX, offsetY;
        float scale = 1;
        float scaleStep=0.1f;
        bool cntrEnabled = false;
        bool isMouseDown = false;

        FiguresCollection bkgImages;
        FiguresCollection facades;

        FigureOnBoard selectedFigure;
        FigureOnBoard bufferedFigure;
        private Point clickPoint=new Point();
        private const float MAX_ZOOM = 3;
        private const int SHIFT_STEP = 5;
        private const int ARRAY_SIZE_STEP = 10;

        ContextMenuStrip mnuFigure;
        ContextMenuStrip mnuCanvas;
        private Image pastingImage;

        private ContextMenuStrip createMenuCanvas()
        {
            LinkedList<Services.MenuItem> menuItems = new LinkedList<Services.MenuItem>();
            menuItems.AddLast(new MenuItemImpl("mnuAddmage", "Добавить изображение", null, mnuAddImage_Click));
            menuItems.AddLast(new MenuItemImpl("mnuPaste", "Вставить", null, null));
            menuItems.AddLast(new MenuItemImpl("mnuComplete", "Обрезать и применить", null, mnuComplete_Click));


            ContextMenuBuilder menuBuilder = new ContextMenuBuilder(menuItems);
           
            return menuBuilder.getContext();
        }

        private void mnuAddImage_Click(object sender, EventArgs e)
        {
            Image newImage = DialogsService.getImageFromDisk("c:\\");
            if (newImage != null)
            {
                pastingImage = newImage;
            }
        }

        private void mnuComplete_Click(object sender, EventArgs e)
        {
           Bitmap image = generateFullGrapics();
           ImageConversion.generateFacades(image, bkgImages);
        }

        private Bitmap generateFullGrapics()
        {
            Point maxSize = getBordersOfCanvas();
            Bitmap newImage = new Bitmap(maxSize.X, maxSize.Y);
            Graphics graphics = Graphics.FromImage(newImage);
            graphics.FillRectangle(Brushes.White, 0, 0, maxSize.X, maxSize.Y);
            foreach (FigureOnBoard item in bkgImages)
            {
                if(item.figure is BkgImage)item.figure.draw(graphics, item.x, item.y);
            }
            return newImage;
        }

        private Point getBordersOfCanvas()
        {
            int maxX=0, maxY=0, curX, curY;
            foreach (FigureOnBoard item in bkgImages)
            {
                curX = item.x + item.figure.width;
                curY = item.y + item.figure.height;
                if (curX > maxX) maxX = curX;
                else if (curY > maxY) maxY = curY;
            }
            return new Point(maxX, maxY);
        }

        private ContextMenuStrip createMenuFigure()
        {
            LinkedList<Services.MenuItem> menuItems = new LinkedList<Services.MenuItem>();
            menuItems.AddLast(new MenuItemImpl("mnuDelete", "Удалить", null, mhuDeleteFigure_Click));
            menuItems.AddLast(new MenuItemImpl("mnuPosition", "Положение", null, null));
            menuItems.AddLast(new MenuItemImpl("mnuCopy", "Копировать", null, mnuCopy_Click));
                      

            ContextMenuBuilder menuBuilder = new ContextMenuBuilder(menuItems);
            menuBuilder.addToExistingStrip("mnuPosition", "На уровень выше", mnuUp_Click);
            menuBuilder.addToExistingStrip("mnuPosition", "На уровень ниже", mnuLower_Click);
            menuBuilder.addToExistingStrip("mnuPosition", "На передний план", mnuToFrontClick);
            menuBuilder.addToExistingStrip("mnuPosition", "На задний план", mnuToBack_Click);

            return menuBuilder.getContext();
        }

        private void mnuLower_Click(object sender, EventArgs e)
        {
            int index = getIndex(selectedFigure);
            if (index > 0)
            {
                shufle(index, index-1);
            }
        }

        private void shufle(int index, int v)
        {
            bkgImages.shuffle(index, v);
            UpdateGraphics();
        }

        private void mnuUp_Click(object sender, EventArgs e)
        {
            int index = getIndex(selectedFigure);
            if (index != -1&&index!=bkgImages.getCount()-1)
            {
                move(index, index+1);
            }
        }

        private void mnuToBack_Click(object sender, EventArgs e)
        {
            int index = getIndex(selectedFigure);
            if (index != -1)
            {
                move(index, 0);
            }            
        }

        private void move(int from, int to)
        {
            bkgImages.move(from, to);
            UpdateGraphics();
        }

        private int getIndex(FigureOnBoard selectedFigure)
        {
            return bkgImages.getIndex(selectedFigure);
        }

        private void mnuToFrontClick(object sender, EventArgs e)
        {
            int index = getIndex(selectedFigure);
            if (index != -1)
            {
                move(index, bkgImages.getCount()-1);
            }
        }

        private void mnuPaste_Click(object sender, EventArgs e)
        {
            pasteBuferedFigure(bufferedFigure.x,bufferedFigure.y);
        }

        private void pasteBuferedFigure(int x, int y)
        {
            addFigure(bufferedFigure.figure.copy(),x+20,y+20);
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            copySelectedFigure();
        }

        private void copySelectedFigure()
        {
            if (selectedFigure != null) bufferedFigure = selectedFigure;
        }

        private void mhuDeleteFigure_Click(object sender, EventArgs e)
        {
            if (selectedFigure != null) {
                deleteFigure(selectedFigure);
                UpdateGraphics();
            }
        }

        private void deleteFigure(FigureOnBoard selectedFigure)
        {
            bkgImages.remove(selectedFigure);
        }
        public Scenes(Control canvas)
        {
            this.canvas = canvas;

            bkgImages = new FiguresCollectionImpl();

            Image newImage = Image.FromFile("C:\\Users\\gerasymiuk\\Documents\\Visual Studio 2017\\Projects\\FacadeCreator\\FacadeCreatorApi\\bin\\Debug\\1.png");
            addFigure(new BkgImage(newImage), 0, 0);

            addFigure(new Facade(1,100, 150), 140, 5);
            addFigure(new Facade(2,100,150),140,5);
            addFigure(new Facade(3,100, 150), 340, 5);      


            mnuFigure = createMenuFigure();
            mnuCanvas = createMenuCanvas();

            canvas.MouseDown += new MouseEventHandler(mouseDown);
            canvas.MouseMove += new MouseEventHandler(mouseMove);
            canvas.MouseUp += new MouseEventHandler(mouseUp);
            canvas.MouseWheel += startScale;
            canvas.KeyDown += new KeyEventHandler(keyDown);
            canvas.KeyUp += new KeyEventHandler(keyUp);
            canvas.Paint += new PaintEventHandler(paint);

            //FiguresCollection col = new FiguresCollectionImpl();
            //col.add(new FigureOnBoard(new Facade(1,10, 20), 12, 21));
            //col.add(new FigureOnBoard(new Facade(2,10, 30), 12, 22));
            //col.add(new FigureOnBoard(new Facade(3,10, 40), 12, 23));
            //foreach (FigureOnBoard item in col.getReverseCollection())
            //{
            //    MessageBox.Show(item.y.ToString());
            //}
        }

        private void addFigure(Figure figure, int x, int y)
        {
            bkgImages.add(new FigureOnBoard(figure, x, y));
            UpdateGraphics();
        }

        private void paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            paintInGraphics(e.Graphics, scale, offsetX, offsetY);        
        }

        private void paintInGraphics(Graphics graphics, float scale, int offsetX, int offsetY)
        {
            graphics.TranslateTransform(offsetX, offsetY);
            graphics.ScaleTransform(scale, scale);
            foreach (FigureOnBoard item in bkgImages)
            {
                item.figure.draw(graphics, item.x, item.y);
            }
        }

        private void startScale(object sender, MouseEventArgs e)
        {
            if (cntrEnabled&&e.Delta!=0)
            {
                if (e.Delta > 0) ZoomIn();
                else ZoomOut();
                UpdateGraphics();
            }
           
        }
        private void ZoomIn()
        {
            if (scale <= MAX_ZOOM)
            {
                scale += scaleStep;
            }
        }
        private void ZoomOut()
        {
            if (scale > scaleStep)
            {
                scale -= scaleStep;                
            }
        }

        

        private void keyUp(object sender, KeyEventArgs e)
        {
            cntrEnabled = false;
            if(e.KeyCode== Keys.C)
            {
                copySelectedFigure();
               
            }else if (e.KeyCode == Keys.V)
            {
                pasteBuferedFigure(bufferedFigure.x, bufferedFigure.y);
                UpdateGraphics();
            }
            //if (selectedFigure != null && selectedFigure.figure.getAction() == models.Action.SIZE_NWSE)
            //{
           //     selectedFigure.figure.setAction(models.Action.SIZE_NWSE_WR);
            //}
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                //if(selectedFigure!=null&&selectedFigure.figure.getAction()== models.Action.SIZE_NWSE_WR)
                //{
                //    selectedFigure.figure.setAction(models.Action.SIZE_NWSE);
               // }
                cntrEnabled = true;
            }
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            if (selectedFigure != null)
            {
                selectedFigure.figure.updateResolution();
            }
            isMouseDown = false;
            UpdateGraphics();
        }

        private void mouseMove(object sender, MouseEventArgs e)
        { 
            if (selectedFigure != null)
            {
                canvas.ContextMenuStrip = mnuFigure;
                //Figure fig = selectedFigure.figure;
                if (isMouseDown)
                {
                    Point coordinate = transformCoordinate(e.X, e.Y);
                    switch (selectedFigure.figure.getAction())
                    {
                        case models.Action.SHIFT:
                            selectedFigure.x = coordinate.X - clickPoint.X;
                            selectedFigure.y = coordinate.Y - clickPoint.Y;
                            if (selectedFigure.figure is Facade)
                            {
                                selectedFigure.x -= selectedFigure.x % SHIFT_STEP;
                                selectedFigure.y -= selectedFigure.y % SHIFT_STEP;
                            }
                            break;
                        default:
                                selectedFigure.figure.Scale(coordinate.X - selectedFigure.x,
                                    coordinate.Y - selectedFigure.y);
                            break;
                    }
                    UpdateGraphics();
                }                
                else
                {
                    Point point = transformCoordinate(e.X, e.Y);
                    point.X -= selectedFigure.x;
                    point.Y -= selectedFigure.y;
                    Cursor.Current = selectedFigure.figure.getCursor(point.X, point.Y,cntrEnabled);
                }
            }else if (pastingImage != null)
            {
                Cursor.Current = Cursors.Cross;
            }
            else 
            {
                canvas.ContextMenuStrip = mnuCanvas;
                if (isMouseDown)
                {
                    Point coordinate = transformCoordinate((int)(e.X + offsetX * scale), (int)(e.Y + offsetY * scale));
                    offsetX = coordinate.X - clickPoint.X;
                    offsetY = coordinate.Y - clickPoint.Y;
                    UpdateGraphics();
                }
            }
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                
            }
            if (e.Button != MouseButtons.Left) return;
            clickPoint = transformCoordinate(e.X, e.Y);
            if (pastingImage != null)
            {
                addFigure(new BkgImage(pastingImage), clickPoint.X, clickPoint.Y);
                pastingImage = null;
            }
            
            isMouseDown = true;
            if (selectedFigure != null && selectedFigure.figure.isPointInFigure(clickPoint.X - selectedFigure.x, clickPoint.Y - selectedFigure.y))
            {
                clickPoint.X -= selectedFigure.x;
                clickPoint.Y -= selectedFigure.y;
                return;
            }

            int x, y;
            foreach (FigureOnBoard item in bkgImages.getReverseCollection())
            {
                x = clickPoint.X - item.x;
                y = clickPoint.Y - item.y;
                if (item.figure.isPointInFigure(x, y))
                {
                    selectFigure(item);
                    Cursor.Current = item.figure.getCursor(x, y, cntrEnabled);
                    clickPoint.X -= selectedFigure.x;
                    clickPoint.Y -= selectedFigure.y;
                    return;
                }
            }
            
            unselectFigure();
            
        }

        private void unselectFigure()
        {
            if (selectedFigure != null) selectedFigure.figure.unselectFigure();
            selectedFigure = null;

        }

        private void selectFigure(FigureOnBoard item)
        {
            if (selectedFigure != null)
            {
                selectedFigure.figure.unselectFigure();
            }
            selectedFigure = item;
            selectedFigure.figure.selectFigure();
        }

        private Point transformCoordinate(int x, int y)
        {
            return new Point((int)(((x - offsetX )/ scale) ), (int)(((y- offsetY) / scale)));
        }
        private void UpdateGraphics()
        {
            canvas.Refresh();
        }
    }
}
