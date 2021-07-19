using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Threading;
using UnityEngine.SceneManagement;

public class editor : MonoBehaviour
{
    public Dropdown dropDownPlayerList;
    public Dropdown dropDownSearchPlayerList;
    public InputField inputPassword;
    public InputField inputNickName;
    public InputField inputPhoneNumber;
    public Button buttonSearch;
    public Button buttonAddPlayer;
    public Text textNickName;
    public Text textCurrentWinPoint;
    public Text textCurrentPoint;
    public GameObject AdminPanel;
    public GameObject menuPanel;
    public InputField inputChangeNickName;
    public InputField inputWinPoint;
    public InputField inputPoint;
    public Button buttonChangeNickName;
    public Button buttonAddWinPoint;
    public Button buttonAddPoint;
    public Text textInfo;
    public Button buttonClose;

    private string password;

    struct Data{
        public int winPoint;
        public int point;
    }

    private Dictionary<string, Data> playerList = new Dictionary<string, Data>();
    private List<string> searchPlayerList = new List<string>();

    string currentKey = "";
    private float mouseAxisX = 0f;
    private float mouseAxisY = 0f;
    private float waitTime = 15f;
    
    // Start is called before the first frame update

    void Awake()
    {
        InitData();
    }

    void Start()
    {
        // Init
        AdminPanel.SetActive(false);

        var strFilePath = Application.dataPath+"/password.txt";
        string readstr;
        FileStream fs = new FileStream(strFilePath, FileMode.OpenOrCreate);
        StreamReader sr = new StreamReader(fs);

        while((readstr = sr.ReadLine()) != null)
        {
            password = readstr;
        }
        sr.Close();
        fs.Close();

        buttonAddPlayer.onClick.AddListener(AddPlayer);
        buttonSearch.onClick.AddListener(SearchPlayer);
        buttonChangeNickName.onClick.AddListener(ChangeNickName);
        buttonAddWinPoint.onClick.AddListener(AddWinPoint);
        buttonAddPoint.onClick.AddListener(AddPoint);
        buttonClose.onClick.AddListener(OnClose);

        dropDownPlayerList.onValueChanged.AddListener(delegate {DropdownALLValueChanged(dropDownPlayerList);});
        dropDownSearchPlayerList.onValueChanged.AddListener(delegate {DropdownSearchValueChanged(dropDownSearchPlayerList);});

        InitData();

        StartCoroutine(ChangeSceneTimer());
    }
    
