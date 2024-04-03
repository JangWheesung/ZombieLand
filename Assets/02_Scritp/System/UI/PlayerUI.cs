using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerText;

    public void SetPlayerUIData(UserData userData)
    {
        playerText.text = userData.nickName;
    }
}
