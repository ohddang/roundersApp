using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ranking : MonoBehaviour
{
    public GameObject menuPanel;
    public Button bingoButton;
    public Button menuButton;
    public Button infoButton;

    // Start is called before the first frame update
    void Start()
    {
        bingoButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Bingo");
        });
        
        infoButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Info");
        });
        
        menuButton.onClick.AddListener(() =>
        {
            menuPanel.SetActive(true);
            menuPanel.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
