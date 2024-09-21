using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    GameManager gm;

    [Header("Canvas")]
    public Transform canvas;

    [Header("Top")]
    public Button btnBackToTitle;
    public Button btnHandRankings;
    public Button btnSave;
    public TextMeshProUGUI textTimer;

    [Header("UpperMiddle")]
    GameObject[] Player;

    [Header("LowerMiddle")]
    public TextMeshProUGUI textPotSize;
    GameObject[] goCard = new GameObject[5];

    [Header("Bottom")]
    public Transform panelBottom;
    Button btnGameStart;
    Button btnSmallBlind;
    Button btnBigBlind;
    Button btnNextRound;
    Button btnShowDown;
    GameObject goAction;
    Button btnFold;
    Button btnBet;
    Button btnPlus;
    Button btnMinus;
    Slider sliderBet;
    TextMeshProUGUI textBet;

    [Header("Popups")]
    GameObject goBackToTitle;
    Button btnNo;
    Button btnYes;
    GameObject goHandRankings;
    Button btnCloseHandRankings;
    GameObject goSave;
    Button btnSaveSave;
    Button btnCancelSave;
    GameObject goShowDown;
    Button btnConfirmWinner;
    GameObject textWarning;
    GameObject[] togPlayer;
    GameObject goMoneyRecharge;
    Button btnRecharge;
    Button btnExclude;
    TextMeshProUGUI textMessage;

    float sec;
    int min, hour;
    int betMoney;               // ���� �÷��̾��� ���� �ݾ�
    int minBetMoney;            // �ּ� ���ð��� �ݾ� ( �̹� ���� ������ �ݾ� - ���� �̹����� �����ߴ� �ݾ�)
    int topBetMoney;            // �̹� ���� ������ �ݾ�
    int lastRaisePlayer;        // ���������� �������� �÷��̾��� �ε��� (���Ƽ� ������� ���� ��������)
    int checkStack;             // checkStack�� ���� ���� �ο�(����� ������ ������)�� üũ�� �ϸ� ���� �����
    int callStack;              // callStack�� ���� ���� �ο�(����� ����, ����� ������)�� ���� �ϸ� ���� �����
    int �����³������ε���;

    Color colorHighlight = new Color(0.32f, 0.85f, 1.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;

        if (!gm.isLoaded)
        {
            sec = 0.0f;
            min = 0;
            hour = 0;
            betMoney = 0;
            minBetMoney = 0;
            topBetMoney = 0;
            lastRaisePlayer = 0;
            gm.gameRound = 0;
            gm.currentPlayer = 1;
        }
        else if (gm.isLoaded)
        {
            LoadGame();
        }


        // ī�� �Ҵ�
        for (int i = 0; i < goCard.Length; i++)
            goCard[i] = GameObject.Find("Panel_LowerMiddle").transform.Find("Image_Card" + (i+1)).gameObject;

        // ������Ʈ, ��ư �Ҵ�
        btnGameStart = panelBottom.Find("Button_GameStart").GetComponent<Button>();
        btnSmallBlind = panelBottom.Find("Button_SmallBlind").GetComponent<Button>();
        btnSmallBlind.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "SB (" + gm.smallBlind + "$)";
        btnBigBlind = panelBottom.Find("Button_BigBlind").GetComponent<Button>();
        btnBigBlind.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "BB (" + gm.bigBlind + "$)";
        btnNextRound = panelBottom.Find("Button_NextRound").GetComponent<Button>();
        btnShowDown = panelBottom.Find("Button_ShowDown").GetComponent <Button>();
        goAction = panelBottom.Find("Parent_Action").gameObject;
        btnFold = goAction.transform.Find("Button_Fold").GetComponent<Button>();
        btnBet = goAction.transform.Find("Button_Bet").GetComponent<Button>();
        btnPlus = goAction.transform.Find("Button_Plus").GetComponent<Button>();
        btnMinus = goAction.transform.Find("Button_Minus").GetComponent<Button>();
        sliderBet = goAction.transform.Find("Slider_Bet").GetComponent<Slider>();
        textBet = goAction.transform.Find("Text_Bet").GetComponent<TextMeshProUGUI>();
        goBackToTitle = canvas.Find("Panel_BackToTitle").gameObject;
        goHandRankings = canvas.Find("Panel_HandRankings").gameObject;
        goSave = canvas.Find("Panel_Save").gameObject;
        goShowDown = canvas.Find("Panel_ShowDown").gameObject;
        btnYes = goBackToTitle.transform.Find("Panel_Window").Find("Button_Yes").GetComponent<Button>();
        btnNo = goBackToTitle.transform.Find("Panel_Window").Find("Button_No").GetComponent<Button>();
        btnCloseHandRankings = goHandRankings.transform.Find("Button_CloseHandRankings").GetComponent<Button>();
        btnSaveSave = goSave.transform.Find("Panel").Find("Button_Save").GetComponent<Button>();
        btnCancelSave = goSave.transform.Find("Panel").Find("Button_Cancel").GetComponent<Button>();
        btnConfirmWinner = goShowDown.transform.Find("Panel").Find("Button_ConfirmWinner").GetComponent<Button>();
        textWarning = goShowDown.transform.Find("Panel").Find("Text_Warning").gameObject;
        goMoneyRecharge = canvas.Find("Panel_MoneyRecharge").gameObject;
        btnRecharge = goMoneyRecharge.transform.Find("Panel").Find("Button_Recharge").GetComponent<Button>();
        btnExclude = goMoneyRecharge.transform.Find("Panel").Find("Button_Exclude").GetComponent<Button>();
        textMessage = goMoneyRecharge.transform.Find("Panel").Find("Text_Message").GetComponent<TextMeshProUGUI>();

        // �÷��̾� ����
        Player = new GameObject[gm.playerCount];
        togPlayer = new GameObject[gm.playerCount];
        for(int i = 0; i < Player.Length; i++)
        {
            Player[i] = canvas.Find("Panel_UpperMiddle").Find("Image_Player (" + i + ")").gameObject;
            Player[i].SetActive(true);
            Player[i].transform.Find("Text_Index").GetComponent<TextMeshProUGUI>().text = gm.GetPlayer(i).Index.ToString();
            Player[i].transform.Find("Text_Name").GetComponent<TextMeshProUGUI>().text = gm.GetPlayer(i).Name;
            Player[i].transform.Find("Text_Stack").GetComponent<TextMeshProUGUI>().text = gm.GetPlayer(i).Money.ToString("#,##0") + "$";

            togPlayer[i] = canvas.Find("Panel_ShowDown").Find("Panel").Find("Scroll View").Find("Viewport").Find("Content").Find("Toggle_Player (" + i + ")").gameObject;
            togPlayer[i].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = gm.GetPlayer(i).Name;
            togPlayer[i].gameObject.SetActive(true);
        }

        // ��ư
        btnBackToTitle.onClick.AddListener(ShowBackToTitle);
        btnHandRankings.onClick.AddListener(ShowHandRankings);
        btnSave.onClick.AddListener(SaveData);
        btnGameStart.onClick.AddListener(GameStart);
        btnSmallBlind.onClick.AddListener(SmallBlind);
        btnBigBlind.onClick.AddListener(BigBlind);
        btnPlus.onClick.AddListener(BetMoneyPlus);
        btnMinus.onClick.AddListener(BetMoneyMinus);
        btnFold.onClick.AddListener(Fold);
        btnBet.onClick.AddListener(Bet);
        btnNextRound.onClick.AddListener(NextRound);
        btnShowDown.onClick.AddListener(ChooseWinner);
        btnNo.onClick.AddListener(CloseBackToTitle);
        btnYes.onClick.AddListener(LoadTitleScene);
        btnCloseHandRankings.onClick.AddListener(CloseHandRankings);
        btnCancelSave.onClick.AddListener(CloseSaveData);
        btnSaveSave.onClick.AddListener(SaveSaveData);
        btnConfirmWinner.onClick.AddListener(ConfirmWinner);
        btnRecharge.onClick.AddListener(RechargeMoney);
        btnExclude.onClick.AddListener(Exclude);

    }

    // Update is called once per frame
    void Update()
    {
        // Ÿ�̸� ���
        sec += Time.deltaTime;
        if(sec >= 60.0f)
        {
            sec = 0.0f;
            min++;
        }
        if(min >= 60)
        {
            min = 0;
            hour++;
        }

        if(textTimer != null)
            textTimer.text = System.String.Format("{0:00}",hour) + ":" + System.String.Format("{0:00}", min);


        // �÷��̾� ���� ������Ʈ
        if (Player != null)
            PlayerUpdate();

        // �� ������ ������Ʈ
        GameObject.Find("Panel_LowerMiddle").transform.Find("Text_PotSize").GetComponent<TextMeshProUGUI>().text = gm.pot.ToString("#,##0") + "$";

        // ���� �Ӵ� ������Ʈ
        if(goAction != null && goAction.activeSelf == true)
        {

            betMoney = (int)sliderBet.value;

            textBet.text = betMoney.ToString("#,##0");

            if(betMoney == 0)
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "üũ";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 80.0f;
            }
            else if(betMoney == minBetMoney)
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "��";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 80.0f;
            }
            else if(betMoney == gm.GetCurrentPlayer().Money)
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "�� ��";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 70.0f;
            }
            else
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "������";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 65.0f;
            }

        }

        // ���带 ������ ��� �ο��� ���� �߰ų� üũ�� ���� ��
        if(checkStack == gm.playerCount - gm.PlayerStateCount(1))
        {
            RoundEnd();
        }
        if (gm.GetCurrentPlayer().Index == lastRaisePlayer)
        {
            RoundEnd();
        }

        // �̹� ���� �÷��̾ ������ �Ѿ
        if(gm.GetCurrentPlayer().State == 1)
        {
            gm.NextPlayer();
            SelectCurrentPlayer();
        }


    }

    void PlayerUpdate()
    {
        for (int i = 0; i < Player.Length; i++)
        {
            Player[i].transform.Find("Text_Index").GetComponent<TextMeshProUGUI>().text = gm.GetPlayer(i).Index.ToString();
            Player[i].transform.Find("Text_Name").GetComponent<TextMeshProUGUI>().text = gm.GetPlayer(i).Name;
            Player[i].transform.Find("Text_Stack").GetComponent<TextMeshProUGUI>().text = gm.GetPlayer(i).Money.ToString("#,##0") + "$";

            if(gm.GetPlayer(i).State == 1)
            {
                Player[i].transform.Find("Panel_Fold").gameObject.SetActive(true);
            }
            else
                Player[i].transform.Find("Panel_Fold").gameObject.SetActive(false);
        }
    }

    void ShowBackToTitle()
    {
        goBackToTitle.SetActive(true);
    }

    void CloseBackToTitle()
    {
        goBackToTitle.SetActive(false);
    }

    void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    void ShowHandRankings()
    {
        goHandRankings.SetActive(true);
    }

    void CloseHandRankings()
    {
        goHandRankings.SetActive(false);
    }

    void SaveData()
    {
        goSave.SetActive(true);
    }

    void CloseSaveData()
    {
        goSave.SetActive(false);
    }

    void SaveSaveData()
    {
        // ������ ����
    }

    void GameStart()
    {
        // ���� bigBlind���� ���� �÷��̾� �˻�
        for(�����³������ε��� = 0; �����³������ε��� < gm.playerCount; �����³������ε���++)
        {
            if(gm.GetPlayer(�����³������ε���).Money < gm.bigBlind)
            {
                ShowRecharge();
                return;
            }
        }

        btnGameStart.gameObject.SetActive(false);


        //btnSmallBlind.gameObject.SetActive(true);
        minBetMoney = gm.smallBlind;
        goAction.SetActive(true);

        btnSave.interactable = false;

        //�����ö� ���� ����
        gm.gameRound = 0;

        gm.currentPlayer = 1;

        SelectCurrentPlayer();
    }

    void ShowRecharge()
    {
        textMessage.text = "<color=blue>" + gm.GetPlayer(�����³������ε���).Name + "</color>��\r\n���� �����ؿ�!\r\n\r\n<color=red>"
            + gm.initStack + "$</color>�� ���� ä��\r\n�ٽ� �����ϰų�\r\n���ӿ��� ���ܽ�ų �� �־��";

        goMoneyRecharge.SetActive(true);
    }

    void RechargeMoney()
    {
        gm.GetPlayer(�����³������ε���).Money = gm.initStack;
        goMoneyRecharge.SetActive(false);
        GameStart();
    }

    void Exclude()
    {
        // �� ���� Player�� ���� Player�� �������� �ѱ�
        //if(gm.GetPlayer(�����³������ε���) == gm.GetCurrentPlayer())
            //gm.NextPlayer();

        gm.RemovePlayer(�����³������ε���);

        // �� ���� Player UI ��Ȱ��ȭ�ϰ� �迭���� ����
        Player[�����³������ε���].SetActive(false);
        for (int i = �����³������ε���; i < Player.Length - 1; i++)
        {
            Player[i] = Player[i + 1];
        }
        Array.Resize(ref Player, Player.Length - 1);

        // ���� ���� �˾������� ����
        togPlayer[�����³������ε���].SetActive(false);
        for(int i = �����³������ε���; i < togPlayer.Length - 1; i++)
        {
            togPlayer[i] = togPlayer[i + 1];
        }
        Array.Resize(ref togPlayer, togPlayer.Length - 1);

        goMoneyRecharge.SetActive(false);
        GameStart();
    }

    void SmallBlind()
    {
        betMoney = gm.smallBlind;

        // 1�� �ε��� ã�Ƽ� ���� ����ε� ����
        for(int i = 0; i < gm.GetPlayerList().Count; i++)
        {
            if(gm.GetPlayer(i).Index == 1)
            {
                gm.GetPlayer(i).Money -= betMoney;
                gm.pot += betMoney;
                gm.GetCurrentPlayer().MyBet = betMoney;
                minBetMoney = betMoney;
                lastRaisePlayer = gm.GetPlayer(i).Index;
            }
        }

        TopBetMoneyUpdate();

        gm.NextPlayer();
        minBetMoney = topBetMoney - gm.GetCurrentPlayer().MyBet;

        btnSmallBlind.gameObject.SetActive(false);
        btnBigBlind.gameObject.SetActive(true);
        SelectCurrentPlayer();
    }

    void BigBlind()
    {
        betMoney = gm.bigBlind;

        // 2�� �ε��� ã�Ƽ� �� ����ε� ����
        for (int i = 0; i < gm.GetPlayerList().Count; i++)
        {
            if (gm.GetPlayer(i).Index == 2)
            {
                gm.GetPlayer(i).Money -= betMoney;
                gm.pot += betMoney;
                gm.GetCurrentPlayer().MyBet = betMoney;
                minBetMoney = betMoney;
                lastRaisePlayer = gm.GetPlayer(i).Index;
            }
        }

        TopBetMoneyUpdate();

        gm.NextPlayer();
        minBetMoney = topBetMoney - gm.GetCurrentPlayer().MyBet;

        btnBigBlind.gameObject.SetActive(false);
        goAction.SetActive(true);
        SelectCurrentPlayer();
    }

    void SelectCurrentPlayer()
    {
        // ���� �÷��̾� ���̶���Ʈ
        for (int i = 0; i < Player.Length; i++)
        {
            Player[i].GetComponent<Image>().color = Color.white;
            if (gm.GetPlayer(i).Index == gm.currentPlayer)
                Player[i].GetComponent<Image>().color = colorHighlight;
        }

        sliderBet.minValue = minBetMoney;
        if (gm.GetCurrentPlayer().Money < minBetMoney)
            sliderBet.minValue = gm.GetCurrentPlayer().Money;
        sliderBet.maxValue = gm.GetCurrentPlayer().Money;
        sliderBet.value = sliderBet.minValue;
    }

    void UnSelectAllPlayer()
    {
        for (int i = 0; i < Player.Length; i++)
        {
            Player[i].GetComponent<Image>().color = Color.white;
        }
    }

    void BetMoneyPlus()
    {
        sliderBet.value++;
    }

    void BetMoneyMinus()
    {
        sliderBet.value--;
    }

    void Fold()
    {
        gm.GetCurrentPlayer().State = 1;
        gm.NextPlayer();
        SelectCurrentPlayer();
    }

    void Bet()
    {
        // ������ ������ ������ ������ �÷��̾� ������Ʈ�ϰ� checkStack �ʱ�ȭ
        if (betMoney != minBetMoney)
        {
            lastRaisePlayer = gm.GetCurrentPlayer().Index;
        }

        gm.GetCurrentPlayer().Money -= betMoney;
        gm.pot += betMoney;
        gm.GetCurrentPlayer().MyBet += betMoney;

        TopBetMoneyUpdate();

        // üũ�� ���� �ϸ� checkStack �þ
        if (betMoney == 0)
            checkStack++;

        // �� �پ��� ����
        if (gm.GetCurrentPlayer().Money >= 0)
            gm.GetCurrentPlayer().State = 2;

        gm.NextPlayer();

        if(gm.gameRound == 0 && gm.GetCurrentPlayer().Index == 2)
        {
            if(topBetMoney < gm.bigBlind)
                minBetMoney = gm.bigBlind;
            else
                minBetMoney = topBetMoney - gm.GetCurrentPlayer().MyBet;
        }
        else
            minBetMoney = topBetMoney - gm.GetCurrentPlayer().MyBet;

        SelectCurrentPlayer();
    }

    void RoundEnd()
    {
        goAction.SetActive(false);

        if(gm.gameRound == 3)
            btnShowDown.gameObject.SetActive(true);
        else
            btnNextRound.gameObject.SetActive(true);

        UnSelectAllPlayer();

        topBetMoney = 0;
        minBetMoney = 0;
        betMoney = 0;
        lastRaisePlayer = 0;
        checkStack = 0;

        for (int i = 0; i < gm.GetPlayerList().Count; i++)
        {
            gm.GetPlayer(i).MyBet = 0;
        }
    }

    void NextRound()
    {
        gm.gameRound++;
        gm.currentPlayer = 1;

        btnNextRound.gameObject.SetActive(false);
        goAction.SetActive(true);
        SelectCurrentPlayer();

        // ���� = 1�̸� 123�� ī��, 2�� 4�� ī��, 3�̸� 5��ī�带 �����´�.
        if(gm.gameRound == 1)
        {
            for(int i = 0; i < 3; i++)
                goCard[i].SetActive(true);
        }
        else if(gm.gameRound == 2)
            goCard[3].SetActive(true);
        else if(gm.gameRound == 3)
            goCard[4].SetActive(true);
    }

    void ChooseWinner()
    {
        goShowDown.SetActive(true);
    }

    void ConfirmWinner()
    {
        // ���ڿ��� pot�Ӵ� ����
        // ���ڰ� ������ ������ �ݿø�
        int winnerCount = 0;
        int prizeMoney = 0;

        for(int i = 0; i <gm.playerCount; i++)
        {
            if (togPlayer[i].GetComponent<Toggle>().isOn)
                winnerCount++;
        }

        if(winnerCount == 0)
        {
            textWarning.SetActive(true);
            return;
        }
        else
        {
            textWarning.SetActive(false);

            prizeMoney = (int)(Mathf.Round((float)gm.pot / (float)winnerCount));

            for (int i = 0; i < gm.playerCount; i++)
            {
                if (togPlayer[i].GetComponent<Toggle>().isOn)
                    gm.GetPlayer(i).Money += prizeMoney;
            }

            gm.pot = 0;

            GameEnd();

            goShowDown.SetActive(false);
            btnShowDown.gameObject.SetActive(false);
            btnGameStart.gameObject.SetActive(true);
        }
    }

    void TopBetMoneyUpdate()
    {
        if(topBetMoney < gm.GetCurrentPlayer().MyBet)
            topBetMoney = gm.GetCurrentPlayer().MyBet;
    }

    void GameEnd()
    {
        for (int i = 0; i < gm.playerCount; i++)
        {
            // �÷��̾� ���� ������
            gm.GetPlayer(i).Index--;
            if(gm.GetPlayer(i).Index <= 0)
                gm.GetPlayer(i).Index = gm.playerCount;

            // ���� ���� ��� �ʱ�ȭ
            togPlayer[i].GetComponent<Toggle>().isOn = false;

            gm.GetPlayer(i).State = 0;
        }

        // ī�� ���� ������
        for(int i = 0; i <goCard.Length; i++)
        {
            goCard[i].SetActive(false);
        }
    }

    void LoadGame()
    {
        //�ҷ�����
    }
}