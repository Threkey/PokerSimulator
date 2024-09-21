using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button btnStart;
    public Button btnLoad;
    public Button btnDescription;
    public Button btnQuit;

    Button btnCloseDescription;
    GameObject panelDescription;

    // Start is called before the first frame update
    void Start()
    {
        GameManager gm = GameManager.Instance;

        panelDescription = GameObject.Find("Canvas").transform.Find("Panel_Description").gameObject;
        btnCloseDescription = panelDescription.transform.Find("Button_CloseDescription").GetComponent<Button>();

        btnStart.onClick.AddListener(LoadInitSettingSecene);
        btnLoad.onClick.AddListener(LoadGame);
        btnDescription.onClick.AddListener(ShowDescriptionUI);
        btnQuit.onClick.AddListener(QuitGame);
        btnCloseDescription.onClick.AddListener(ClosePanel);
    }

    void LoadInitSettingSecene()
    {
        SceneManager.LoadScene("InitSettingScene");
    }

    void LoadGame()
    {
        // 플레이어프렙스로 저장한 내용 불러오기
    }

    void ShowDescriptionUI()
    {
        panelDescription.SetActive(true);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void ClosePanel()
    {
        panelDescription.SetActive(false);
    }
}
