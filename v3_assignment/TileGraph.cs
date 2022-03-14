using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace v3_assignment
{
    class TileGraph
    {
        public HashSet<Tile> Nodes;
        public Dictionary<Tile, ulong[]> Connections;

        public TileGraph()
        {
            Nodes = new HashSet<Tile>();
            Connections = new Dictionary<Tile, ulong[]>();
        }

        public void CreateFromMap(TiledMapTileLayer mapLayer, int colStart, int rowStart)
        {
            bool hasTile = mapLayer.TryGetTile((ushort)colStart, (ushort)rowStart, out TiledMapTile? tile);

            if (hasTile)
            {
                BFSConstructGraph(mapLayer, colStart, rowStart);
            }
            else
            {
                throw new Exception("Error: ColStart or RowStart is Invalid.");
            }
        }

        private void BFSConstructGraph(TiledMapTileLayer mapLayer, int colStart, int rowStart)
        {
            int[] MoveRow = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] MoveCol = { -1, 0, 1, -1, 1, -1, 0, 1 };
            ulong[] Cost = { 2, 1, 2, 1, 1, 2, 1, 2 }; // Diagonal : 2, Non-Diagonal : 1
            int[] Direction = { 1, 3, 4, 6 }; // 4 directions

            Tile startTile = new Tile(colStart, rowStart);

            Nodes.Add(startTile);

            // PROBLEM 3(b) : Create a queue to contain Tile object for BFS traversal.
            // - Kindly uncomment the next line and fill up the blanks.
            Queue<Tile> q = new Queue<Tile>();

            q.Enqueue(startTile);

            // Continue looping if the queue is NOT empty
            while (q.Count > 0)
            {
                Tile currentTile = q.Dequeue();

                foreach (int direction in Direction)
                {
                    ushort rowNeighbour = (ushort)(currentTile.Row + MoveRow[direction]);
                    ushort colNeighbour = (ushort)(currentTile.Col + MoveCol[direction]);


                    // A valid neighbour satisfies the following criteria:
                    // (1) Row and column is within the number of rows and columns respectively
                    // (2) A tile exists in map layer at location (column, row)
                    // (3) Global Identifier is 0 (i.e. navigable area)
                    if ((0 <= rowNeighbour && rowNeighbour < mapLayer.Height) &&
                        (0 <= colNeighbour && colNeighbour < mapLayer.Width) &&
                        mapLayer.TryGetTile(colNeighbour, rowNeighbour, out TiledMapTile? neighbourTiledMapTile) &&
                        neighbourTiledMapTile.Value.GlobalIdentifier == 0)
                    {
                        // Add neighbour node and its connections to graph
                        Tile neighbourTile = new Tile(colNeighbour, rowNeighbour);

                        // If neighbour node not yet created or does not exist yet.
                        if (!Nodes.Contains(neighbourTile))
                        {
                            // Add neighbour tile to the graph (this also marks it as visited)
                            Nodes.Add(neighbourTile);

                            // Add neighbour to queue
                            q.Enqueue(neighbourTile);
                        }

                        // Create a new connection joining the current node with the neighbour node
                        if (!Connections.TryGetValue(currentTile, out ulong[] weights))
                        {
                            weights = new ulong[Cost.Length]; // Allocate 8 ulongs
                            weights[direction] = Cost[direction];
                            Connections.Add(currentTile, weights);
                        }
                        else
                        {
                            Connections[currentTile][direction] = Cost[direction];
                        }
                    }
                }
            }
        }
    }
}
