using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace v3_assignment
{
    public class Game1 : Game
    {
        public int mapselection;
        Random rd = new Random();

        public bool stopspawn = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        TiledMapRenderer mapRenderer;
        //counters for enemy
        int counter_1 = 1;
        int counter_2 = 2;
        int counter_3 = 3;
        int counter_4 = 4;
        // Define the timer for spawning
        float secondscountdown =  8; // 60 fps 
        LinkedList<Enemy> enemies = new LinkedList<Enemy>();
        float countdownduration;

        GameTime time = new GameTime();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("font");

            // TODO: use this.Content to load your game content here
            // Load the maps from file
            mapselection = rd.Next(0, 3);
            switch (mapselection)
            {
                case 0:
                    GameScene.Map = new GameMap(Content.Load<TiledMap>("map"));
                    break;
                case 1:
                    GameScene.Map = new GameMap(Content.Load<TiledMap>("map2"));
                    break;
                case 2:
                    GameScene.Map = new GameMap(Content.Load<TiledMap>("map3"));
                    break;

            }
            GameScene.Map.Initialize(Content);
            mapRenderer = new TiledMapRenderer(GraphicsDevice, GameScene.Map.GetTiledMap());

            //  Get the Wall Layer from the map
            TiledMapTileLayer wallLayer = GameScene.Map.GetTiledMap().GetLayer<TiledMapTileLayer>("Walls");

            // Define start row and column (must refer to a navigable tile)
            int colStart = 18;
            int rowStart = 11;



            // Construct the TileGraph
            GameScene.NavigationGraph = new TileGraph();
            GameScene.NavigationGraph.CreateFromMap(wallLayer, colStart, rowStart);

            // Initialize Animations
            GameScene.Animations = new Dictionary<string, AnimatedSprite>();
            GameScene.InitializeAnimations(Content);

            // countdown duration
            countdownduration = 60 * secondscountdown;

            // Create an object of enemy
            for (int i = 0; i < 4; i++)
            {
                Enemy enemy = new Enemy();
                //enemy.Name = "ghost" + counter;
                switch (i)
                {
                    case 0: //Top Left
                        enemy.Name = "enemy" + counter_1;
                        counter_1 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(1, 2), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;
                    case 1://Bot Left
                        enemy.Name = "enemy" + counter_2;
                        counter_2 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(1, 27), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;
                    case 2:// Top Right
                        enemy.Name = "enemy" + counter_3;
                        counter_3 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(25, 3), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;
                    case 3:// Bot Right
                        enemy.Name = "enemy" + counter_4;
                        counter_4 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(26, 26), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;

                }
            }

        


            Player pacman = new Player(15, 15);
            pacman.Name = "pacman";
            pacman.Origin = pacman.Position;
            pacman.OnReachTile += GameScene.Map.CoverTile;

            // Add all game entities to the world
            

            World.Add(pacman.Name, pacman);

            // Call Initialize() for each game entities
            World.Initialize(Content);

            // Change window size to fit the map
            _graphics.PreferredBackBufferWidth = wallLayer.Width * wallLayer.TileWidth;
            _graphics.PreferredBackBufferHeight = wallLayer.Height * wallLayer.TileHeight;
            _graphics.ApplyChanges();

        }


        private void Pacman_OnReachTile(Tile location)
        {
            throw new NotImplementedException();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            mapRenderer.Update(gameTime);

            GameScene.Map.Update(gameTime);

            //Debug.WriteLine(countdownduration);
            //EnemySpawner.Update(gameTime);
            stopspawn = World.getspawn();
            if (stopspawn)
            {
                countdownduration = 60 * secondscountdown;
            }
            else
            {
                countdownduration--;
            }
            
            if (countdownduration ==0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Enemy enemy = new Enemy();
                    //enemy.Name = "ghost" + counter;
                    switch (i)
                    {
                        case 0: //Top Left
                            enemy.Name = "enemy" + counter_1;
                            counter_1 += 4;
                            enemy.Position = Tile.ToPosition(new Tile(1, 2), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                            enemy.setRoam();
                            World.Add(enemy.Name, enemy);
                            break;
                        case 1://Bot Left
                            enemy.Name = "enemy" + counter_2;
                            counter_2 += 4;
                            enemy.Position = Tile.ToPosition(new Tile(1, 27), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                            enemy.setRoam();
                            World.Add(enemy.Name, enemy);
                            break;
                        case 2:// Top Right
                            enemy.Name = "enemy" + counter_3;
                            counter_3 += 4;
                            enemy.Position = Tile.ToPosition(new Tile(25, 3), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                            enemy.setRoam();
                            World.Add(enemy.Name, enemy);
                            break;
                        case 3:// Bot Right
                            enemy.Name = "enemy" + counter_4;
                            counter_4 += 4;
                            enemy.Position = Tile.ToPosition(new Tile(26, 26), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                            enemy.setRoam();
                            World.Add(enemy.Name, enemy);
                            break;

                    }
                }

                countdownduration = 60 * secondscountdown;
            }
            
            World.Update(gameTime);

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                World.Clear();
                addnewWorldEntity();
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            GameScene.Map.Draw(_spriteBatch, gameTime);
            mapRenderer.Draw();
            if (stopspawn)
            {
                _spriteBatch.DrawString(_font, "Game Over. Press Enter to Restart",new Vector2(GameScene.Map.TileWidth, GameScene.Map.TileHeight), Color.White);
            }
            _spriteBatch.End();

            // Draw all game entities
            World.Draw(_spriteBatch, gameTime);

            base.Draw(gameTime);
        }


        public void addnewWorldEntity()
        {
            mapselection = rd.Next(0, 3);
            switch (mapselection)
            {
                case 0:
                    GameScene.Map = new GameMap(Content.Load<TiledMap>("map"));
                    break;
                case 1:
                    GameScene.Map = new GameMap(Content.Load<TiledMap>("map2"));
                    break;
                case 2:
                    GameScene.Map = new GameMap(Content.Load<TiledMap>("map3"));
                    break;

            }
            GameScene.Map.Initialize(Content);
            mapRenderer = new TiledMapRenderer(GraphicsDevice, GameScene.Map.GetTiledMap());

            //  Get the Wall Layer from the map
            TiledMapTileLayer wallLayer = GameScene.Map.GetTiledMap().GetLayer<TiledMapTileLayer>("Walls");

            // Define start row and column (must refer to a navigable tile)
            int colStart = 18;
            int rowStart = 11;



            // Construct the TileGraph
            GameScene.NavigationGraph = new TileGraph();
            GameScene.NavigationGraph.CreateFromMap(wallLayer, colStart, rowStart);

            // Initialize Animations
            GameScene.Animations = new Dictionary<string, AnimatedSprite>();
            GameScene.InitializeAnimations(Content);

            countdownduration = 60 * secondscountdown;

            for (int i = 0; i < 4; i++)
            {
                Enemy enemy = new Enemy();
                //enemy.Name = "ghost" + counter;
                switch (i)
                {
                    case 0: //Top Left
                        enemy.Name = "enemy" + counter_1;
                        counter_1 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(1, 2), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;
                    case 1://Bot Left
                        enemy.Name = "enemy" + counter_2;
                        counter_2 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(1, 27), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;
                    case 2:// Top Right
                        enemy.Name = "enemy" + counter_3;
                        counter_3 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(25, 3), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;
                    case 3:// Bot Right
                        enemy.Name = "enemy" + counter_4;
                        counter_4 += 4;
                        enemy.Position = Tile.ToPosition(new Tile(26, 26), GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                        enemy.Origin = enemy.Position;
                        enemies.AddLast(enemy);
                        World.Add(enemy.Name, enemy);
                        break;

                }
            }




            Player pacman = new Player(15, 15);
            pacman.Name = "pacman";
            pacman.Origin = pacman.Position;
            pacman.OnReachTile += GameScene.Map.CoverTile;

            // Add all game entities to the world


            World.Add(pacman.Name, pacman);

            // Call Initialize() for each game entities
            World.Initialize(Content);
            World.setrespawn();
        }
    }
}