    public IEnumerator ChangeSceneTimer()
    {
        while (true)
        {
            if (!menuPanel.activeSelf && mouseAxisX == 0 && mouseAxisY == 0)
            {
                if (waitTime <= 0)
                {
                    // save file
                    string saveFile = DateTime.Now.ToString("MM-dd-hh") + "-playerList.txt";
                    var strFilePath = Application.dataPath+"/"+saveFile;
                    using(FileStream fs = new FileStream(strFilePath, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            foreach (var item in playerList)
                            {
                                Data data = item.Value;
                                string line = item.Key + " " + item.Value.winPoint.ToString() + " " + item.Value.point.ToString();
                                sw.WriteLine(line);
                                Thread.Sleep(10);
                            }

                            sw.Close();
                        }
                        fs.Close();
                    }
                    
                    SceneManager.LoadScene("Bingo");
                }
                --waitTime;
            }
            else
            {
                waitTime = 15;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(password == inputPassword.text)
        {
            AdminPanel.SetActive(true);
        }
        else
        {
            AdminPanel.SetActive(false);
        }

        if (inputNickName.isFocused)
        {
            if(Input.GetKeyUp(KeyCode.Tab))
                inputPhoneNumber.Select();
        }
        else if (inputPhoneNumber.isFocused)
        {
            if(Input.GetKeyUp(KeyCode.Tab))
                inputNickName.Select();
        }
        
        mouseAxisX = Input.GetAxis("Mouse X");
        mouseAxisY = Input.GetAxis("Mouse Y");
    }

    void InitData()
    {
        inputPassword.text = "";
        inputNickName.text = "";
        inputPhoneNumber.text = "";

        inputWinPoint.text = "";
        inputPoint.text = "";

        textNickName.text = "Nick Name";
        textCurrentWinPoint.text = "0";
        textCurrentPoint.text = "0";

        dropDownPlayerList.ClearOptions();
        LoadPlayerList();
    }

    void UpdatePlayerList()
    {
        dropDownPlayerList.ClearOptions();
        foreach(var item in playerList)
        {
            string nickName = item.Key.Split('_')[0];
            string number = item.Key.Split('_')[1];
            dropDownPlayerList.options.Add(new Dropdown.OptionData() {text = item.Key});
        }
        dropDownPlayerList.RefreshShownValue();
    }

    void AddPlayer()
    {
        string key = inputNickName.text + "_" + inputPhoneNumber.text;

        if(playerList.ContainsKey(key))
            textInfo.text = "이미 존재하는 플레이어";
        else
        {
            Data emptyData = new Data();
            playerList.Add(key, emptyData);
            textInfo.text = "플레이어 추가 완료";
            UpdatePlayerList();
            SavePlayerList();
        }

        textNickName.text = inputNickName.text;
        textCurrentPoint.text = "0";
        textCurrentWinPoint.text = "0";
        currentKey = key;
    }

    void ChangeNickName()
    {
        if(playerList.ContainsKey(currentKey))
        {
            var currentData = playerList[currentKey];
            string number = currentKey.Split('_')[1];

            string newKey = inputChangeNickName.text +"_"+number;
            if(playerList.ContainsKey(newKey))
                textInfo.text = "이미 존재하는 플레이어";
            else
            {
                playerList.Add(newKey, currentData);
                playerList.Remove(currentKey);
                currentKey = newKey;

                textNickName.text = inputChangeNickName.text;
                textInfo.text = "닉네임 변경 완료";            
                UpdatePlayerList();
                SavePlayerList();
            }
        }
        else{
            textInfo.text = "존재하지 않는 플레이어";
        }
    }
    void AddWinPoint()
    {
        if(playerList.ContainsKey(currentKey))
        {
            Data newData = playerList[currentKey];
            newData.winPoint += int.Parse(inputWinPoint.text);

            playerList[currentKey] = newData;
            textCurrentWinPoint.text = newData.winPoint.ToString();
            textInfo.text = "승점 추가 완료";
            SavePlayerList();
        }
        else
        {
            textInfo.text = "존재하지 않는 플레이어";
        }
    }

    void AddPoint()
    {
        if(playerList.ContainsKey(currentKey))
        {
            Data newData = playerList[currentKey];
            newData.point += int.Parse(inputPoint.text);

            playerList[currentKey] = newData;
            textCurrentPoint.text = newData.point.ToString();
            textInfo.text = "포인트 추가 완료";
            SavePlayerList();
        }
        else
        {
            textInfo.text = "존재하지 않는 플레이어";
        }
    }

    void UpdateRankingBoard()
    {
        var sortPlayerList = playerList.OrderByDescending(data => data.Value.winPoint);

        int index = 0;
        foreach(var item in sortPlayerList)
        {
            if(index >= 20)
                break;

            GameObject panel = GameObject.Find("Panel_"+(index+1).ToString());
            panel.transform.GetChild(0).GetComponent<Text>().text = item.Key.Split('_')[0];
            panel.transform.GetChild(1).GetComponent<Text>().text = item.Value.winPoint.ToString();
            ++index;
        }
    }

    public void SearchPlayer()
    {
        searchPlayerList.Clear();
        dropDownSearchPlayerList.ClearOptions();

        if(inputNickName.text != "" && inputPhoneNumber.text != "")
        {
            string key = inputNickName.text + "_" + inputPhoneNumber.text;
            if(playerList.ContainsKey(key))
            {
                dropDownSearchPlayerList.options.Add(new Dropdown.OptionData() {text = key});
                searchPlayerList.Add(key);
            }
            else
            {
                textInfo.text = "존재하지 않는 정보";
            }
        }
        else if(inputNickName.text != "")
        {
            foreach(var item in playerList)
            {
                string nickName = item.Key.Split('_')[0];
                string number = item.Key.Split('_')[1];
                
                if(inputNickName.text == nickName)
                {
                    dropDownSearchPlayerList.options.Add(new Dropdown.OptionData() {text = item.Key});
                    searchPlayerList.Add(item.Key);
                }    
            }
        }
        else if(inputPhoneNumber.text != "")
        {
            foreach(var item in playerList)
            {
                string nickName = item.Key.Split('_')[0];
                string number = item.Key.Split('_')[1];
                
                if(inputPhoneNumber.text == number)
                {
                    dropDownSearchPlayerList.options.Add(new Dropdown.OptionData() {text = item.Key});
                    searchPlayerList.Add(item.Key);
                }    
            }
        }
        dropDownSearchPlayerList.RefreshShownValue();

        if(dropDownSearchPlayerList.options.Count == 0)
            textInfo.text = "존재하지 않는 정보";
        else
        {
            string key = searchPlayerList[0];
            Data data = playerList[key];

            textNickName.text = key.Split('_')[0];
            textCurrentWinPoint.text = data.winPoint.ToString();
            textCurrentPoint.text = data.point.ToString();

            currentKey = key;

            textInfo.text = "검색 완료";
        }

        textInfo.text = "";
    }

    void DropdownALLValueChanged(Dropdown change)
    {
        int index = change.value;
        string key = playerList.ElementAt(index).Key;
        Data data = playerList.ElementAt(index).Value;

        textNickName.text = key.Split('_')[0];
        textCurrentWinPoint.text = data.winPoint.ToString();
        textCurrentPoint.text = data.point.ToString();

        currentKey = key;
    }

    void DropdownSearchValueChanged(Dropdown change)
    {
        int index = change.value;
        string key = searchPlayerList[index];
        Data data = playerList[key];

        textNickName.text = key.Split('_')[0];
        textCurrentWinPoint.text = data.winPoint.ToString();
        textCurrentPoint.text = data.point.ToString();

        currentKey = key;
    }

    void SavePlayerList()
    {
        var strFilePath = Application.dataPath+"/playerList.txt";
        using(FileStream fs = new FileStream(strFilePath, FileMode.OpenOrCreate))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                Debug.Log(playerList.Count);
                foreach (var item in playerList)
                {
                    string line = item.Key + " " + item.Value.winPoint.ToString() + " " + item.Value.point.ToString();
                    sw.WriteLine(line);
                    Thread.Sleep(10);
                }

                sw.Close();
            }
            fs.Close();
        }
        UpdateRankingBoard();
    }

    void LoadPlayerList()
    {
        playerList.Clear();
        
        var strFilePath = Application.dataPath+"/playerList.txt";
        string readstr;
        using (FileStream fs = new FileStream(strFilePath, FileMode.OpenOrCreate))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                while ((readstr = sr.ReadLine()) != null)
                {
                    string key = readstr.Split(' ')[0];
                    string winPoint = readstr.Split(' ')[1];
                    string point = readstr.Split(' ')[2];

                    Data newData;
                    newData.winPoint = int.Parse(winPoint);
                    newData.point = int.Parse(point);

                    playerList.Add(key, newData);
                }
                sr.Close();
            }
            fs.Close();
        }

        UpdatePlayerList();
        UpdateRankingBoard();
    }

    void OnClose()
    {
        InitData();
        GameObject.Find("MenuPanel").SetActive(false);
    }
}