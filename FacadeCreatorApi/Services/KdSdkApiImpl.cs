using KD.SDK;
using System;
using FacadeCreatorApi.models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FacadeCreatorApi.Services
{
    public class KdSdkApiImpl : KdSdkApi
    {
        enum TextureType
        {
            Sandblast,
            Photo
        }
        Appli _appli;
        Scene _scene;
        int iParamsBlock;
        public KdSdkApiImpl(int iParamsBlock)
        {
            this.iParamsBlock = iParamsBlock;
            _appli = new Appli();
            _scene = _appli.Scene;
        }


        public ICollection<FigureOnBoard> getFacades()
        {
            LinkedList<FigureOnBoard> facades = new LinkedList<FigureOnBoard>();
            try
            {
                _appli.StartSessionFromCallParams(iParamsBlock);
            }catch(Exception e)
            {
                throw new MethodAccessException("Cannot start session: " + e.StackTrace);
            }
            int n = _scene.SelectionGetObjectsNb();
            if (n < 1)
            {
                throw new ArgumentNullException("Please selected object in scene");
            }
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
                if (!searchLabel(objectId)) continue;
                //MessageBox.Show(objectId.ToString());
                //MessageBox.Show("x=" + x + " y = " + y + " width=" + width + "height=" + height);
                string textureNum = _scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.TEXTURE);
                //MessageBox.Show(textureNum);
                int textureNumber = -1;
                if (!textureNum.Equals(""))
                {
                    try
                    {
                        textureNum = textureNum.Split(";,".ToCharArray())[1];
                    }
                    catch (Exception)
                    {
                        textureNumber = -1;
                    }
                    if (!Int32.TryParse(textureNum, out textureNumber))
                    {
                        textureNumber = -1;
                    }
                }
                //MessageBox.Show(textureNumber.ToString());
                if (float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.POSX), out x) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.POSY), out y) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.POSZ), out z) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.DIMX), out width) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.DIMZ), out height) &&
                    int.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.ON_OR_UNDER), out onOrUnder) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.ANGLEXY), out angle) &&
                    float.TryParse(_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.ANGLEXY), out angle))
                {
                    FigureOnBoard fig = createFacadeOnBoardFromKdScenes(objectId, textureNumber, x, y, z, width, height, angle, xScenes, yScenes, heightScenes, onOrUnder, getSceneDimension());
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

        private int getSceneDimension()
        {
            int dimension = 0;
            if(!Int32.TryParse(_appli.Scene.SceneGetInfo(SceneEnum.SceneInfo.UNITDIVISIONSNB),out dimension)){
                return 10;
            }
            return dimension;
        }

        private FigureOnBoard createFacadeOnBoardFromKdScenes(int facadeId,int textureNumber, float x,float y, float z, float width, float height, float angle, float xScenes, float yScenes, float heightScenes, int onOrUnder, int dimensionSizeScenes)
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
            //MessageBox.Show("x=" + figureX + " y=" + figureY);
            return new FigureOnBoard(new Facade(facadeId, textureNumber, (int)width, (int)height), (int)figureX, (int)figureY);

        }

        private bool searchLabel(int objectId)
        {
            //int n = _scene.ObjectGetChildrenNb(objectId, true);
            //int componentId = 0;
            //string labelString = StringResources.getObjectLabelText();
            if (_scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.TEXTURE).Equals("")) return false;
            //for (int i = 0; i < n; i++)
            //{
               
            //    componentId = _scene.ObjectGetChildId(objectId, i, true);
            //    //MessageBox.Show("componentID=" + componentId + " " + _scene.ObjectGetInfo(componentId, SceneEnum.ObjectInfo.BLOCKCODE));
            //    if (_scene.ObjectGetInfo(componentId, SceneEnum.ObjectInfo.BLOCKCODE).Equals(labelString)) return true;
            //}
            return true;
        }
        public void applyFacadeImage(IDictionary<Facade, string> facades)
        {
            KD.SDK.Catalog catalog = _appli.Catalog;
            
            catalog.SessionId = _appli.StartSessionFromCallParams(iParamsBlock);
            if (catalog.FileLoad(StringResources.getCatalogsPath() + "\\@tx_pal.cat", "")){
                
                String project = _scene.SceneGetInfo(SceneEnum.SceneInfo.NAME);
                //MessageBox.Show("isLoad=" + catalog.IsLoaded() + " " + catalog.GetInfo(CatalogEnum.InfoId.FILENAME));
                int n = catalog.TableGetLinesNb(CatalogEnum.TableId.TEXTURES, 0);
                int textureIndex = 0;
                int TextureKey;

                Random rand = new Random();
                //MessageBox.Show(facades.Count.ToString());
                string strWidth;
                int oldWidth, newWidth;
                foreach (KeyValuePair<Facade,string> item in facades)
                {
                    //MessageBox.Show(item.Key.getTextureId().ToString());
                    if (item.Key.getTextureId() != -1)
                    {
                        textureIndex = catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES,0,0,item.Key.getTextureId().ToString(),false);
                        TextureKey = item.Key.getTextureId();
                        //MessageBox.Show("texture index = " + textureIndex);
                    }
                    else
                    {
                        catalog.TableAddLines(CatalogEnum.TableId.TEXTURES, 0, 1);
                        textureIndex = n++;
                        TextureKey = textureIndex + 1;
                        while (catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES, 0, 0, TextureKey.ToString(), false) !=-1)
                        {
                            //MessageBox.Show("key: " + TextureKey + " alreadyExist\n"+ catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES, 0, 0, TextureKey.ToString(), false));
                            TextureKey++;
                        }
                        item.Key.setTextureId(TextureKey);
                    }
                    strWidth = catalog.TableGetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 4);
                    Int32.TryParse(strWidth, out oldWidth);
                    if (oldWidth != item.Key.width) newWidth = item.Key.width;
                    else newWidth = oldWidth + 1;
                    //MessageBox.Show("edit "+ textureIndex + " line, with "+ TextureKey + " kod");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 0, TextureKey.ToString());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 1, project+item.Key.getNumber() + rand.Next(1,10));
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 3, item.Value);
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 4, newWidth.ToString());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 5, item.Key.height.ToString());
                    //catalog.FileSave(StringResources.getCatalogsPath() + "\\@tx_pal.cat");

                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 10, "");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 11, "0");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 12, "0");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 13, "0.2");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 15, "0.2");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 16, "0");

                    //MessageBox.Show(catalog.TableGetLine(CatalogEnum.TableId.TEXTURES, 0, textureIndex));
                    //n++;
                    //nextTextureKey++; 
                }
                if(catalog.FileSave(StringResources.getCatalogsPath() + "\\@tx_pal.cat"))
                {
                    //MessageBox.Show("File Saved");
                }else MessageBox.Show("File dont saved");
                registerTextures(facades.Keys);
                updateGraphics();

                //MessageBox.Show(count.ToString());
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
        private void registerTextures(ICollection<Facade> facades)
        {
            foreach (Facade item in facades)
            {
                string textureString = "128128128," + item.getTextureId() + ";128128128,@1";
                //MessageBox.Show(textureString);
                if(_scene.ObjectSetInfo(item.getNumber(), textureString, SceneEnum.ObjectInfo.TEXTURE))
                {
                   // MessageBox.Show("Saved texture to file");
                }
            }
        }
        private void updateGraphics()
        {
            _scene.ViewRefresh();
        }

        public string getScenesName()
        {
           return _scene.SceneGetInfo(SceneEnum.SceneInfo.NAME);
        }

        public void applyFacadeImageHowSandblast(IDictionary<Facade, string> facades)
        {
            KD.SDK.Catalog catalog = _appli.Catalog;

            catalog.SessionId = _appli.StartSessionFromCallParams(iParamsBlock);
            if (catalog.FileLoad(StringResources.getCatalogsPath() + "\\@tx_pal.cat", ""))
            {

                String project = _scene.SceneGetInfo(SceneEnum.SceneInfo.NAME);
                //MessageBox.Show("isLoad=" + catalog.IsLoaded() + " " + catalog.GetInfo(CatalogEnum.InfoId.FILENAME));
                int n = catalog.TableGetLinesNb(CatalogEnum.TableId.TEXTURES, 0);
               
                int textureIndex = 0;
                int TextureKey;

                Random rand = new Random();
                //MessageBox.Show(facades.Count.ToString());
                string strWidth;
                int oldWidth, newWidth;
                foreach (KeyValuePair<Facade, string> item in facades)
                {
                    //MessageBox.Show(item.Key.getTextureId().ToString());
                    if (item.Key.getTextureId() != -1)
                    {
                        textureIndex = catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES, 0, 0, item.Key.getTextureId().ToString(), false);
                        TextureKey = item.Key.getTextureId();
                       // MessageBox.Show("texture index = " + textureIndex);
                    }
                    else
                    {
                        catalog.TableAddLines(CatalogEnum.TableId.TEXTURES, 0, 1);
                        textureIndex = n++;
                        TextureKey = textureIndex + 1;
                        while (catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES, 0, 0, TextureKey.ToString(), false) != -1)
                        {
                            //MessageBox.Show("key: " + TextureKey + " alreadyExist\n"+ catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES, 0, 0, TextureKey.ToString(), false));
                            TextureKey++;
                        }
                        item.Key.setTextureId(TextureKey);
                    }
                    strWidth = catalog.TableGetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 11);
                    Int32.TryParse(strWidth, out oldWidth);
                    if (oldWidth != item.Key.width) newWidth = item.Key.width;
                    else newWidth = oldWidth + 1;
                    //MessageBox.Show("edit "+ textureIndex + " line, with "+ TextureKey + " kod");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 0, TextureKey.ToString());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 1, project + item.Key.getNumber() + rand.Next(1, 10));
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 3, StringResources.getImageDirectoryName()+"\\gray.jpg");

                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 4, "0");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 5, "0");

                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 10, item.Value);
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 11, newWidth.ToString());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 12, item.Key.height.ToString());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 13, "0");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 15, "1");
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 16, "1");

                    //catalog.FileSave(StringResources.getCatalogsPath() + "\\@tx_pal.cat");
                    //MessageBox.Show(catalog.TableGetLine(CatalogEnum.TableId.TEXTURES, 0, textureIndex));
                    //n++;
                    //nextTextureKey++; 
                }
                if (catalog.FileSave(StringResources.getCatalogsPath() + "\\@tx_pal.cat"))
                {
                    //MessageBox.Show("File Saved");
                }
                else MessageBox.Show("File dont saved");
                registerTextures(facades.Keys);
                updateGraphics();

                //MessageBox.Show(count.ToString());
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
        public string getImagePathFromTexture(int id)
        {
            KD.SDK.Catalog catalog = _appli.Catalog;

            catalog.SessionId = _appli.StartSessionFromCallParams(iParamsBlock);
            if (catalog.FileLoad(StringResources.getCatalogsPath() + "\\@tx_pal.cat", ""))
            {

                String project = _scene.SceneGetInfo(SceneEnum.SceneInfo.NAME);
                //MessageBox.Show("isLoad=" + catalog.IsLoaded() + " " + catalog.GetInfo(CatalogEnum.InfoId.FILENAME));

                int textureIndex = catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES, 0, 0, id.ToString(), false);
                //MessageBox.Show(catalog.TableGetLine(CatalogEnum.TableId.TEXTURES, 0, textureIndex));
                string sandblandPath = catalog.TableGetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 10);
                string photoPath = catalog.TableGetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 3);
                string pathToTextures = StringResources.getResourcesPath();
                //MessageBox.Show(sandblandPath);
                if (!sandblandPath.Equals(""))
                {
                    return pathToTextures + "\\" + sandblandPath;
                }
                else if(!photoPath.Equals(""))
                {
                    return pathToTextures + "\\" + photoPath;
                }
            }
            return "";
        }
    }
}
