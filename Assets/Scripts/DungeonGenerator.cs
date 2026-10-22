using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Side = Room_SO.Side;
using RoomType = Room_SO.RoomType;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private List<Room_SO> roomsPresets = new List<Room_SO>();
    [SerializeField] private int numberOfRooms = 10;
    [SerializeField] private float roomSize = 10f;
    [SerializeField] private bool autoDetectRoomSize = true;
    [SerializeField] private int maxDoorsPerNonMainRoom = 2;
    [SerializeField] private Transform dungeonParent;

    private static readonly Side[] AllSides = { Side.Top, Side.Right, Side.Bottom, Side.Left };

    void Start()
    {
        GenerateDungeon();
    }

    public void RegenerateDungeon()
    {
        foreach (Transform child in dungeonParent)
        {
            Destroy(child.gameObject);
        }
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        Room_SO mainRoomPreset = roomsPresets.FirstOrDefault(r => r.Type == RoomType.Main);
        if (mainRoomPreset == null)
        {
            Debug.LogError("DungeonGenerator: no Room_SO with RoomType.Main found in roomsPresets.");
            return;
        }

        Vector2 cellSize = autoDetectRoomSize ? DetectRoomWorldSize(mainRoomPreset.roomPrefab) : new Vector2(roomSize, roomSize);

        // Grid based random layout generation
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>(); // Tracks occupied room positions in the grid
        Dictionary<Vector2Int, HashSet<Side>> doors = new Dictionary<Vector2Int, HashSet<Side>>(); // Tracks which sides have doors for each room position
        List<Vector2Int> frontier = new List<Vector2Int>(); // Rooms that are candidates for expansion

        Vector2Int origin = Vector2Int.zero;
        occupied.Add(origin);
        doors[origin] = new HashSet<Side>();
        frontier.Add(origin);

        int safety = numberOfRooms * 20;
        while (occupied.Count < numberOfRooms && frontier.Count > 0 && safety-- > 0) // Continue until have enough rooms or run out of candidates or hit safety limit
        {
            Vector2Int current = frontier[Random.Range(0, frontier.Count)];
            bool expanded = false;

            bool currentIsOrigin = current == origin;
            if (!currentIsOrigin && doors[current].Count >= maxDoorsPerNonMainRoom)
            {
                frontier.Remove(current);
                continue;
            }

            foreach (Side side in Shuffle(AllSides))
            {
                Vector2Int neighborPos = current + SideToOffset(side);
                if (occupied.Contains(neighborPos))
                    continue;

                if (!currentIsOrigin)
                {
                    HashSet<Side> candidateSides = new HashSet<Side>(doors[current]) { side };
                    if (!IsDoorPatternSupported(candidateSides))
                        continue;
                }

                if (HasUnintendedAdjacency(neighborPos, current, occupied))
                    continue;

                occupied.Add(neighborPos);
                doors[current].Add(side);
                doors[neighborPos] = new HashSet<Side> { OppositeSide(side) };
                frontier.Add(neighborPos);
                expanded = true;
                break;
            }

            if (!expanded)
                frontier.Remove(current);
        }

        // Place rooms based on required door connections.
        foreach (var kvp in doors)
        {
            Vector2Int gridPos = kvp.Key;
            HashSet<Side> requiredSides = kvp.Value;

            RoomType desiredType = gridPos == origin
                ? RoomType.Main
                : requiredSides.Count == 1
                    ? RoomType.End
                    : RoomType.Hall;

            Room_SO preset = PickRoomPreset(desiredType, requiredSides) ?? PickRoomPreset(RoomType.Hall, requiredSides);
            if (preset == null)
            {
                Debug.LogWarning($"DungeonGenerator: no matching Room_SO preset found for position {gridPos} with doors {string.Join(",", requiredSides)}.");
                continue;
            }

            Vector3 worldPosition = new Vector3(gridPos.x * cellSize.x, gridPos.y * cellSize.y, 0f);
            Instantiate(preset.roomPrefab, worldPosition, Quaternion.identity, dungeonParent);
        }
    }

    private bool IsDoorPatternSupported(HashSet<Side> requiredSides)
    {
        return roomsPresets.Any(r => r.Type != RoomType.Main && r.HasDoorsFor(requiredSides));
    }

    private static bool HasUnintendedAdjacency(Vector2Int newPos, Vector2Int cameFrom, HashSet<Vector2Int> occupied)
    {
        foreach (Side side in AllSides)
        {
            Vector2Int neighborOfNew = newPos + SideToOffset(side);
            if (neighborOfNew == cameFrom)
                continue;

            if (occupied.Contains(neighborOfNew))
                return true;
        }

        return false;
    }

    private Vector2 DetectRoomWorldSize(GameObject roomPrefab)
    {
        if (roomPrefab == null)
            return new Vector2(roomSize, roomSize);

        GameObject temp = Instantiate(roomPrefab);
        temp.SetActive(true);

        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();
        Vector2 detectedSize = new Vector2(roomSize, roomSize);

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            detectedSize = new Vector2(bounds.size.x, bounds.size.y);
        }

        Destroy(temp);
        return detectedSize.x > 0f && detectedSize.y > 0f ? detectedSize : new Vector2(roomSize, roomSize);
    }

    private Room_SO PickRoomPreset(RoomType type, HashSet<Side> requiredSides)
    {
        List<Room_SO> candidates = roomsPresets.Where(r => r.Type == type && r.HasDoorsFor(requiredSides)).ToList();
        if (candidates.Count == 0)
            return null;

        // Prefer presets whose door count matches exactly, to avoid unused open doors when possible.
        Room_SO exactMatch = candidates.FirstOrDefault(r => r.SidesWithDoors.Count == requiredSides.Count);
        return exactMatch ?? candidates[Random.Range(0, candidates.Count)];
    }

    private static Vector2Int SideToOffset(Side side)
    {
        switch (side)
        {
            case Side.Top: return new Vector2Int(0, 1);
            case Side.Right: return new Vector2Int(1, 0);
            case Side.Bottom: return new Vector2Int(0, -1);
            case Side.Left: return new Vector2Int(-1, 0);
            default: return Vector2Int.zero;
        }
    }

    private static Side OppositeSide(Side side)
    {
        switch (side)
        {
            case Side.Top: return Side.Bottom;
            case Side.Right: return Side.Left;
            case Side.Bottom: return Side.Top;
            case Side.Left: return Side.Right;
            default: return side;
        }
    }

    private static IEnumerable<Side> Shuffle(Side[] sides)
    {
        List<Side> list = new List<Side>(sides);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}
