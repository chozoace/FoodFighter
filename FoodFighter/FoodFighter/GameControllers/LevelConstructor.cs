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

        int playerX;
        int playerY;

        string regularLevel1 = "Content/XML/Level1.xml";

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
            if (currentLevel == "Content/XML/TutorialLevel.xml") 
                XmlLoad(currentLevel, false);
            else
                XmlLoad(currentLevel);
        }

        //begin XML here
        //public void XmlLoad(String level, bool loadBackground = true)
        //{
        //    //map, tileset and layer, data, tile 
        //    xDoc = new XmlDocument();
        //    xDoc.Load(level);
        //    XmlNode mapData = xDoc.FirstChild;
        //    numberOfTiles = mapData.FirstChild.NextSibling.FirstChild.ChildNodes.Count;
        //    mapWidth = int.Parse(mapData.Attributes.GetNamedItem("width").Value);
        //    mapHeight = int.Parse(mapData.Attributes.GetNamedItem("height").Value);
        //    tileWidth = int.Parse(mapData.Attributes.GetNamedItem("tilewidth").Value);
        //    tileHeight = int.Parse(mapData.Attributes.GetNamedItem("tileheight").Value);

        //    //gidlist is single dimensional array
        //    foreach (XmlNode xNode in mapData.FirstChild.NextSibling.FirstChild.ChildNodes)
        //    {
        //        gidList.Add(int.Parse(xNode.Attributes.GetNamedItem("gid").Value));
        //    }

        //    tileLoad();
        //}

        public void XmlLoad(String level, bool loadBackground = true)
        {
            //map, tileset and layer, data, tile 
            xDoc = new XmlDocument();
            //sending it the parameter fucks it up
            xDoc.Load(level);
            XmlNode mapData = xDoc.FirstChild;
            if (!loadBackground)
            {
                numberOfTiles = mapData.FirstChild.NextSibling.NextSibling.FirstChild.ChildNodes.Count;
            }
            else
            {
                numberOfTiles = mapData.FirstChild.NextSibling.NextSibling.NextSibling.FirstChild.ChildNodes.Count;
            }
            mapWidth = int.Parse(mapData.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(mapData.Attributes.GetNamedItem("height").Value);
            tileWidth = int.Parse(mapData.Attributes.GetNamedItem("tilewidth").Value);
            tileHeight = int.Parse(mapData.Attributes.GetNamedItem("tileheight").Value);

            //gidlist is single dimensional array
            if (!loadBackground)
            {
                gidList.Clear();
                foreach (XmlNode xNode in mapData.FirstChild.NextSibling.NextSibling.FirstChild.ChildNodes)
                {
                    gidList.Add(int.Parse(xNode.Attributes.GetNamedItem("gid").Value));
                }
                tileLoad();
            }
            else
            {
                gidList.Clear();
                foreach (XmlNode xNode in mapData.FirstChild.NextSibling.NextSibling.NextSibling.FirstChild.ChildNodes)
                {
                    gidList.Add(int.Parse(xNode.Attributes.GetNamedItem("gid").Value));
                }
                tileLoadBackground();
            }

            //tileLoad();
        }

        public void tileLoad()
        {
            Wall theWall;
            Enemy enemy;
            Sprite theObject;

            if (currentLevel == "Content/XML/TutorialLevel.xml")
            {
                theObject = new Sprite("LevelObjects/BackImage1", 0, 0);
                LevelManager.Instance().addToSpriteList(theObject);
                theObject = new Sprite("LevelObjects/BackImage2", 4096, 0);
                LevelManager.Instance().addToSpriteList(theObject);
            }

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
                            theWall = new DeathBlock(new Vector2(destX, destY), tileWidth, tileHeight, 3);
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
                        case 7:
                            enemy = new FryEnemy(new Vector2(destX, destY));
                            LevelManager.Instance().addToEnemyList(enemy);
                            LevelManager.Instance().addToSpriteList(enemy);
                            break;
                        case 8:
                            theWall = new ThinFloor(new Vector2(destX, destY), tileWidth, tileHeight, 9, "LevelObjects/ThinFloor");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 9:
                            theObject = new HealthPickUp(new Vector2(destX, destY));
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 10:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, 0, "LevelObjects/DeathBlock", "NextLevel");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                    }
                }
            }
            if (currentLevel == "Content/XML/TutorialLevel.xml")
            {
                playerX = playerXpos;
                playerY = playerYpos;
            }
            else
            {
                LevelManager.Instance().player = new Player(new Vector2(playerXpos, playerYpos));
                LevelManager.Instance().addToSpriteList(LevelManager.Instance().player);
            }

            if (currentLevel == "Content/XML/TutorialLevel.xml")
            {
                XmlLoad(currentLevel, true);
            }

        }

        public void tileLoadBackground()
        {
            Sprite theSprite;
            for (int spriteforX = 0; spriteforX < mapWidth; spriteforX++)
            {
                for (int spriteForY = 0; spriteForY < mapHeight; spriteForY++)
                {
                    int destY = spriteForY * tileHeight;
                    int destX = spriteforX * tileWidth;

                    switch (getTileAt(spriteforX, spriteForY))
                    {
                        case 1:
                            theSprite = new Sprite("LevelObjects/TutorialLevelLayer", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            theSprite = new Sprite("LevelObjects/TutorialLevelLayer2", 4096, 0);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                        case 11:
                            theSprite = new Sprite("Buildings/build1", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                        case 12:
                            theSprite = new Sprite("Buildings/build2", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                        case 13:
                            theSprite = new Sprite("Buildings/build3", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                        case 14:
                            theSprite = new Sprite("Buildings/build4", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                        case 15:
                            theSprite = new Sprite("Buildings/build5", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                        case 16:
                            theSprite = new Sprite("Buildings/build6", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                        case 17:
                            theSprite = new Sprite("Buildings/build7", destX, destY);
                            LevelManager.Instance().addToSpriteList(theSprite);
                            break;
                    }
                }
            }
            if (currentLevel == "Content/XML/Level1.xml")
                XmlLoad("Content/XML/Level1.xml", false);
            else
            {
                LevelManager.Instance().player = new Player(new Vector2(playerXpos, playerYpos));
                LevelManager.Instance().addToSpriteList(LevelManager.Instance().player);
            }

        }

        public int getTileAt(int x, int y)
        {
            return gidList[(x + (y * mapWidth))];
        }

    }
}
