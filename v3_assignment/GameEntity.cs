using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace v3_assignment
{
    public abstract class GameEntity
    {
        public bool Alive;
        public string Name;
        public Vector2 Position;
        public Vector2 Origin;

        //public Vector2 rangeofmap_1;
        //public Vector2 rangeofmap_2;
        protected GameEntity()
        {
            Alive = true;
            Name = string.Empty;
            Position = Vector2.Zero;
            Origin = Vector2.Zero;
            
        }

        public abstract void Initialize(ContentManager content);

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
