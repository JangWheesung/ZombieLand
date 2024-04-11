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

        // floor Ÿ�ϸ��� ��� ä���� Ÿ�� ��ġ�� Ȯ���Ͽ� ����Ʈ�� �߰�
        foreach (var cellPosition in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(cellPosition);
            if (tile != null)
            {
                // Ÿ���� �߽� ��ǥ�� ������ ����Ʈ�� �߰�
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

        // ä���� ��ġ �� �ϳ��� �������� �����Ͽ� ��ȯ
        int randomIndex = Random.Range(0, filledPositions.Count);
        return filledPositions[randomIndex];
    }

    [ClientRpc]
    private void CreateMapClientRpc(int index)
    {
        Instantiate(tilemaps[index], girdObj);
    }
}
