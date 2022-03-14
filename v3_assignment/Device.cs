using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace v3_assignment
{
    class Device : GameEntity
    {
        private Texture2D m_texture;
        private string m_texture_name;
        private Rectangle m_textureTileRect;
        private Tile srcTile;

        public Device()
        {
            m_texture_name = "pacmanSprites";
        }

        public override void Initialize(ContentManager content)
        {
            m_texture = content.Load<Texture2D>(m_texture_name);
            Tile tile = new Tile(6, 5);

            Vector2 tilePosition = Tile.ToPosition(tile, GameScene.Map.TileWidth, GameScene.Map.TileHeight);

            m_textureTileRect = new Rectangle
            {
                X = (int)tilePosition.X,
                Y = (int)tilePosition.Y,
                Width = GameScene.Map.TileWidth,
                Height = GameScene.Map.TileHeight
            };

            // Initialize source tile from current position
            srcTile = Tile.ToTile(Position, GameScene.Map.TileWidth, GameScene.Map.TileHeight);
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(m_texture, Position, m_textureTileRect, Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 1f);
        }
    }
}
