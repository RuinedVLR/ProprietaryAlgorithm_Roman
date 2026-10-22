using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Room", menuName = "Dungeon/Room")]
public class Room_SO : ScriptableObject
{
    [SerializeField] private int doorCount;
    [SerializeField] List<Side> sidesWithDoors = new List<Side>();

    private enum Side { Top, Right, Bottom, Left }

    [SerializeField] public GameObject roomPrefab;
}
