using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi.Services
{
    public class ImageConversion
    {
        public static IDictionary<Facade,string> generateFacades(Rectangle areaSize, Bitmap image,FiguresCollection collection,string scenesName)
        {
            IDictionary<Facade, string> facades = new Dictionary<Facade, string>();
            Image img;
            string pathToImage = "";
            string alternativeName = "";
            foreach (FigureOnBoard item in collection)
            { 
                    img = new Bitmap(item.figure.width, item.figure.height);
                    using(Graphics gp = Graphics.FromImage(img))
                    {
                        alternativeName = StringResources.getImageDirectoryName() + "\\" + scenesName + item.figure.ToString() + ".jpg";
                        pathToImage = StringResources.getResourcesPath()+"\\"+ StringResources.getImageDirectoryName() + "\\"+ scenesName + item.figure.ToString() + ".jpg";
                            //MessageBox.Show(pathToImage);
                        try
                        {
                            gp.DrawImage(image, new Rectangle(0, 0, img.Width, img.Height), new Rectangle(item.x - areaSize.X, item.y - areaSize.Y, img.Width, img.Height), GraphicsUnit.Pixel);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Cannot Draw Image!!\n" + e.StackTrace);
                        }
                        try
                        {
                        //MessageBox.Show(pathToImage);
                            img.Save(pathToImage, ImageFormat.Jpeg);
                        }
                        catch (Exception e)
                        {
                            throw new System.IO.FileNotFoundException("Cannot Save Image!!"+img+"\n"+e.StackTrace);
                        }
                        facades.Add((Facade)item.figure, alternativeName);
                    }
                    //img = new Bitmap(image.Clone(new Rectangle(item.x, item.y, item.figure.width, item.figure.height), image.PixelFormat           
            }
            return facades;
        }

        public static void createBackgroundImage(string path)
        {
            Image image = new Bitmap(100, 100);
            Graphics graph = Graphics.FromImage(image);
            graph.FillRectangle(Brushes.LightGray, 0, 0, 100, 100);

            //graph.Clear(Color.FromArgb(0, 178, 178, 178));
            //graph.Save();
            image.Save(path);
        }
        public static Bitmap inverseBlackWhiteImage(Bitmap oldImage)
        {
            Bitmap newImage = new Bitmap(oldImage.Width, oldImage.Height);
            Graphics newGraphics = Graphics.FromImage(newImage);
            int width = oldImage.Width;
            int height = oldImage.Height;
            int R, G, B;
            newGraphics.FillRectangle(Brushes.Black, 0, 0, width, height);
            Color color;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    color = oldImage.GetPixel(i, j);
                    if (color.R < 128 && color.G < 128 && color.B < 128)
                    {
                        newImage.SetPixel(i, j, Color.White);
                    }
                }
            return newImage;
        }
        public static Bitmap getImage(String path)
        {
            if (!File.Exists(path)) return null;
            using(Bitmap imageFromDisk = new Bitmap(path)){
                try
                {
                    Bitmap imageInRam = new Bitmap(imageFromDisk.Width, imageFromDisk.Height);
                    Graphics imageGraphics = Graphics.FromImage(imageInRam);
                    imageGraphics.DrawImage(imageFromDisk, 0, 0);
                    
                    return imageInRam;
                } catch (Exception){ }
                
            }
            return null;
        }
    }
}
