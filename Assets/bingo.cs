using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class bingo : MonoBehaviour
{
    public GameObject editPanel;
    public Button rankingButton;
    public Button menuButton;
    public Button infoButton;

    private List<GameObject> titleList = new List<GameObject>(); 
    private List<GameObject> textGridList = new List<GameObject>();
    private List<GameObject> inputTitleList = new List<GameObject>();
    private List<GameObject> inputTextGridList = new List<GameObject>();

    private float waitTime = 15f;

    private float mouseAxisX = 0;

    private float mouseAxisY = 0;
    // Start is called before the first frame update
    void Start()
    {
        rankingButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Ranking");
        });
        
        infoButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Info");
        });
        
        menuButton.onClick.AddListener(() =>
        {
            UpdateBingo(editPanel.activeSelf);
            editPanel.SetActive(!editPanel.activeSelf);
        });
        

        for (int i = 1; i < 10; ++i)
        {
            titleList.Add(GameObject.Find("title_"+i.ToString()));
            textGridList.Add(GameObject.Find("textGrid_"+i.ToString()));
            inputTitleList.Add(editPanel.transform.Find("inputTitle_"+i.ToString()).gameObject);
            inputTextGridList.Add(editPanel.transform.Find("inputTextGrid_"+i.ToString()).gameObject);
        }
        
        var strFilePath = Application.dataPath+"/bingo.txt";
        string readstr;
        using (FileStream fs = new FileStream(strFilePath, FileMode.OpenOrCreate))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                int index = 0;
                while ((readstr = sr.ReadLine()) != null)
                {
                    string title = readstr.Split('_')[0];
                    string player = readstr.Split('_')[1];

                    titleList[index].GetComponent<Text>().text = title;
                    inputTitleList[index].GetComponent<InputField>().text = title;

                    textGridList[index].GetComponent<Text>().text = player;
                    inputTextGridList[index].GetComponent<InputField>().text = player;
                    ++index;
                }
                sr.Close();
            }
            fs.Close();
        }

        StartCoroutine(ChangeSceneTimer());
    }

    public IEnumerator ChangeSceneTimer()
    {
        while (true)
        {
            if (!editPanel.activeSelf && mouseAxisX == 0 && mouseAxisY == 0)
            {
                if (waitTime <= 0)
                {
                    // save file
                    string saveFile = DateTime.Now.ToString("MM-dd-hh") + "-bingo.txt";
                    
                    var strFilePath = Application.dataPath+"/"+saveFile;
                    using(FileStream fs = new FileStream(strFilePath, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            for (int i = 0; i < 9; ++i)
                            {
                                string line = titleList[i].GetComponent<Text>().text + "_" +
                                              textGridList[i].GetComponent<Text>().text;
                                sw.WriteLine(line);
                            }
                            sw.Close();
                        }
                        fs.Close();
                    }

                    SceneManager.LoadScene("Info");
                }
                --waitTime;
            }
            else
            {
                waitTime = 15;
            }
            yield return new WaitForSeconds(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mouseAxisX = Input.GetAxis("Mouse X");
        mouseAxisY = Input.GetAxis("Mouse Y");
    }

    void UpdateBingo(bool value)
    {
        if (value)
        {
            // edit -> board
            for (int i = 0; i < 9; ++i)
            {
                titleList[i].GetComponent<Text>().text = inputTitleList[i].GetComponent<InputField>().text;
                textGridList[i].GetComponent<Text>().text = inputTextGridList[i].GetComponent<InputField>().text;
            }
            
            var strFilePath = Application.dataPath+"/bingo.txt";
            using(FileStream fs = new FileStream(strFilePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < 9; ++i)
                    {
                        string line = titleList[i].GetComponent<Text>().text + "_" +
                                      textGridList[i].GetComponent<Text>().text;
                        sw.WriteLine(line);
                    }
                    sw.Close();
                }
                fs.Close();
            }
        }
        else
        {
            // board -> edit
            for (int i = 0; i < 9; ++i)
            {
                inputTitleList[i].GetComponent<InputField>().text = titleList[i].GetComponent<Text>().text;
                inputTextGridList[i].GetComponent<InputField>().text = textGridList[i].GetComponent<Text>().text;
            }
        }
    }
}
