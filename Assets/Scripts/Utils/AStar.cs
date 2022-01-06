using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {


    // A* needs only a WeightedGraph and a location type L, and does *not*
    // have to be a grid. However, in the example code I am using a grid.
    public interface WeightedGraph<L> {
        double Cost(Location a, Location b);
        IEnumerable<Location> Neighbors(Location id);
    }


    public struct Location {
        // Implementation notes: I am using the default Equals but it can
        // be slow. You'll probably want to override both Equals and
        // GetHashCode in a real project.

        public readonly int x, y;
        public Location(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Location l) {
            return x == l.x && y == l.y;
        }
    }

    public class SquareGrid : WeightedGraph<Location> {
        // Implementation notes: I made the fields public for convenience,
        // but in a real project you'll probably want to follow standard
        // style and make them private.

        //8 directions for neighbors
        public static readonly Location[] DIRS = new[]
            {
            new Location(1, 0),
            new Location(1, -1),
            new Location(0, -1),
            new Location(-1, -1),
            new Location(-1, 0),
            new Location(-1, 1),
            new Location(0, 1),
            new Location(1, 1)
        };

        public int width, height;
        public HashSet<Location> tiles = new HashSet<Location>();
        public HashSet<Location> walls = new HashSet<Location>();
        public HashSet<Location> forests = new HashSet<Location>();

        public static double LOWEST_COST = 1;

        public SquareGrid(int width, int height) {
            this.width = width;
            this.height = height;
        }

        public bool InBounds(Location id) {
            return 0 <= id.x && id.x < width
                && 0 <= id.y && id.y < height;
        }

        public bool Passable(Location id) {
            return !walls.Contains(id);
        }
        public bool WithinReach(Location a, Location b) {
            return tiles.Contains(a) && tiles.Contains(b);
        }

        public double Cost(Location a, Location b) {
            return forests.Contains(b) ? 5 : LOWEST_COST;
        }


        public IEnumerable<Location> Neighbors(Location id) {
            foreach (var dir in DIRS) {
                Location next = new Location(id.x + dir.x, id.y + dir.y);
                if (tiles.Contains(next) && Passable(next) && WithinReach(id, next)) {
                    yield return next;
                }
            }
        }
    }


    public class PriorityQueue<T> {
        // I'm using an unsorted array for this example, but ideally this
        // would be a binary heap. There's an open issue for adding a binary
        // heap to the standard C# library: https://github.com/dotnet/corefx/issues/574
        //
        // Until then, find a binary heap class:
        // * https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
        // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
        // * http://xfleury.github.io/graphsearch.html
        // * http://stackoverflow.com/questions/102398/priority-queue-in-net

        private List<Tuple<T, double>> elements = new List<Tuple<T, double>>();

        public int Count {
            get { return elements.Count; }
        }

        public void Enqueue(T item, double priority) {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue() {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++) {
                if (elements[i].Item2 < elements[bestIndex].Item2) {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }


    /* NOTE about types: in the main article, in the Python code I just
     * use numbers for costs, heuristics, and priorities. In the C++ code
     * I use a typedef for this, because you might want int or double or
     * another type. In this C# code I use double for costs, heuristics,
     * and priorities. You can use an int if you know your values are
     * always integers, and you can use a smaller size number if you know
     * the values are always small. */

    public class AStarSearch {
        public Dictionary<Location, Location> cameFrom
            = new Dictionary<Location, Location>();
        public Dictionary<Location, double> costSoFar
            = new Dictionary<Location, double>();

        // Note: a generic version of A* would abstract over Location and
        // also Heuristic

        /**
         * Heuristic calculation for 4 direction movement on grid (manhatan distance)
         */
        /*static public double Heuristic(Location a, Location b) {
            int dx = Math.Abs(a.x - b.x);
            int dy = Math.Abs(a.y - b.y);
            return dx + dy;
        }*/

        /**
         * Heuristic calculation for 8 direction movement on grid (diagonal distance)
         */
        static public double Heuristic(Location a, Location b)
        {
            int dx = Math.Abs(a.x - b.x);
            int dy = Math.Abs(a.y - b.y);
            double d = SquareGrid.LOWEST_COST;
            double d2 = Math.Sqrt(2);
            return d * (dx + dy) + (d2 - 2 * d) * Math.Min(dx, dy);
        }

        public AStarSearch(WeightedGraph<Location> graph, Location start, Location goal) {
            var frontier = new PriorityQueue<Location>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0) {
                var current = frontier.Dequeue();

                if (current.Equals(goal)) {
                    break;
                }

                foreach (var next in graph.Neighbors(current)) {
                    double newCost = costSoFar[current]
                        + graph.Cost(current, next);
                    if (!costSoFar.ContainsKey(next)
                        || newCost < costSoFar[next]) {
                        costSoFar[next] = newCost;
                        double priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }
        }
    }

    public class Test {
        static void DrawGrid(SquareGrid grid, AStarSearch astar) {
            // Print out the cameFrom array
            for (var y = 0; y < 10; y++) {
                for (var x = 0; x < 10; x++) {
                    Location id = new Location(x, y);
                    Location ptr = id;
                    if (!astar.cameFrom.TryGetValue(id, out ptr)) {
                        ptr = id;
                    }
                    if (grid.walls.Contains(id)) {
                        Console.Write("##");
                    } else if (ptr.x == x + 1) {
                        Console.Write("\u2192 ");
                    } else if (ptr.x == x - 1) {
                        Console.Write("\u2190 ");
                    } else if (ptr.y == y + 1) {
                        Console.Write("\u2193 ");
                    } else if (ptr.y == y - 1) {
                        Console.Write("\u2191 ");
                    } else {
                        Console.Write("* ");
                    }
                }
                Console.WriteLine();
            }
        }

        static void Main() {
            // Make "diagram 4" from main article
            var grid = new SquareGrid(10, 10);
            for (var x = 1; x < 4; x++) {
                for (var y = 7; y < 9; y++) {
                    grid.walls.Add(new Location(x, y));
                }
            }
            grid.forests = new HashSet<Location>
                {
                new Location(3, 4), new Location(3, 5),
                new Location(4, 1), new Location(4, 2),
                new Location(4, 3), new Location(4, 4),
                new Location(4, 5), new Location(4, 6),
                new Location(4, 7), new Location(4, 8),
                new Location(5, 1), new Location(5, 2),
                new Location(5, 3), new Location(5, 4),
                new Location(5, 5), new Location(5, 6),
                new Location(5, 7), new Location(5, 8),
                new Location(6, 2), new Location(6, 3),
                new Location(6, 4), new Location(6, 5),
                new Location(6, 6), new Location(6, 7),
                new Location(7, 3), new Location(7, 4),
                new Location(7, 5)
            };

            // Run A*
            var astar = new AStarSearch(grid, new Location(1, 4),
                                        new Location(8, 5));

            DrawGrid(grid, astar);
        }
    }
}
