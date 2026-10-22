using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Room", menuName = "Dungeon/Room")]
public class Room_SO : ScriptableObject
{
    public enum Side { Top, Right, Bottom, Left }
    public enum RoomType { Main, Hall, End }

    [SerializeField] private RoomType roomType = RoomType.Hall;
    [SerializeField] private int doorCount;
    [SerializeField] private List<Side> sidesWithDoors = new List<Side>();

    [SerializeField] public GameObject roomPrefab;

    public RoomType Type => roomType;
    public IReadOnlyList<Side> SidesWithDoors => sidesWithDoors;

    public bool HasDoorsFor(IEnumerable<Side> requiredSides)
    {
        foreach (Side side in requiredSides)
        {
            if (!sidesWithDoors.Contains(side))
                return false;
        }
        return true;
    }
}
