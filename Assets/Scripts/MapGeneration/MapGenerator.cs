using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    private int width;
    private int height;
    [Header("Generation")]
    [SerializeField][Range(0, 80)] private int fillPercentage;
    [SerializeField] private int smoothnessFactor;
    [SerializeField] private int wallThresholdAmount;
    [SerializeField] private int roomThresholdAmount;
    [SerializeField] private int passageWayRadius;

    [Header("Tiles and tilemaps")]
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private Tilemap groundMap;
    [SerializeField] private Tile wallTile;
    [SerializeField] private Tile groundTile;

    int[,] map;

    private static MapGenerator instance;

    public static MapGenerator Instance { get => instance; }
    public bool Loaded { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public Vector3Int GetEmptyPosition()
    {
        int x = GameController.GetRandom.Next(width);
        int y = GameController.GetRandom.Next(height);

        if (IsInMapRange(x,y) && map[x,y] == 0)
        {
            Vector3 position = CoordToWorldPoint(new Coord(x, y));

            return new Vector3Int((int)position.x, (int)position.y, 0);
        }
        else
        {
            return GetEmptyPosition();
        }
    }

    public ref int[,] GenerateMap(int width, int height)
    {
        this.width = width;
        this.height = height;
        map = new int[width, height];

        FillMap();
        SmoothMap();
        FillHoles();
        ProcessMap();
        DrawMap();

        Loaded = true;
        return ref map;
    }

    private void FillMap()
    {
        if(map != null)
        {

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        map[x, y] = GameController.GetRandom.Next(101) < fillPercentage ? 1 : 0;
                    }
                }
            }
        }
    }

    private void SmoothMap()
    {
        for(int i = 0; i < smoothnessFactor; i++)
        {
            for(int x = 2; x < width - 3; x++)
            {
                for(int y = 2; y < height - 3; y++)
                {
                    int wallCount = GetWallCount(x,y);
                    if(wallCount > 4)
                    {
                        map[x, y] = 1;
                    }
                    else if(wallCount < 4)
                    {
                        map[x, y] = 0;
                    }
                }
            }
        }
    }
    private void FillHoles()
    {
        for (int i = 0; i < smoothnessFactor; i++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int wallCount = GetWallCount(x, y);

                    if (wallCount >= 5)
                    {
                        map[x, y] = 1;
                    }
                }
            }
        }
    }

    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        List<List<Coord>> roomRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();

        int wallThresholdSize = wallThresholdAmount;
        int roomThresholdSize = roomThresholdAmount;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if(wallRegion.Count < wallThresholdSize)
            {
                foreach(Coord tile in wallRegion)
                {
                    map[tile.X, tile.Y] = 0;
                }
            }
        }

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.X, tile.Y] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }

        survivingRooms.Sort();
        survivingRooms[0].IsMainRoom = true;
        survivingRooms[0].IsAccessibleFromMainRoom = true;
        ConnectClosestRooms(survivingRooms);
    }

    private void ConnectClosestRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in rooms)
            {
                if (room.IsAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = rooms;
            roomListB = rooms;
        }

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;

                if(roomA.ConnectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if(roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                if (roomA.IsConnected(roomB))
                {
                    possibleConnectionFound = false;
                    break;
                }
                for (int tileIndexA = 0; tileIndexA < roomA.EdgeTiles.Count; tileIndexA++)
                {
                    for(int tileIndexB = 0; tileIndexB < roomB.EdgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.EdgeTiles[tileIndexA];
                        Coord tileB = roomB.EdgeTiles[tileIndexB];

                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.X - tileB.X, 2) + Mathf.Pow(tileA.Y - tileB.Y, 2));

                        if(distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if(possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(rooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(rooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 5);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord coord in line)
        {
            DrawCircle(coord, passageWayRadius);
        }
    }

    private void DrawCircle(Coord coord, int radius)
    {
        for(int x = -radius; x <= radius; x++)
        {
            for(int y = -radius; y <= radius; y++)
            {
                if(x*x + y*y <= radius * radius)
                {
                    int drawX = coord.X + x;
                    int drawY = coord.Y + y;

                    if(IsInMapRange(drawX, drawY))
                    {
                        map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.X;
        int y = from.Y;

        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        
        for(int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;

            if(gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    private Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + 0.5f + tile.X, -height / 2 + 0.5f + tile.Y, 0);
    }

    private List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>(); // List to be returned
        int[,] mapFlags = new int[width, height]; // Keep a list of points in the map we've already looked at
        int tileType = map[startX, startY]; // The tiletype of the start (What we want to look for)
        Queue<Coord> queue = new Queue<Coord>(); // Create a queue of tiles to check for neighbours

        queue.Enqueue(new Coord(startX, startY)); // Put the start tile in the queue
        mapFlags[startX, startY] = 1; // Mark the current tile as already checked
        
        while(queue.Count > 0) // Loop until we checked every tile in this area
        {
            Coord tile = queue.Dequeue(); // Dequeue a tile
            tiles.Add(tile); // Add it to the return list

            for(int x = tile.X - 1; x <= tile.X + 1; x++)
            {
                for(int y = tile.Y - 1; y <= tile.Y + 1; y++)
                {
                    if(IsInMapRange(x,y) && (y == tile.Y || x == tile.X)) // We only want to check tiles in the map and tiles right next to the current tile
                    {
                        if(mapFlags[x,y] == 0 && map[x,y] == tileType) // Check so we haven't already checked the neighbour and it's the right tile
                        {
                            mapFlags[x, y] = 1; // Mark the tile as checked
                            queue.Enqueue(new Coord(x, y)); // Add the tile to the queue
                        }
                    }
                }
            }
        }

        return tiles;
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x,y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.X, tile.Y] = 1;
                    }
                }
            }
        }

        return regions;
    }

    private int GetWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        for (int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x == gridX && y == gridY)
                {
                    continue;
                }

                wallCount += map[x, y];
            }
        }

        return wallCount;
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void DrawMap(int[,] theMap = null)
    {
        wallMap.ClearAllTiles();
        groundMap.ClearAllTiles();
        Debug.Log("Drawing map");
        if(theMap == null)
        {
            theMap = map;
        }

        if(map != null)
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(theMap[x,y] == 0)
                    {
                        Vector3 pos = CoordToWorldPoint(new Coord(x, y));
                        //Vector3Int intPos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
                        Vector3Int intPos = new Vector3Int(x, y, 0);
                        groundMap.SetTile(intPos, groundTile);
                    }
                    else if(theMap[x,y] == 1)
                    {
                        Vector3 pos = CoordToWorldPoint(new Coord(x, y));
                        //Vector3Int intPos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
                        Vector3Int intPos = new Vector3Int(x, y, 0);
                        wallMap.SetTile(intPos, wallTile);
                    }
                }
            }
        }
    }

    private struct Coord
    {
        int tileX;
        int tileY;

        public int X { get => tileX; }
        public int Y { get => tileY; }

        public Coord(int tileX, int tileY)
        {
            this.tileX = tileX;
            this.tileY = tileY;
        }
    }

    private class Room : IComparable<Room>
    {
        private List<Coord> tiles;
        private List<Coord> edgeTiles;
        private List<Room> connectedRooms;
        private int roomSize;
        private bool isAccessibleFromMainRoom;
        private bool isMainRoom;

        public List<Coord> EdgeTiles { get => edgeTiles; }
        public List<Room> ConnectedRooms { get => connectedRooms; }
        public bool IsAccessibleFromMainRoom { get => isAccessibleFromMainRoom; set => isAccessibleFromMainRoom = value; }
        public bool IsMainRoom { get => isMainRoom; set => isMainRoom = value; }
        public Room()
        {

        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();


            foreach (Coord tile in tiles)
            {
                for (int x = tile.X - 1; x <= tile.X + 1; x++)
                {
                    for (int y = tile.Y - 1; y <= tile.Y + 1; y++)
                    {
                        if (x == tile.X || y == tile.Y)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void AddRoomToConnected(Room room)
        {
            connectedRooms.Add(room);
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.AddRoomToConnected(roomB);
            roomB.AddRoomToConnected(roomA);
        }

        public bool IsConnected(Room room)
        {
            return connectedRooms.Contains(room);
        }

        public int CompareTo(Room room)
        {
            return room.roomSize.CompareTo(roomSize);
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }
    }
}