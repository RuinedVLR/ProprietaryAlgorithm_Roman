using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Room_SO : ScriptableObject
{
    [SerializeField] private int doorCount;
    [SerializeField] List<Side> sidesWithDoors = new List<Side>();

    private enum Side { Top, Right, Bottom, Left }
}
