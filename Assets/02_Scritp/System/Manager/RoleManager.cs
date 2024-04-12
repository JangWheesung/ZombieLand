using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public enum PlayerRole
{
    None,
    Human,
    Zombie
}

public class RoleManager : NetworkBehaviour
{
    public static RoleManager Instance { get; private set; }

    public event Action OnGameEndEvt;

    [SerializeField] private Image fadeImage;
    [SerializeField] private Image timerCircle;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text subText;

    [Header("Value")]
    [SerializeField] private float playTime;

    public float Timer => currentTime;
    private float currentTime;
    private bool startTimer;

    private readonly string zombieRead = "Your role is Zombie!";
    private readonly string humanRead = "Your role is Human!";
    private readonly string zombieSubRead = "Infect everyone!";
    private readonly string humanSubRead = "Survive till the end!";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentTime = playTime;

        GameManager.Instance.OnPlayerSpawnEndEvt += SetTimer;
        GameManager.Instance.OnPlayerSpawnEndEvt += OnRoleText;
    }

    private void FixedUpdate()
    {
        if (!startTimer) return;

        if (IsClient)
        {
            timerCircle.fillAmount = currentTime / playTime;
            int min = Mathf.FloorToInt(currentTime / 60f);
            int sec = Mathf.CeilToInt(((currentTime / 60f) - min) * 60);
            timerText.text = $"{min}:{sec}";

            if(currentTime > 0)
                currentTime -= Time.fixedDeltaTime;
        }

        if (!IsServer) return;

        if (currentTime <= 0 || 
            GameManager.Instance.GetPlayersCount() == GameManager.Instance.GetPlayersCount(PlayerRole.Zombie))
        {
            NextGameSceneClientRpc();
            OnGameEndEvt?.Invoke();
            //°ÔÀÓ ³¡
        }
    }

    public void AssignedToRole()
    {
        //Random.Range(0, NetworkManager.ConnectedClients.Count)
        int zombieIndex = 1;
        foreach (var item in NetworkManager.ConnectedClientsIds)
        {
            PlayerRole newPlayerRole = zombieIndex == 0 ? PlayerRole.Zombie : PlayerRole.Human;
            GameManager.Instance.PlayerRoleChange(item, newPlayerRole);

            zombieIndex--;
        }
    }

    private void SetTimer()
    {
        startTimer = true;
        timerCircle.gameObject.SetActive(true);
    }

    private void OnRoleText()
    {
        PlayerRole role = GameManager.Instance.localPlayerController.playerRole;

        if (role == PlayerRole.Human)
        {
            roleText.color = Color.blue;
            roleText.text = humanRead;
            subText.text = humanSubRead;
        }
        else if (role == PlayerRole.Zombie)
        {
            roleText.color = Color.red;
            roleText.text = zombieRead;
            subText.text = zombieSubRead;
        }

        DOTween.Sequence()
            .Append(roleText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCubic))
            .Insert(1.5f, roleText.transform.DOScale(Vector3.zero, 0.5f));
    }

    #region ServerRpc



    #endregion

    #region ClientRpc

    [ClientRpc]
    private void NextGameSceneClientRpc()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1, 2f).OnComplete(() => 
        {
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(SceneList.WinScene, LoadSceneMode.Single);
            }
        });
    }

    #endregion
}
