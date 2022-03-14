using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace v3_assignment
{
    public class Enemy : GameEntity
    {
        // Animation string
        private string m_animationKey;

        // Behaviour States
        private enum BehaviourState {ROAM};

        // Current behaviour state
       // private BehaviourState m_behaviourState = BehaviourState.ROAM;

        // Roam HCFSM
        RoamHCFSM roamFSM;
        // Chase HCFSM
        //ChaseHCFSM chaseFSM;
        // Evade HCFSM
        //EvadeHCFSM evadeFSM;

        public Enemy()
        {
            m_animationKey = "ghostRedLeft";
            GameScene.Animations["ghostRed"].OriginNormalized = Vector2.Zero; // set the origin of each sprite image 
            GameScene.Animations["ghostRed"].Play(m_animationKey); // selects the correct animation
            GameScene.Animations["ghostRed"].Update(0); // to show the first frame of the animation, and not the first frame of the sprite image.
        }

        public void setRoam()
        {
            roamFSM = new RoamHCFSM(this, World.Objects["pacman"]);
        }

        public override void Initialize(ContentManager content)
        {
            // Create rectangle to extract the ghost from the texture
            Tile tile = new Tile(0, 6);

            Vector2 tilePosition = Tile.ToPosition(tile, GameScene.Map.TileWidth, GameScene.Map.TileHeight);

            // Initialize Roam FSM
            roamFSM = new RoamHCFSM(this, World.Objects["pacman"]);
            //chaseFSM = new ChaseHCFSM(this, World.Objects["pacman"]);
            //evadeFSM = new EvadeHCFSM(this, World.Objects["pacman"]);
        }

        public override void Update(GameTime gameTime)
        {
            // Parameters for the condition of ROAM <-> CHASE transition
            Player pacman = (Player)World.Objects["pacman"];



           // Debug.WriteLine(test);
            roamFSM.Update(gameTime);

            

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw the ghost sprite at a position.
            spriteBatch.Draw(GameScene.Animations["ghostRed"], Position);
        }

        public void SetAnimationKey(string key)
        {
            m_animationKey = key;
        }

        public string GetAnimationKey()
        {
            return m_animationKey;
        }

        public void Move(Vector2 dest, double elapsedSeconds, double speed)
        {
            Vector2 dP = dest - Position;
            float distance = dP.Length();
            float step = (float)(speed * elapsedSeconds);

            if (step < distance)
            {
                dP.Normalize();
                Position += (dP * step);
            }
            else
                Position = dest;
        }

        // Given source (src) and destination (dest) locations, elapsed time, and speed, 
        //     try to move from source to destination at the given speed within elapsed time.
        // If cannot reach dest within the elapsed time, return the location where it will reach
        // If can reach or over-reach the dest, the return dest.
        private Vector2 Move(Vector2 src, Vector2 dest, double elapsedSeconds, double speed)
        {
            Vector2 dP = dest - src;
            float distance = dP.Length();
            float step = (float)(speed * elapsedSeconds);

            if (step < distance)
            {
                dP.Normalize();
                return src + (dP * step);
            }
            else
                return dest;
        }

        // Manhattan distance between two tiles
        public ulong Manhattan(Tile start, Tile end)
        {
            int tileWidth = GameScene.Map.TileWidth;
            int tileHeight = GameScene.Map.TileHeight;

            Vector2 s = Tile.ToPosition(start, tileWidth, tileHeight);
            Vector2 e = Tile.ToPosition(end, tileWidth, tileHeight);

            float dx = e.X - s.X;
            float dy = e.Y - s.Y;
            return (ulong)(Math.Abs(dx) + Math.Abs(dy));
        }

        // Euclidean distance between two tiles
        public ulong Euclidean(Tile start, Tile end)
        {
            int tileWidth = GameScene.Map.TileWidth;
            int tileHeight = GameScene.Map.TileHeight;

            Vector2 s = Tile.ToPosition(start, tileWidth, tileHeight);
            Vector2 e = Tile.ToPosition(end, tileWidth, tileHeight);

            return (ulong)(e - s).Length();
        }

    } 
    
}
