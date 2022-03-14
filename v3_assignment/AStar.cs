using System;
using System.Collections.Generic;
using System.Text;

namespace v3_assignment
{
    class AStar
    {
        // None = Not seen
        // Opened = Seen and is in the Priority Queue (also called 'OpenSet' / 'OpenList')
        // Closed = Seen and has been taken out of the Priority Queue.
        private enum State { None = 0, Opened, Closed };

        // NodeRecord is designed to be used solely for 
        // node selection in the priority queue
        private class NodeRecord : IComparable<NodeRecord>
        {
            public Tile self;
            public Tile parent;
            public ulong g; // cost so far
            public ulong h; // heuristic
            public State state; // default is None

            public NodeRecord(Tile self, ulong g, ulong h)
            {
                this.self = self;
                this.g = g;
                this.h = h;
            }

            public int CompareTo(NodeRecord rhs)
            {
                ulong f1 = this.g + this.h;
                ulong f2 = rhs.g + rhs.h;
                return (int)(f1 - f2);
            }

            public override string ToString()
            {
                return self.ToString();
            }
        }

        // A "function pointer" pointing to a heuristic function
        public delegate ulong Heuristic(Tile start, Tile end);

        public static LinkedList<Tile> Compute(TileGraph graph, Tile start, Tile end, Heuristic heuristic)
        {
            // Create node records to store data used by AStar
            Dictionary<Tile, NodeRecord> nodeRecords = new Dictionary<Tile, NodeRecord>();

            // Initialize g and h values
            foreach (Tile node in graph.Nodes)
                nodeRecords.Add(node, new NodeRecord(node, ulong.MaxValue, heuristic(node, end)));
            nodeRecords[start] = new NodeRecord(start, 0, heuristic(start, end));

            // Priority Queue for deciding which node to process
            PriorityQueue<NodeRecord> pq = new PriorityQueue<NodeRecord>();

            // Push the node record for start node to the priority queue
            NodeRecord startRecord = nodeRecords[start];
            pq.Push(startRecord);
            startRecord.state = State.Opened;

            NodeRecord endRecord = nodeRecords[end];
            NodeRecord curRecord, neighbourRecord;

            // Help navigations
            int[] MoveRow = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] MoveCol = { -1, 0, 1, -1, 1, -1, 0, 1 };

            while (!pq.Empty())
            {
                curRecord = pq.Top();
                pq.Pop();
                curRecord.state = State.Closed;

                if (curRecord.self == end)
                    break;

                // Get the cost to neighbours of "curRecord.self"
                ulong[] connections = graph.Connections[curRecord.self];

                for (int i = 0; i < connections.Length; ++i)
                {
                    // if there is a connection
                    if (connections[i] != 0)
                    {
                        int col = curRecord.self.Col + MoveCol[i];
                        int row = curRecord.self.Row + MoveRow[i];

                        Tile neighbourTile = new Tile(col, row);
                        neighbourRecord = nodeRecords[neighbourTile];
                    }
                    else continue;

                    // Ignore neighbours in Closed state
                    if (neighbourRecord.state == State.Closed)
                        continue;

                    // Update g
                    ulong gNew = curRecord.g + connections[i];
                    if (neighbourRecord.g > gNew)
                    {
                        neighbourRecord.g = gNew;
                        neighbourRecord.parent = curRecord.self;
                    }

                    // Update PQ
                    if (neighbourRecord.state == State.Opened)
                        pq.Update(neighbourRecord);
                    else
                    {
                        pq.Push(neighbourRecord);
                        neighbourRecord.state = State.Opened;
                    }
                }

            }

            return ConstructPath(nodeRecords, start, end);
        }

        private static LinkedList<Tile> ConstructPath(Dictionary<Tile, NodeRecord> nodeRecords, Tile start, Tile end)
        {
            LinkedList<Tile> path = new LinkedList<Tile>();

            for (NodeRecord cur = nodeRecords[end]; cur.parent != null;)
            {
                path.AddFirst(cur.self);
                cur = nodeRecords[cur.parent];
            }
            path.AddFirst(start);

            return path;
        }
    }
}
