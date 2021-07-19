using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class info : MonoBehaviour
{
    public GameObject editPanel;
    public Button editButton;
    public Button bingoButton;
    public Button rankingButton;
    public Text[] texts;
    public InputField[] inputFields;
    
    private float waitTime = 15f;

    private float mouseAxisX = 0;

    private float mouseAxisY = 0;
    // Start is called before the first frame update
    void Start()
    {
        //load file
        {
            var strFilePath = Application.dataPath + "/info.txt";
        
            string readstr;
            using (FileStream fs = new FileStream(strFilePath, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    int index = 0;
                    while ((readstr = sr.ReadLine()) != null)
                    {
                        texts[index].text = readstr;
                        ++index;
                    }
        
                    sr.Close();
                }
        
                fs.Close();
            }
        }

        bingoButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Bingo");
        });
        
        rankingButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Ranking");
        });
        
        editButton.onClick.AddListener(() =>
        {
            if (!editPanel.activeSelf)
            {
                editPanel.SetActive(true);
                
                // text -> input
                for (int i = 0; i < 4; ++i)
                {
                    inputFields[i].text = texts[i].text;
                }
            }
            else
            {
                editPanel.SetActive(false);
                
                // input -> text
                for (int i = 0; i < 4; ++i)
                {
                    texts[i].text = inputFields[i].text;
                }
                
                // save file
                var strFilePath = Application.dataPath+"/info.txt";
                using(FileStream fs = new FileStream(strFilePath, FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        foreach (InputField input in inputFields)
                        {
                            string line = input.text;
                            sw.WriteLine(line);
                        }
                        sw.Close();
                    }
                    fs.Close();
                }
            }
        });

        StartCoroutine(ChangeSceneTimer());
    }

    // Update is called once per frame
    void Update()
    {
        mouseAxisX = Input.GetAxis("Mouse X");
        mouseAxisY = Input.GetAxis("Mouse Y");
    }

    public IEnumerator ChangeSceneTimer()
    {
        while (true)
        {
            if (!editPanel.activeSelf && mouseAxisX == 0 && mouseAxisY == 0)
            {
                if (waitTime <= 0)
                {
                    SceneManager.LoadScene("Ranking");
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
}
