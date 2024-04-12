using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerWinUI : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TMP_Text nameText;

    public void SetData(UserData data)
    {
        img.color = data.playerRole == PlayerRole.Human ? Color.blue : Color.red;
        nameText.text = data.nickName;
    }
}
