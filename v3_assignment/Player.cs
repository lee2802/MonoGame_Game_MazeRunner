using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Animations;

namespace v3_assignment
{
    struct InputMapping
    {
        internal InputMapping(Tile direction, String animationKey)
        {
            this.direction = direction;
            this.animationKey = animationKey;
        }

        internal Tile direction;
        internal String animationKey;

    }

    public class Player : GameEntity
    {
        /// Delegates ///
        public delegate void ReachTileHandler(Tile location);

        /// Events ///
        public event ReachTileHandler OnReachTile;

        // Speed of the player movement
        private float m_speed;

        // A name to access the player animation sprite
        private String m_animationKey;

        // States for movement
        private enum MoveState { STOP, MOVING };

        // Current move state
        private MoveState m_currentMoveState = MoveState.STOP;

        // Which tile location pacman is in
        private Tile m_tileLocation;

        
        // Pixel location of the next destination
        private Vector2 m_destination;

        // Mapping from input keys to direction vector and animation key
        private Dictionary<Keys, InputMapping> m_inputMapping;

        // The key corresponding to the current movement direction
        private Keys m_currentKey;

        // The key that was pressed during MOVING state
        // - cause pacman to change direction half-way whenever possible
        private Keys m_desiredKey;

        public Player(int col, int row)
        {
            // Initialize pacman location (by Tile or by Pixel location)
            m_tileLocation = new Tile(col, row);
            Position = Tile.ToPosition(m_tileLocation, GameScene.Map.TileWidth, GameScene.Map.TileHeight);
            
            // Initialize speed and animation key
            m_speed = 150.0f;
            m_animationKey = "pacmanRight";
            GameScene.Animations["pacman"].OriginNormalized = Vector2.Zero; // set the origin of each sprite image 
            GameScene.Animations["pacman"].Play(m_animationKey); // selects the correct animation
            GameScene.Animations["pacman"].Update(0); // to show the first frame of the animation, and not the first frame of the sprite image.

            // Initialize input mapping
            m_inputMapping = new Dictionary<Keys, InputMapping>();
            m_inputMapping.Add(Keys.Up, new InputMapping(new Tile(0, -1), "pacmanUp"));
            m_inputMapping.Add(Keys.Down, new InputMapping(new Tile(0, 1), "pacmanDown"));
            m_inputMapping.Add(Keys.Left, new InputMapping(new Tile(-1, 0), "pacmanLeft"));
            m_inputMapping.Add(Keys.Right, new InputMapping(new Tile(1, 0), "pacmanRight"));

            // Initialize direction
            m_currentKey = Keys.None;
            m_desiredKey = Keys.None;


        }
        public override void Initialize(ContentManager content)
        {
            // TODO: Load all pacman content

            // Call all callbacks for OnReachTile
            OnReachTile(m_tileLocation);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Pacman movement based on user input
            KeyboardState keyboard = Keyboard.GetState();
           
            // Check if any keys in @{m_inputMapping} has been pressed.
            Keys pressedKey = Keys.None;
            foreach (Keys key in m_inputMapping.Keys)
                if (keyboard.IsKeyDown(key))
                {
                    pressedKey = key;
                    break;
                }

            if (m_currentMoveState == MoveState.STOP)
            {
                // Transition to MOVING state
                if (pressedKey != Keys.None)
                {
                    // Update current direction via @{m_currentKey}
                    m_currentKey = pressedKey;

                    // Update next tile location
                    Tile nextTile = new Tile(m_tileLocation.Col, m_tileLocation.Row);
                    nextTile.Col += m_inputMapping[m_currentKey].direction.Col;
                    nextTile.Row += m_inputMapping[m_currentKey].direction.Row;

                    if (GameScene.NavigationGraph.Nodes.Contains(nextTile))
                    {
                        // Update animation key
                        m_animationKey = m_inputMapping[m_currentKey].animationKey;

                        // Calculate the position of new destination
                        m_destination = Tile.ToPosition(nextTile, GameScene.Map.TileWidth, GameScene.Map.TileHeight);

                        m_currentMoveState = MoveState.MOVING;
                    }
                }

                // No action for STOP state.
            }
            else if (m_currentMoveState == MoveState.MOVING)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Update animations
                GameScene.Animations["pacman"].Play(m_animationKey);
                GameScene.Animations["pacman"].Update(elapsedSeconds);

                // Update position
                Position = Move(Position, m_destination, elapsedSeconds, m_speed);

                // Update current key is there is a pressed key
                if (pressedKey != Keys.None)
                    m_desiredKey = pressedKey;

                // Reach the destination tile
                if (Position.Equals(m_destination))
                {
                    // Update tile location
                    m_tileLocation = Tile.ToTile(Position, GameScene.Map.TileWidth, GameScene.Map.TileHeight);

                    // Invoke OnReachTile event
                    OnReachTile(m_tileLocation);

                    // Check next tile, located at the direction corresponding to desired key
                    // If it's block tile
                    //    continue moving according to current key
                    //    change to STOP state if
                    Keys[] keysToCheck = { m_desiredKey, m_currentKey };
                    Keys keySelected = Keys.None;

                    Tile nextTile = m_tileLocation;

                    foreach (Keys k in keysToCheck)
                    {
                        if (k != Keys.None)
                        {
                            nextTile = new Tile(m_tileLocation.Col, m_tileLocation.Row);
                            nextTile.Col += m_inputMapping[k].direction.Col;
                            nextTile.Row += m_inputMapping[k].direction.Row;

                            if (GameScene.NavigationGraph.Nodes.Contains(nextTile))
                            {
                                keySelected = k;
                                if (k == m_desiredKey)
                                {
                                    m_currentKey = m_desiredKey;
                                    m_desiredKey = Keys.None;
                                }
                                break;
                            }
                        }
                    }

                    // Transition to STOP state
                    if (keySelected == Keys.None)
                    {
                        m_currentKey = Keys.None;
                        m_currentMoveState = MoveState.STOP;
                    }
                    // Continue moving to the next destination
                    else
                    {
                        // Update animation key
                        m_animationKey = m_inputMapping[keySelected].animationKey;

                        // Update destination
                        m_destination = Tile.ToPosition(nextTile, GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                    }

                }

            }


        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw the animation sprite
            spriteBatch.Draw(GameScene.Animations["pacman"], Position);
        }

        // Given source (src) and destination (dest) locations, elapsed time, and speed, 
        //     try to move from source to destination at the given speed within elapsed time.
        // If cannot reach dest within the elapsed time, return the location where it will reach
        // If can reach or overshoots the destination, the return @{dest}.
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

        public Tile getTilePos()
        {
            return m_tileLocation;
        }
    }
}
