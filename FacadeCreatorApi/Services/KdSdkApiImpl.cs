﻿
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
                if (!searchLabel(objectId)) continue;
                //MessageBox.Show(objectId.ToString());
                //MessageBox.Show("x=" + x + " y = " + y + " width=" + width + "height=" + height);
                string textureNum = _scene.ObjectGetInfo(objectId, SceneEnum.ObjectInfo.TEXTURE);
                //MessageBox.Show(textureNum);
                int textureNumber = -1;
                if (!textureNum.Equals(""))
                {
                    textureNum = textureNum.Split(";,".ToCharArray())[1];                    
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
                    FigureOnBoard fig = createFacadeOnBoardFromKdScenes(objectId, textureNumber, x, y, z, width, height, angle, xScenes, yScenes, heightScenes, onOrUnder, 10);
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
            return false;
        }
        public void applyFacadeImage(IDictionary<Facade, string> facades)
        {
            KD.SDK.Catalog catalog = _appli.Catalog;
            
            catalog.SessionId = _appli.StartSessionFromCallParams(iParamsBlock);
            if (catalog.FileLoad(StringResources.getCatalogsPath() + "\\@tx_pal.cat", "")){
                String project = _scene.SceneGetInfo(SceneEnum.SceneInfo.NAME);
                //MessageBox.Show("isLoad=" + catalog.IsLoaded() + " " + catalog.GetInfo(CatalogEnum.InfoId.FILENAME));
                int n = catalog.TableGetLinesNb(CatalogEnum.TableId.TEXTURES,0);
                int textureIndex = 0;
                int TextureKey;
                //MessageBox.Show(facades.Count.ToString());
                foreach (KeyValuePair<Facade,string> item in facades)
                {
                    //MessageBox.Show(item.Key.getTextureId().ToString());
                    if (item.Key.getTextureId() != -1)
                    {
                        textureIndex = catalog.TableGetLineRankFromCode(CatalogEnum.TableId.TEXTURES,0,0,item.Key.getTextureId().ToString(),false);
                        TextureKey = item.Key.getTextureId();
                        MessageBox.Show("texture index = " + textureIndex);
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
                    
                    //MessageBox.Show("edit "+ textureIndex + " line, with "+ TextureKey + " kod");

                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 0, TextureKey.ToString());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 1, project+item.Key.getNumber());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 3, item.Value);
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 4, item.Key.width.ToString());
                    catalog.TableSetLineInfo(CatalogEnum.TableId.TEXTURES, 0, textureIndex, 5, item.Key.height.ToString());
                    //catalog.FileSave(StringResources.getCatalogsPath() + "\\@tx_pal.cat");
                    MessageBox.Show(catalog.TableGetLine(CatalogEnum.TableId.TEXTURES, 0, textureIndex));
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
                    MessageBox.Show("Saved texture to file");
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
    }
}