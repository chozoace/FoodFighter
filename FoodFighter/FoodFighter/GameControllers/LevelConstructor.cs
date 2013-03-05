using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;


namespace FoodFighter
{
    class LevelConstructor
    {
        List<Wall> walls = new List<Wall>();
        static LevelConstructor instance;
        Vector2 camera = new Vector2(0, 0);

        XmlDocument xDoc;

        string currentLevel;
        int playerXpos;
        int playerYpos;
        int mapWidth;
        int mapHeight;
        int tileWidth;
        int tileHeight;
        int numberOfTiles;
        List<int> gidList = new List<int>();
        List<int> gidList2 = new List<int>();

        public LevelConstructor()
        {
            instance = this;
        }

        public static LevelConstructor Instance()
        {
            return instance;
        }

        public List<Wall> getWallList()
        {
            return walls;
        }

        public void loadLevel(String level)
        {
            currentLevel = level;
            XmlLoad(currentLevel);
        }

        //begin XML here
        public void XmlLoad(String level)
        {
            //map, tileset and layer, data, tile 
            xDoc = new XmlDocument();
            xDoc.Load(level);
            XmlNode mapData = xDoc.FirstChild;
            numberOfTiles = mapData.FirstChild.NextSibling.FirstChild.ChildNodes.Count;
            mapWidth = int.Parse(mapData.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(mapData.Attributes.GetNamedItem("height").Value);
            tileWidth = int.Parse(mapData.Attributes.GetNamedItem("tilewidth").Value);
            tileHeight = int.Parse(mapData.Attributes.GetNamedItem("tileheight").Value);

            //gidlist is single dimensional array
            foreach (XmlNode xNode in mapData.FirstChild.NextSibling.FirstChild.ChildNodes)
            {
                gidList.Add(int.Parse(xNode.Attributes.GetNamedItem("gid").Value));
            }

            tileLoad();
        }

        public void tileLoad()
        {
            Wall theWall;
            Enemy enemy;

            for (int spriteforX = 0; spriteforX < mapWidth; spriteforX++)
            {
                for (int spriteForY = 0; spriteForY < mapHeight; spriteForY++)
                {
                    int destY = spriteForY * tileHeight;
                    int destX = spriteforX * tileWidth;

                    switch (getTileAt(spriteforX, spriteForY))
                    {
                        case 1:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, 1);
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 2:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, 2);
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 3:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, 3);
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 4:
                            playerXpos = destX;
                            playerYpos = destY;
                            break;
                        case 5:
                            enemy = new OnionEnemy(new Vector2(destX, destY));
                            LevelManager.Instance().addToEnemyList(enemy);
                            LevelManager.Instance().addToSpriteList(enemy);
                            break;
                        case 6:
                            enemy = new MeleeEnemy(new Vector2(destX, destY));
                            LevelManager.Instance().addToEnemyList(enemy);
                            LevelManager.Instance().addToSpriteList(enemy);
                            break;
                    }
                }
            }
            LevelManager.Instance().player = new Player(new Vector2(playerXpos, playerYpos));
            //LevelManager.Instance().addToEnemyList(enemy);
            LevelManager.Instance().addToSpriteList(LevelManager.Instance().player);
        }

        public int getTileAt(int x, int y)
        {
            return gidList[(x + (y * mapWidth))];
        }

    }
}
