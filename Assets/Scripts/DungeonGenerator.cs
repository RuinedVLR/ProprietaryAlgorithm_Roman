using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private List<Room_SO> roomsPresets = new List<Room_SO>();
    [SerializeField] private int numberOfRooms = 10;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            GameObject roomPrefab = roomsPresets[Random.Range(0, roomsPresets.Count)].roomPrefab;
            Vector3 position = new Vector3(i * 10, 0, 0); // Simple linear placement for demonstration
            Instantiate(roomPrefab, position, Quaternion.identity);
        }
    }
}
