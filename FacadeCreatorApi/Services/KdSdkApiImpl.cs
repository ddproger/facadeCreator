
using KD.SDK;
using System;
using System.Runtime.InteropServices;
using FacadeCreatorApi.models;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FacadeCreatorApi.Services
{


    public class KdSdkApiImpl : KdSdkApi
    {
        Appli _appli;
        Scene _scene;
        int iParamsBlock;
        public KdSdkApiImpl(int iParamsBlock)
        {
            this.iParamsBlock = iParamsBlock;
            this._appli = new Appli();
            _scene = _appli.Scene;
        }

        public void applyFacadeImage(Image img)
        {
            throw new NotImplementedException();
        }

        public ICollection<FigureOnBoard> getFacades()
        {
            LinkedList<FigureOnBoard> facades = new LinkedList<FigureOnBoard>();
            _appli.StartSessionFromCallParams(iParamsBlock);
            int n = _scene.SelectionGetObjectsNb();
            int objectId = 0;

            float x = 0, y = 0, z=0, width = 0, height = 0, angle=0, heightScenes,xScenes,yScenes, minX=float.MaxValue,minY= float.MaxValue;
            int onOrUnder;
            if (!float.TryParse(_scene.SceneGetInfo(SceneEnum.SceneInfo.DIMX), out xScenes)||
                !float.TryParse(_scene.SceneGetInfo(SceneEnum.SceneInfo.DIMY), out yScenes) ||
                !float.TryParse(_scene.SceneGetInfo(SceneEnum.SceneInfo.DIMZ), out heightScenes))
                throw new Exception("cannot get dimension of Scenes");
            //MessageBox.Show("CountOfElement=" + n);
            for (int i = 0; i < n; i++)
            {
                objectId = _scene.SelectionGetObjectId(i);
                //if (!searchLabel(objectId)) break;
                //MessageBox.Show(objectId.ToString());
                //MessageBox.Show("x=" + x + " y = " + y + " width=" + width + "height=" + height);
                if (float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.POSX), out x) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.POSY), out y) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.POSZ), out z) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.DIMX), out width) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.DIMZ), out height) &&
                    int.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.ON_OR_UNDER), out onOrUnder) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.ANGLEXY), out angle) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.ANGLEXY), out angle))
                {
                    FigureOnBoard fig = createFacadeOnBoardFromKdScenes(objectId, x, y, z, width, height, angle, xScenes, yScenes, heightScenes, onOrUnder, 10);
                    if (fig.x < minX) minX = fig.x;
                    if (fig.y < minY) minY = fig.y;
                    facades.AddLast(fig);                    
                }
                else { MessageBox.Show("I cant get all values from object"); }
        }
            foreach (FigureOnBoard item in facades)
            {
                item.x -= (int)minX;
                item.y -= (int)minY;
            }
            return facades;
        }

        private FigureOnBoard createFacadeOnBoardFromKdScenes(int facadeId, float x,float y, float z, float width, float height, float angle, float xScenes, float yScenes, float heightScenes, int onOrUnder, int dimensionSizeScenes)
        {
            float figureX = 0,figureY=0;
            if (onOrUnder != 0) figureY = heightScenes/ dimensionSizeScenes - z;
            else figureY = heightScenes/ dimensionSizeScenes - height - z;
            int intAngle = (int)angle;
            intAngle %= 180;
            if (intAngle == 90)
            {
                figureX = y + yScenes / (dimensionSizeScenes * 2) - width;
            }
            else if (intAngle == 0)
            {
                figureX = x + xScenes / (dimensionSizeScenes * 2) - width;
            }
            else throw new Exception("Please correct your angle in facades: " + angle);
            MessageBox.Show("x=" + figureX + " y=" + figureY);
            return new FigureOnBoard(new Facade(facadeId, (int)width, (int)height), (int)figureX, (int)figureY);

        }

        private bool searchLabel(int objectId)
        {
            int n = _scene.ObjectGetChildrenNb(objectId, true);
            int componentId = 0;
            for (int i = 0; i < n; i++)
            {
               
                componentId = _scene.ObjectGetChildId(objectId, i, true);
                //MessageBox.Show("componentID=" + componentId + " " + _scene.ObjectGetInfo(componentId, SceneEnum.ObjectInfo.BLOCKCODE));
                if (_scene.ObjectGetInfo(componentId, SceneEnum.ObjectInfo.BLOCKCODE).Equals("PANPLACE")) return true;
            }
            return false;
        }
        public void updateCatalog(int CallParam)
        {
            KD.SDK.Catalog catalog = _appli.Catalog;
           
            catalog.SessionId = _appli.StartSessionFromCallParams(CallParam);
            if (catalog.FileLoad("C:\\InSitu\\Catalogs\\@tx_pal.cat", "")){
                
                MessageBox.Show("isLoad=" + catalog.IsLoaded() + " " + catalog.GetInfo(CatalogEnum.InfoId.FILENAME));
                int n = catalog.TableGetLinesNb(CatalogEnum.TableId.TEXTURES,0);

                int count = catalog.TableAddLines(CatalogEnum.TableId.TEXTURES, 0, 1);
                MessageBox.Show(count.ToString());
                //catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, n, 0, (n+1).ToString());
                //catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, n, 1, "New Texture");
                //catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, n, 3, "images\\new.jpg");
                //catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, n, 4, "1521");
                //catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, n, 5, "651");
                //catalog.FileSave("C:\\InSitu\\Catalogs\\@tx_pal.cat");
                //n++;


                //for (int i = 0; i < n; i++)
                //{                    
                //    MessageBox.Show(catalog.TableGetLine(CatalogEnum.TableId.TEXTURES, 0, i));
                //}
                
            }
            else
            {
                MessageBox.Show("SomethingWrong");
            }
        }
        
    }
}
