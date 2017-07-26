using FacadeCreatorApi.Forms;
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
        private const float MAX_ZOOM = 3;
        private const int SHIFT_STEP = 5;
        private const int ARRAY_SIZE_STEP = 10;
        private const float SCALE_STEP = 0.1f;

        KdSdkApi kdApi;
        Control canvas;
        ContextMenuStrip mnuFigure;
        ContextMenuStrip mnuCanvas;

        FiguresCollection bkgImages;
        FiguresCollection facades;
        FigureOnBoard selectedFigure;
        FigureOnBoard bufferedFigure;

        private Point clickPoint = new Point();
        private Point mousePosition = new Point();
        private Bitmap pastingImage;
        private Point absoluteClickPoint;
        int offsetX, offsetY;
        float scale = 1;

        bool cntrEnabled = false;
        bool isMouseDown = false;
        bool canMovedFacadeMode = false;
        public Scenes(Control canvas, KdSdkApi kdApi)
        {
            this.canvas = canvas;
            this.kdApi = kdApi;
            bkgImages = new FiguresCollectionImpl();
            facades = new FiguresCollectionImpl();

            //Image newImage = Image.FromFile("C:\\Users\\gerasymiuk\\Documents\\Visual Studio 2017\\Projects\\FacadeCreator\\FacadeCreatorApi\\bin\\Debug\\1.png");
            //addFigure(new BkgImage(newImage), 0, 0);
            //addFigure(new Facade(1,100, 150), 140, 5);
            //addFigure(new Facade(2,100,150),140,5);
            //addFigure(new Facade(3,100, 150), 340, 5);      


            mnuFigure = createMenuFigure();
            mnuCanvas = createMenuCanvas();

            canvas.MouseDown += new MouseEventHandler(mouseDown);
            canvas.MouseMove += new MouseEventHandler(mouseMove);
            canvas.MouseUp += new MouseEventHandler(mouseUp);
            canvas.MouseWheel += startScale;
            canvas.KeyDown += new KeyEventHandler(keyDown);
            canvas.KeyUp += new KeyEventHandler(keyUp);
            canvas.Paint += new PaintEventHandler(paint);
            canvas.DoubleClick += new EventHandler(canvas_double_click);

            //FiguresCollection col = new FiguresCollectionImpl();
            //col.add(new FigureOnBoard(new Facade(1,10, 20), 12, 21));
            //col.add(new FigureOnBoard(new Facade(2,10, 30), 12, 22));
            //col.add(new FigureOnBoard(new Facade(3,10, 40), 12, 23));
            //foreach (FigureOnBoard item in col.getReverseCollection())
            //{
            //    MessageBox.Show(item.y.ToString());
            //}
        }


        #region createMethods
        private ContextMenuStrip createMenuCanvas()
        {
            LinkedList<Services.MenuItem> menuItems = new LinkedList<Services.MenuItem>();
            menuItems.AddLast(new MenuItemImpl("mnuAddmage", "Добавить изображение", null, mnuAddImage_Click));
            menuItems.AddLast(new MenuItemImpl("mnuClearImages", "Очистить сцену", null, mnuClearImages_Click));
            menuItems.AddLast(new MenuItemImpl("mnuEnableEditPosition", "Редактировать положение фасадов", null, mnuEnableEditPosition_Click));
            menuItems.AddLast(new MenuItemImpl("mnuPaste", "Вставить", null, mnuPaste_Click));

            menuItems.AddLast(new MenuItemImpl("mnuFillFacades", "Обрезать", null, mnuFillFacades_Click));
            menuItems.AddLast(new MenuItemImpl("mnuCreateHowPhotoFacade", "Создать как фото фасады", null, mnuCreateHowPhotoFacade_Click));
            menuItems.AddLast(new MenuItemImpl("mnuCreateHowSandblast", "Создать как пескоструйные фасады", null, mnuCreateHowSandblast_Click));

            ContextMenuBuilder menuBuilder = new ContextMenuBuilder(menuItems);
            return menuBuilder.getContext();
        }
        private ContextMenuStrip createMenuFigure()
        {
            LinkedList<Services.MenuItem> menuItems = new LinkedList<Services.MenuItem>();
           

            menuItems.AddLast(new MenuItemImpl("mnuDelete", "Удалить", null, mhuDeleteFigure_Click));
            menuItems.AddLast(new MenuItemImpl("mnuPosition", "Положение", null, null));
            menuItems.AddLast(new MenuItemImpl("mnuEditImage", "Эффекты", null, null));
            menuItems.AddLast(new MenuItemImpl("mnuCopy", "Копировать", null, mnuCopy_Click));
            menuItems.AddLast(new MenuItemImpl("mnuGetProperty", "Свойства", null, canvas_double_click));

            ContextMenuBuilder menuBuilder = new ContextMenuBuilder(menuItems);
            menuBuilder.addToExistingStrip("mnuPosition", new MenuItemImpl("mnuLevelUp", "На уровень вышe", null, mnuUp_Click));
            menuBuilder.addToExistingStrip("mnuPosition", new MenuItemImpl("mnuLevelUp", "На уровень ниже", null, mnuLower_Click));
            menuBuilder.addToExistingStrip("mnuPosition", new MenuItemImpl("mnuLevelUp", "На передний план", null, mnuToFrontClick));
            menuBuilder.addToExistingStrip("mnuPosition", new MenuItemImpl("mnuLevelUp", "На задний план", null, mnuToBack_Click));

            menuBuilder.addToExistingStrip("mnuEditImage", new MenuItemImpl("mnuInverse", "Инвертировать", null, mnuInversion_Click));
            menuBuilder.addToExistingStrip("mnuEditImage", new MenuItemImpl("mnuMirror", "Отзеркалить", null, null));
            menuBuilder.addToExistingStrip("mnuMirror", new MenuItemImpl("mnuMirrorHorizontal", "По горизонтали", null, mnuMirrorHorizontal_Click));
            menuBuilder.addToExistingStrip("mnuMirror", new MenuItemImpl("mnuMirrorVertical", "По вертикали", null, mnuMirrorVertical_Click));

            return menuBuilder.getContext();
        }

        private void mnuMirrorVertical_Click(object sender, EventArgs e)
        {
            if(selectedFigure!=null&&selectedFigure.figure is BkgImage)
            {
                ((BkgImage)selectedFigure.figure).mirrorVertical();
            }
        }

        private void mnuMirrorHorizontal_Click(object sender, EventArgs e)
        {
            if (selectedFigure != null && selectedFigure.figure is BkgImage)
            {
                ((BkgImage)selectedFigure.figure).mirrorHorizontal();
            }
        }

        #endregion

        #region Event Listener methods
        private void mnuInversion_Click(object sender, EventArgs e)
        {
            if (selectedFigure != null && selectedFigure.figure is BkgImage)
            {
                BkgImage image = (BkgImage)selectedFigure.figure;
                image.inverse();
                UpdateGraphics();
            }
        }
        private void canvas_double_click(object sender, EventArgs e)
        {
            if (selectedFigure != null && !(selectedFigure.figure is Facade))
            {
                PropertyForm frm = new PropertyForm(selectedFigure.figure);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    selectedFigure.figure.width = frm.width;
                    selectedFigure.figure.height = frm.height;
                    UpdateGraphics();
                }
            }
        }
        private void mnuPaste_Click(object sender, EventArgs e)
        {
            pasteBuferedFigure(bufferedFigure.x, bufferedFigure.y);
        }
        private void mhuDeleteFigure_Click(object sender, EventArgs e)
        {
            if (selectedFigure != null)
            {
                deleteFigure(selectedFigure);
            }
        }

        private void mnuClearImages_Click(object sender, EventArgs e)
        {
            bkgImages.removeAll();
            UpdateGraphics();
        }
        private void paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            paintInGraphics(e.Graphics, scale, offsetX, offsetY);
        }
        private void startScale(object sender, MouseEventArgs e)
        {
            if (cntrEnabled && e.Delta != 0)
            {
                if (e.Delta > 0) ZoomIn();
                else ZoomOut();
                Figure.setDelta(scale);
                UpdateGraphics();
            }

        }
        private void mnuToFrontClick(object sender, EventArgs e)
        {
            int index = getIndex(selectedFigure);
            if (index != -1)
            {
                move(index, bkgImages.getCount() - 1);
            }
        }
        private void keyUp(object sender, KeyEventArgs e)
        {
            cntrEnabled = false;

            if (e.KeyCode == Keys.C)
            {
                copySelectedFigure();
            }
            else if (e.KeyCode == Keys.V)
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
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (selectedFigure != null)
                    {
                        selectedFigure.x--;
                    }
                    else
                    {
                        offsetX--;
                    }
                    UpdateGraphics();
                    break;
                case Keys.Right:
                    if (selectedFigure != null)
                    {
                        selectedFigure.x++;
                    }
                    else
                    {
                        offsetX++;
                    }
                    UpdateGraphics();
                    break;
                case Keys.Up:
                    if (selectedFigure != null)
                    {
                        selectedFigure.y--;
                    }
                    else
                    {
                        offsetY--;
                    }
                    UpdateGraphics();
                    break;
                case Keys.Down:
                    if (selectedFigure != null)
                    {
                        selectedFigure.y++;
                    }
                    else
                    {
                        offsetY++;
                    }
                    UpdateGraphics();
                    break;
                case Keys.Delete: if (selectedFigure != null)
                    {
                        deleteFigure(selectedFigure);
                    }
                    break;
            }
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
                    Cursor.Current = selectedFigure.figure.getCursor(point.X, point.Y, cntrEnabled);
                }
            }
            else if (pastingImage != null)
            {
                Cursor.Current = Cursors.Cross;
            }
            else
            {
                canvas.ContextMenuStrip = mnuCanvas;
                if (isMouseDown)
                {
                    Point coordinate = transformCoordinate(e.X, e.Y);

                    //Console.WriteLine("e.x=" + e.X + " offsetX=" + offsetX);
                    //Console.WriteLine("e.y=" + e.Y + " offsetY=" + offsetY);
                    //Console.WriteLine("scale=" + scale + " transformPoint=" + coordinate);
                    //Console.WriteLine("clickpointX=" + clickPoint.X + " clickpointY=" + clickPoint.Y);

                    offsetX += (int)((coordinate.X - clickPoint.X) * scale);
                    offsetY += (int)((coordinate.Y - clickPoint.Y) * scale);
                    clickPoint = transformCoordinate(e.X, e.Y);
                    //Console.WriteLine("offsetX After refreshing="+offsetX+ " offsetY After refreshing=" + offsetY);

                    UpdateGraphics();
                }
            }
            
                mousePosition = new Point(e.X, e.Y);
            
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

            }
            if (e.Button != MouseButtons.Left) return;
            clickPoint = transformCoordinate(e.X, e.Y);
            absoluteClickPoint = new Point(e.X, e.Y);
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
            if (canMovedFacadeMode)
            {
                foreach (FigureOnBoard item in facades.getReverseCollection())
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
            }
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
        private void mnuFillFacades_Click(object sender, EventArgs e)
        {
            Rectangle areaSize = new Rectangle();
            try
            {
                areaSize = getBordersOfCanvas();
                Bitmap image = generateFullGrapics(areaSize);
                bkgImages.removeAll();

                foreach (FigureOnBoard item in facades)
                {
                    Bitmap img = new Bitmap(item.figure.width,item.figure.height);
                    using (Graphics gp = Graphics.FromImage(img))
                    {
                        gp.DrawImage(image, new Rectangle(0, 0, img.Width, img.Height), new Rectangle(item.x - areaSize.X, item.y - areaSize.Y, img.Width, img.Height), GraphicsUnit.Pixel);
                        bkgImages.add(new FigureOnBoard(new BkgImage(img), item.x,item.y));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(areaSize.ToString());
            }
            UpdateGraphics();
        }
        private void mnuCreateHowSandblast_Click(object sender, EventArgs e)
        {
            Rectangle areaSize = new Rectangle();
            try
            {
                areaSize = getBordersOfCanvas();
                Bitmap image = generateFullGrapics(areaSize);
                IDictionary<Facade, string> facades = ImageConversion.generateFacades(areaSize, image, this.facades, kdApi.getScenesName());
                kdApi.applyFacadeImageHowSandblast(facades);
                closePlugin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(areaSize.ToString());
            }
        }
        private void mnuAddImage_Click(object sender, EventArgs e)
        {
            Bitmap newImage = DialogsService.getImageFromDisk("c:\\");
            if (newImage != null)
            {
                pastingImage = newImage;
            }
        }
        private void mnuCreateHowPhotoFacade_Click(object sender, EventArgs e)
        {
            Rectangle areaSize = new Rectangle() ;
            try
            {
                areaSize = getBordersOfCanvas();
                Bitmap image = generateFullGrapics(areaSize);
                IDictionary<Facade, string> facades = ImageConversion.generateFacades(areaSize, image, this.facades, kdApi.getScenesName());
                kdApi.applyFacadeImage(facades);
                closePlugin();
            }catch(Exception ex)
            {
                MessageBox.Show(areaSize.ToString());
            }            
        }

        private void closePlugin()
        {
            if(canvas is Form)
            {
                Form frm = (Form)canvas;
                frm.Close();
            }
            //Application.Exit();
            //Environment.Exit(0);
        }

        private void mnuEnableEditPosition_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = ((ToolStripMenuItem)sender);
            if (menu.CheckState == CheckState.Unchecked)
            {
                canMovedFacadeMode = true;
                menu.CheckState = CheckState.Checked;
            }
            else
            {
                canMovedFacadeMode = false;
                menu.CheckState = CheckState.Unchecked;
            }
        }

        private void mnuLower_Click(object sender, EventArgs e)
        {
            int index = getIndex(selectedFigure);
            if (index > 0)
            {
                shufle(index, index - 1);
            }
        }
        private void mnuUp_Click(object sender, EventArgs e)
        {
            int index = getIndex(selectedFigure);
            if (index != -1 && index != bkgImages.getCount() - 1)
            {
                move(index, index + 1);
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
        private void mnuCopy_Click(object sender, EventArgs e)
        {
            copySelectedFigure();
        }
        #endregion

        #region work with graphics
        private Bitmap generateFullGrapics(Rectangle areaSize)
        {
            if (areaSize.Width == 0 || areaSize.Height == 0)
            {
                throw new ArgumentException("One or more arguments are zero");
            }
            Bitmap newImage = new Bitmap(areaSize.Width, areaSize.Height);
            Graphics graphics = Graphics.FromImage(newImage);
            //graphics.TranslateTransform(areaSize.X, areaSize.Y);
            graphics.FillRectangle(Brushes.White, 0, 0, areaSize.Width, areaSize.Height);
            foreach (FigureOnBoard item in bkgImages)
            {
                item.figure.draw(graphics, item.x-areaSize.X, item.y-areaSize.Y);
            }
            //newImage.Save("c:\\images\\fullImage.jpg");
            return newImage;
        }
        private void paintInGraphics(Graphics graphics, float scale, int offsetX, int offsetY)
        {
            graphics.TranslateTransform(offsetX, offsetY);
            graphics.ScaleTransform(scale, scale);

            foreach (FigureOnBoard item in bkgImages)
            {
                item.figure.draw(graphics, item.x, item.y);
            }
            if (selectedFigure != null)
            {
                selectedFigure.figure.draw(graphics, selectedFigure.x, selectedFigure.y);
            }
            foreach (FigureOnBoard item in facades)
            {
                item.figure.draw(graphics, item.x, item.y);
            }
        }
        private void UpdateGraphics()
        {
            canvas.Refresh();
        }
        #endregion

        #region work with positions
        public void scalingToAllFigureisVisibleMode()
        {
            scale = 0.5f;
        }
        private Rectangle getBordersOfCanvas()
        {
            int minX=int.MaxValue,minY=int.MaxValue, maxX=0, maxY=0, curX, curY;
            foreach (FigureOnBoard item in bkgImages)
            {
                curX = item.x + item.figure.width;
                curY = item.y + item.figure.height;
                if (curX > maxX) maxX = curX;
                if (curY > maxY) maxY = curY;
                if (item.x < minX) minX = item.x;
                if (item.y < minY) minY = item.y;
            }
            foreach (FigureOnBoard item in facades)
            {
                curX = item.x + item.figure.width;
                curY = item.y + item.figure.height;
                if (curX > maxX) maxX = curX;
                if (curY > maxY) maxY = curY;
                if (item.x < minX) minX = item.x;
                if (item.y < minY) minY = item.y;
            }
            maxX -= minX;
            maxY -= minY;

            return new Rectangle(minX,minY,maxX, maxY);
        }
        private void shufle(int index, int v)
        {
            bkgImages.shuffle(index, v);
            UpdateGraphics();
        }
        private void move(int from, int to)
        {
            bkgImages.move(from, to);
            UpdateGraphics();
        }
        private void ZoomIn()
        {
            if (scale <= MAX_ZOOM)
            {
                scale += SCALE_STEP;
                
                //offsetX -= (int)(((mousePosition.X + offsetX) * scaleStep) );
                //offsetY -= (int)(((mousePosition.Y + offsetY) * scaleStep) );
                float x = mousePosition.X * SCALE_STEP;
                x /= scale;
                
                offsetX -= (int)(((mousePosition.X * SCALE_STEP) ) + 0.5);
                offsetY -= (int)(((mousePosition.Y * SCALE_STEP) ) + 0.5);
                Console.WriteLine("MouseX=" + mousePosition.X + " mouseY=" + mousePosition.Y);
                Console.WriteLine("resultX=" + (int)(((mousePosition.X * SCALE_STEP) / scale) + 0.5));
                Console.WriteLine("resultY=" + (int)(((mousePosition.Y * SCALE_STEP) / scale) + 0.5));
                Console.WriteLine("scale=" + scale);

            }
        }
        private void ZoomOut()
        {
            if (scale > SCALE_STEP)
            {
                // offsetX += (int)(mousePosition.X * scaleStep);
                //offsetY += (int)(mousePosition.Y * scaleStep);
                scale -= SCALE_STEP;
                offsetX += (int)(((mousePosition.X * SCALE_STEP) )+0.5);
                offsetY += (int)(((mousePosition.Y * SCALE_STEP) ) + 0.5);
                // offsetX += (int)(((mousePosition.X + offsetX) * scaleStep) );
                //offsetY += (int)(((mousePosition.Y + offsetY) * scaleStep) );

            }
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
            return new Point((int)(((x - offsetX) / scale)), (int)(((y - offsetY) / scale)));
        }
       
        #endregion

        #region work with collection of figures
        private void copySelectedFigure()
        {
            if (selectedFigure != null) bufferedFigure = selectedFigure;
        }
        private void pasteBuferedFigure(int x, int y)
        {
            addFigure(bufferedFigure.figure.copy(), x + 20, y + 20);
        }
        public void addFigure(Figure figure, int x, int y)
        {
            if (figure is BkgImage)
                bkgImages.add(new FigureOnBoard(figure, x, y));
            else
                facades.add(new FigureOnBoard(figure, x, y));
            UpdateGraphics();
        }        
        private void deleteFigure(FigureOnBoard selectedFigure)
        {
            if (selectedFigure.figure is BkgImage)
            {
                bkgImages.remove(selectedFigure);
            }
            else
            {
                facades.remove(selectedFigure);
            }
            unselectFigure();
            UpdateGraphics();
        }
        private int getIndex(FigureOnBoard selectedFigure)
        {
            return bkgImages.getIndex(selectedFigure);
        }
        #endregion    
        
        
    }
}
