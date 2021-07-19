using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccount : MonoBehaviour
{
    [SerializeField] private InputField nickNameInput;
    [SerializeField] private Text infoText;
    [SerializeField] private Button createButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(()=>{panel.SetActive(false);});
        createButton.onClick.AddListener(OnCreateButtonClick);
    }

    // Update is called once per frame
    private void OnCreateButtonClick()
    {
        var players = Global.GetInstance().playerList;

        var input = nickNameInput.text;
        
        // 유효성 체크
        foreach(var ch in input)
        {
            if(char.IsDigit(ch) || char.IsLetter(ch))
                continue;
            else
            {
                infoText.text = "닉네임은 한/영, 숫자만 가능합니다.";
                return;
            }
        }

        // 중복 체크
        foreach(var player in players)
        {
            if(player.ToLower() == input.ToLower())
            {
                infoText.text = "이미 존재하는 닉네임입니다.";
                return;
            }
        }

        // 생성 성공
        players.Add(input);
        Global.GetInstance().SavePlayerList();
    }
}
