using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace v3_assignment
{
    public class RoamHCFSM
    {
        // Navigation states
        public enum NavigationState { STOP, MOVING , END};

        // Current state
        // Initial state is STOP state.
        private NavigationState m_currentState = NavigationState.STOP;

        // Navigation
        private Tile m_srcTile;
        private Tile m_destTile;
        private LinkedList<Tile> m_path = null;

        // Junction node list
        private List<Tile> m_junctionTiles = new List<Tile>();

        // Random generator
        private Random m_rand = new Random();

        // Ghost agent by which this FSM possessed
        private Enemy m_agent;
        private GameEntity m_targetAgent;

        public RoamHCFSM(Enemy agent, GameEntity targetAgent)
        {
            // Set the agent which this FSM controls
            m_agent = agent;
            m_targetAgent = targetAgent;
            // Initialize junction nodes
            InitializeJunctionNodes();

            // Reset
            EntryAction();
        }

        public void EntryAction()
        {
            // Reset to initial state
            m_currentState = NavigationState.STOP;

            // Reset source tile
            m_srcTile = Tile.ToTile(m_agent.Position, GameScene.Map.TileWidth, GameScene.Map.TileHeight);

            // Reset path
            if (m_path != null)
                m_path.Clear();
        }

        public void Update(GameTime gameTime)
        {
            // Run Navigation FSM
            if (m_currentState == NavigationState.STOP)
            {
                // Randomly choose a junction tile
                m_destTile = m_junctionTiles[m_rand.Next(m_junctionTiles.Count)];

                // If (1) Random tile is a navigable tile that ...
                //    (2) ... is not where the ghost is standing
                if (GameScene.NavigationGraph.Nodes.Contains(m_destTile) &&
                    !m_destTile.Equals(m_srcTile)
                    )
                {
                    /* Transition Action */
                    m_path = AStar.Compute(GameScene.NavigationGraph, m_srcTile, m_destTile, m_agent.Euclidean);


                    // Remove source tile from path
                    m_path.RemoveFirst();

                    // Play the animation based on its next node in the path
                    Tile nextTile = m_path.First.Value;
                    Tile diff = new Tile(nextTile.Col - m_srcTile.Col, nextTile.Row - m_srcTile.Row);

                    


                    // Determine which animation to play
                    if (diff.Col < 0)
                        m_agent.SetAnimationKey("ghostRedLeft");
                    else if (diff.Col > 0)
                        m_agent.SetAnimationKey("ghostRedRight");
                    else if (diff.Row < 0)
                        m_agent.SetAnimationKey("ghostRedUp");
                    else if (diff.Row > 0)
                        m_agent.SetAnimationKey("ghostRedDown");

                    // Update Animation
                    GameScene.Animations["ghostRed"].Play(m_agent.GetAnimationKey());
                    GameScene.Animations["ghostRed"].Update(0.0f);

                    // Change to MOVING state
                    m_currentState = NavigationState.MOVING;
                }

                /* Action to execute on this state */

                // No action
            }
            else if (m_currentState == NavigationState.MOVING)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Check transition
                int tileWidth = GameScene.Map.TileWidth;
                int tileHeight = GameScene.Map.TileHeight;

                // Reach destination
                if (m_agent.Position.Equals(Tile.ToPosition(m_destTile, tileWidth, tileHeight)))
                {
                    // Update source tile to destination tile
                    m_srcTile = m_destTile;
                    m_destTile = null;

                    // Change to STOP state
                    m_currentState = NavigationState.STOP;
                }
                else
                {

                    /* Action to execute on this state */
                    try
                    {
                        Tile nextTile = m_path.First.Value; // throw exception if path is empty

                        Vector2 nextTilePosition = Tile.ToPosition(nextTile, tileWidth, tileHeight);


                        if (nextTilePosition == m_targetAgent.Position)
                        {

                            m_currentState = NavigationState.END;
                        }

                        if (m_agent.Position.Equals(nextTilePosition))
                        {

                            // Console.WriteLine("Reach head tile. Removing head tile. Get next node from path.");
                            m_path.RemoveFirst();

                            // Get the next destination position
                            Tile currentTile = nextTile;
                            nextTile = m_path.First.Value; // throw exception if path is empty

                            // Change animation
                            Tile diff = new Tile(nextTile.Col - currentTile.Col, nextTile.Row - currentTile.Row);
                            if (diff.Col < 0) // LEFT
                                m_agent.SetAnimationKey("ghostRedLeft");
                            else if (diff.Col > 0) // RIGHT
                                m_agent.SetAnimationKey("ghostRedRight");
                            else if (diff.Row < 0) // UP
                                m_agent.SetAnimationKey("ghostRedUp");
                            else if (diff.Row > 0) // DOWN
                                m_agent.SetAnimationKey("ghostRedDown");
                        }


                        // Move the ghost to the new tile location
                        m_agent.Move(nextTilePosition, elapsedSeconds, 100.0);


                        // Update animation
                        GameScene.Animations["ghostRed"].Play(m_agent.GetAnimationKey());
                        GameScene.Animations["ghostRed"].Update(elapsedSeconds);
                    }
                    catch (Exception ex)
                    {
                        if (ex is NullReferenceException || ex is InvalidOperationException)
                        {
                            Console.WriteLine("Path empty or Head tile unreachable.");

                            // For safety, clear the path in case the exception was not caused by empty path.
                            m_path.Clear();

                            // Update source tile to destination tile
                            m_srcTile = Tile.ToTile(m_agent.Position, GameScene.Map.TileWidth, GameScene.Map.TileHeight);
                            m_destTile = null;

                            // Change to STOP state.
                            m_currentState = NavigationState.STOP;
                        }
                        else throw;
                    }
                }
            }

            else if(m_currentState == NavigationState.END)
            {
                m_path.Clear();
                //m_targetAgent.Alive = false;
                World.stopall();
                Console.WriteLine("Game Over.");
            }
        }

        // Store a list of junction tiles
        private void InitializeJunctionNodes()
        {
            foreach (Tile tile in GameScene.NavigationGraph.Nodes)
            {
                var connections = GameScene.NavigationGraph.Connections[tile];

                // Sum indices where connection exists
                int sumIndex = 0;
                for (int i = 0; i < connections.Length; ++i)
                    if (connections[i] > 0)
                        sumIndex += i;

                // Non-junctions have sumIndex = 7
                // 012
                // 3 4
                // 567
                if (sumIndex != 7)
                    m_junctionTiles.Add(tile);
            }
        }
    }
}
