using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class HostGameManager : MonoBehaviour
{
    private Allocation allocation;
    private string joinCode;
    private const int maxConnections = 20;

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError("호스트 연결 오류");
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch(Exception e)
        {
            Debug.LogError("조인코드 받기 에러");
            return;
        }
    }
}
