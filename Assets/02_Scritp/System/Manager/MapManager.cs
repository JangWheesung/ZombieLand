using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;

public class MapManager : NetworkBehaviour
{
    public static MapManager Instance;

    [SerializeField] private GameObject[] tilemaps;
    [SerializeField] private Transform girdObj;
    private Tilemap tilemap;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateMap()
    {
        int randomIndex = Random.Range(0, tilemaps.Length);
        tilemap = tilemaps[randomIndex].GetComponent<Tilemap>();

        CreateMapClientRpc(randomIndex);
    }

    public Vector3 GetRandomPointInFloorMap()
    {
        Debug.Log(tilemap);
        var filledPositions = new List<Vector3>();

        // floor 타일맵의 모든 채워진 타일 위치를 확인하여 리스트에 추가
        foreach (var cellPosition in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(cellPosition);
            if (tile != null)
            {
                // 타일의 중심 좌표를 가져와 리스트에 추가
                Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                
                Debug.Log(tileCenter);
                filledPositions.Add(cellPosition);
            }
        }

        if (filledPositions.Count == 0)
        {
            Debug.LogWarning("There are no filled positions in the floor tilemap.");
            return Vector3.zero;
        }

        // 채워진 위치 중 하나를 무작위로 선택하여 반환
        int randomIndex = Random.Range(0, filledPositions.Count);
        return filledPositions[randomIndex];
    }

    [ClientRpc]
    private void CreateMapClientRpc(int index)
    {
        Instantiate(tilemaps[index], girdObj);
    }
}
