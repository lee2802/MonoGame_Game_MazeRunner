using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace v3_assignment
{
    class GameScene
    {
        public static TileGraph NavigationGraph;
        public static GameMap Map; // public static TiledMap Map;
        public static SpriteSheet GameSpriteSheet;
        public static Dictionary<string, AnimatedSprite> Animations;

        public static void InitializeAnimations(ContentManager content)
        {
            GameSpriteSheet = content.Load<SpriteSheet>("pacmanSprites.sf", new JsonContentLoader());
            Animations["ghostRed"] = new AnimatedSprite(GameSpriteSheet);
            Animations["pacman"] = new AnimatedSprite(GameSpriteSheet);
        }
    }
}
