using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
                    alternativeName = StringResources.getResourcePathWithoutAbsolute() + "\\" + scenesName + item.figure.ToString() + ".jpg";
                    pathToImage = StringResources.getResourcesPath()+ "\\"+ scenesName + item.figure.ToString() + ".jpg";
                        MessageBox.Show(pathToImage);
                    try
                    {
                        gp.DrawImage(image, new Rectangle(0, 0, img.Width, img.Height), new Rectangle(item.x - areaSize.X, item.y - areaSize.Y, img.Width, img.Height), GraphicsUnit.Pixel);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Cannot Draw Image!!" + e.StackTrace);
                    }
                    try
                    {
                        img.Save(pathToImage, ImageFormat.Jpeg);
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show("Cannot Save Image!!" + e.StackTrace);
                    }
                        facades.Add((Facade)item.figure, alternativeName);
                    }
                    //img = new Bitmap(image.Clone(new Rectangle(item.x, item.y, item.figure.width, item.figure.height), image.PixelFormat           
            }
            return facades;
        }
    }
}
