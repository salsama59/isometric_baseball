using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using UnityEngine.Tilemaps;

public class PathFindingManager : MonoBehaviour {

    public Tilemap map;

    public List<Vector3Int> GetCalculatedMapPath(Vector2Int start, Vector2Int end) {

        SquareGrid grid = new SquareGrid(GameManager.StadiumColumnMaximum, GameManager.StadiumRowMaximum);
        grid.tiles = GenerateTileMapLocations();

        // Run A*
        AStarSearch astar = new AStarSearch(grid, new Location(start.x, start.y), new Location(end.x, end.y));
        List<Location> path = DeterminePathFromAStar(astar, new Location(start.x, start.y), new Location(end.x, end.y));

        List<Vector3Int> mapPositions = new List<Vector3Int>();
        foreach (Location l in path) {
            if (grid.tiles.Contains(l)) { // Did we select inside the known grid?
                mapPositions.Add(new Vector3Int(l.x, l.y, (int)map.transform.position.y));
            }
        }

       return mapPositions;
    }
    public HashSet<Location> GenerateTileMapLocations()
    {

        HashSet<Location> blockMap2D = new HashSet<Location>();

        //Iterate through all of the indices
        // get the highest non-empty block in each column.
        for (int collumn = GameManager.StadiumColumnMinimum; collumn < GameManager.StadiumColumnMaximum + 1; collumn++)
        {
            for (int row = GameManager.StadiumRowMinimum; row < GameManager.StadiumRowMaximum + 1; row++)
            {
                if (map.GetTile(new Vector3Int(collumn, row, (int)map.transform.position.y)) != null)
                {
                    Location l = new Location(collumn, row);
                    if (!blockMap2D.Contains(l))
                    {
                        blockMap2D.Add(new Location(collumn, row));
                    }
                }
            }
        }
        return blockMap2D;
    }

    public List<Location> DeterminePathFromAStar(AStarSearch search, Location start, Location end) {
        List<Location> path = new List<Location>();
        Location current = start;
        Location next = end;
        path.Add(end);
        while (!next.Equals(current)) {
            current = next;
            if (search.cameFrom.ContainsKey(next)) {
                next = search.cameFrom[next];
                if (!current.Equals(next)) {
                    path.Add(next);
                }
            }
        }

        path.Reverse();
        return path;
    }

}
