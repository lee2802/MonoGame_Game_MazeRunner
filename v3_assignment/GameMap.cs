using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace v3_assignment
{
    class GameMap
    {
        /// Publics ///
        public int TileWidth
        {
            get { return m_map.TileWidth; }
        }

        public int TileHeight
        {
            get { return m_map.TileHeight; }
        }

        /// Delegates ///
        public delegate void FoodClearedHandler();

        /// Events ///
        public event FoodClearedHandler OnFoodCleared;

        /// Privates ///

        // Tile map
        private TiledMap m_map;

        // Tiles that cover eaten tiles
        private HashSet<Tile> m_coverTiles;

        // Texture for cover tile
        private Texture2D m_texture;

        // Where the cover tile texture can be found in the texture
        private Rectangle m_sourceRect;



        public GameMap(TiledMap map)
        {
            m_map = map;
            m_coverTiles = new HashSet<Tile>();
        }

        public void Initialize(ContentManager content)
        {
            m_texture = content.Load<Texture2D>("pacman-wall-24");
            m_sourceRect = new Rectangle(96, 0, 24, 24);
        }

        public void Update(GameTime gameTime)
        {
           
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw all cover tiles
            foreach (Tile t in m_coverTiles)
            {
                Vector2 position = Tile.ToPosition(t, m_map.TileWidth, m_map.TileHeight);
                spriteBatch.Draw(m_texture, position, m_sourceRect, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1f);
            }
        }

        public void CoverTile(Tile location)
        {
            var mapLayer = m_map.GetLayer<TiledMapTileLayer>("BG");

            TiledMapTile? tilePacman;
            bool hasTile = mapLayer.TryGetTile((ushort)location.Col, (ushort)location.Row, out tilePacman);

            if (hasTile)
            {
                
                if (tilePacman.Value.GlobalIdentifier == 4)
                {
                    
                    if (!m_coverTiles.Contains(location))
                    {
                        m_coverTiles.Add(location);

                        // Fires OnFoodCleared event if all food has been cleared
                        if (m_coverTiles.Count == 6)
                            World.stopall();

                    }
                }
            }
        }

        public TiledMap GetTiledMap()
        {
            return m_map;
        }

    }
}
