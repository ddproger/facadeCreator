using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public class ImageConversion
    {
        public static void generateFacades(Rectangle areaSize, Bitmap image,FiguresCollection collection)
        {
            
            Image img;
            foreach (FigureOnBoard item in collection)
            { 
                    img = new Bitmap(item.figure.width, item.figure.height);
                    using(Graphics gp = Graphics.FromImage(img))
                    {
                        gp.DrawImage(image, new Rectangle(0, 0, img.Width, img.Height), new Rectangle(item.x-areaSize.X, item.y-areaSize.Y, img.Width, img.Height),GraphicsUnit.Pixel);
                        img.Save("c:\\images\\image" + item.figure.ToString() + ".jpg", ImageFormat.Jpeg);
                    }
                    //img = new Bitmap(image.Clone(new Rectangle(item.x, item.y, item.figure.width, item.figure.height), image.PixelFormat           
            } 
        }
    }
}
