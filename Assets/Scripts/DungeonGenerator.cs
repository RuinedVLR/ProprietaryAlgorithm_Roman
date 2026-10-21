using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] private int numberOfRooms = 10;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            Vector3 position = new Vector3(i * 10, 0, 0); // Simple linear placement for demonstration
            Instantiate(roomPrefab, position, Quaternion.identity);
        }
    }
}
