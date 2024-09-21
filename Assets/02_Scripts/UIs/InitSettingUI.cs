using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitSettingUI : MonoBehaviour
{
    GameManager gm;

    [Header("Canvas")]
    public Transform canvas;

    [Header("NumberOfPlayer")]
    public GameObject numberOfPlayer;
    public Button btnNOPLeft;
    public Button btnNOPRight;
    public TextMeshProUGUI textNOP;

    [Header("Stack")]
    public GameObject stack;
    public Button btnStackLeft;
    public Button btnStackRight;
    public TextMeshProUGUI textStack;

    [Header("Bet")]
    public GameObject bet;
    public Button btnBetLeft;
    public Button btnBetRight;
    public TextMeshProUGUI textBet;

    [Header("Others")]
    public Button btnAutoNameGenerator;
    public Button btnDefault;
    public Button btnStart;
    public Button btnCancel;
    public Button btnHidden;

    [Header("Warning")]
    public GameObject panelWarning;

    [Header("RandomNames")]
    public string[] names = new string[35];
    public string[] hiddenNames = new string[7];

    GameObject[] playerName = new GameObject[10];

    int playerCount = 2;
    int stackMoney = 100;
    int smallBlind = 1;
    int bigBlind = 2;

    void Start()
    {
        gm = GameManager.Instance;

        for (int i = 0; i < playerName.Length; i++)
        {
            playerName[i] = canvas.Find("Parent_Name (" + i + ")").gameObject;
        }

        btnNOPLeft.onClick.AddListener(RemovePlayer);
        btnNOPRight.onClick.AddListener(AddPlayer);
        btnStackLeft.onClick.AddListener(DecreaseStack);
        btnStackRight.onClick.AddListener(IncreaseStack);
        btnBetLeft.onClick.AddListener(DecreaseBet);
        btnBetRight.onClick.AddListener(IncreaseBet);
        btnDefault.onClick.AddListener(SetDefault);
        btnAutoNameGenerator.onClick.AddListener(AutoNameGenerator);
        btnHidden.onClick.AddListener(HiddenNamer);
        btnStart.onClick.AddListener(ConfirmSetting);
        btnCancel.onClick.AddListener(LoadTitleScene);
        panelWarning.transform.Find("Image_Warning").Find("Button_CloseWarning").GetComponent<Button>().onClick.AddListener(CloseWarning);

    }

    void Update()
    {
        // 플레이어 수
        textNOP.text = playerCount + "명";

        if (playerCount <= 2)
            btnNOPLeft.interactable = false;
        else
            btnNOPLeft.interactable = true;

        if (playerCount >= 10)
            btnNOPRight.interactable = false;
        else
            btnNOPRight .interactable = true;

        // 스택
        textStack.text = stackMoney + "$";

        if (stackMoney <= 50)
            btnStackLeft.interactable = false;
        else
            btnStackLeft.interactable = true;

        if (stackMoney >= 1000)
            btnStackRight.interactable = false;
        else
            btnStackRight .interactable = true;

        // 배팅
        textBet.text = smallBlind + "/" + bigBlind;

        if (smallBlind <= 1)
            btnBetLeft.interactable = false;
        else
            btnBetLeft .interactable = true;

        if (smallBlind >= 5)
            btnBetRight.interactable = false;
        else
            btnBetRight.interactable = true;
    }

    void RemovePlayer()
    {
        playerCount--;
        playerName[playerCount].SetActive(false);
    }

    void AddPlayer()
    {
        playerCount++;
        playerName[playerCount - 1].SetActive(true);
    }

    void DecreaseStack()
    {
        stackMoney -= 50;

        if (textStack.fontSize != 80.0f)
            textStack.fontSize = 80.0f;
    }

    void IncreaseStack()
    {
        stackMoney += 50;

        if (stackMoney >= 1000)
            textStack.fontSize = 65.0f;
    }

    void DecreaseBet()
    {
        smallBlind -= 1;
        bigBlind = smallBlind * 2;
    }

    void IncreaseBet()
    {
        smallBlind += 1;
        bigBlind = smallBlind * 2;
    }

    void SetDefault()
    {
        stackMoney = 100;
        smallBlind = 1;
        bigBlind = smallBlind * 2;
    }

    void AutoNameGenerator()
    {
        int index;
        int[] listOfIndex = new int[playerName.Length];
        System.Array.Fill(listOfIndex, -1);

        for (int i = 0; i < playerName.Length; i++)
        {
            index = Random.Range(0, names.Length);

            // 랜덤으로 뽑은 index중에 중복이 있으면 다시 뽑음
            for(int j = 0; j < playerName.Length; )
            {
                if (listOfIndex[j] == index)
                {
                    j = 0;
                    index = Random.Range(0, names.Length);
                }
                else
                {
                    j++;
                }
            }

            listOfIndex[i] = index;
            playerName[i].transform.Find("InputField_Name").GetComponent<TMP_InputField>().text = names[index];
        }
    }

    void HiddenNamer()
    {
        //인원 세팅
        playerCount = 7;

        for(int i = 0; i < playerCount; i++)
            if (playerName[i].activeSelf == false)
                playerName[i].SetActive(true);

        for (int i = playerCount; i < playerName.Length; ++i)
            if (playerName[i].activeSelf == true)
                playerName[i].SetActive(false);

        SetDefault();


        int index;
        int[] listOfIndex = new int[playerCount];
        System.Array.Fill(listOfIndex, -1);



        for (int i = 0; i < playerCount; i++)
        {
            index = Random.Range(0, hiddenNames.Length);

            // 랜덤으로 뽑은 index중에 중복이 있으면 다시 뽑음
            for (int j = 0; j < playerCount;)
            {
                if (listOfIndex[j] == index)
                {
                    j = 0;
                    index = Random.Range(0, hiddenNames.Length);
                }
                else
                {
                    j++;
                }
            }

            listOfIndex[i] = index;
            playerName[i].transform.Find("InputField_Name").GetComponent<TMP_InputField>().text = hiddenNames[index];
        }
    }

    void ConfirmSetting()
    {
        gm.ResetData();

        for(int i = 0; i < playerCount; i++)
        {
            gm.AddPlayer(i+1, stackMoney, playerName[i].transform.Find("InputField_Name").GetComponent<TMP_InputField>().text);
            if(gm.GetPlayer(i).Name == "")
            {
                panelWarning.SetActive(true);
                return;
            }    
        }

        gm.smallBlind = smallBlind;
        gm.bigBlind = bigBlind;
        gm.initStack = stackMoney;
        gm.pot = 0;

        SceneManager.LoadScene("PlayScene");
    }

    void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    void CloseWarning()
    {
        panelWarning.SetActive(false);
    }
}