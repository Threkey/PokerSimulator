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
    int betMoney;               // 현재 플레이어의 베팅 금액
    int minBetMoney;            // 최소 베팅가능 금액 ( 이번 라운드 레이즈 금액 - 내가 이번라운드 베팅했던 금액)
    int topBetMoney;            // 이번 라운드 레이즈 금액
    int lastRaisePlayer;        // 마지막으로 레이즈한 플레이어의 인덱스 (돌아서 여기까지 오면 다음라운드)
    int checkStack;             // checkStack이 게임 참가 인원(폴드와 올인을 제외한)이 체크를 하면 다음 라운드로
    int callStack;              // callStack이 게임 참가 인원(폴드와 올인, 레이즈를 제외한)이 콜을 하면 다음 라운드로
    int 돈없는놈포문인덱스;

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


        // 카드 할당
        for (int i = 0; i < goCard.Length; i++)
            goCard[i] = GameObject.Find("Panel_LowerMiddle").transform.Find("Image_Card" + (i+1)).gameObject;

        // 오브젝트, 버튼 할당
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

        // 플레이어 세팅
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

        // 버튼
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
        // 타이머 계산
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


        // 플레이어 정보 업데이트
        if (Player != null)
            PlayerUpdate();

        // 팟 사이즈 업데이트
        GameObject.Find("Panel_LowerMiddle").transform.Find("Text_PotSize").GetComponent<TextMeshProUGUI>().text = gm.pot.ToString("#,##0") + "$";

        // 배팅 머니 업데이트
        if(goAction != null && goAction.activeSelf == true)
        {

            betMoney = (int)sliderBet.value;

            textBet.text = betMoney.ToString("#,##0");

            if(betMoney == 0)
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "체크";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 80.0f;
            }
            else if(betMoney == minBetMoney)
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "콜";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 80.0f;
            }
            else if(betMoney == gm.GetCurrentPlayer().Money)
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "올 인";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 70.0f;
            }
            else
            {
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "레이즈";
                btnBet.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 65.0f;
            }

        }

        // 폴드를 제외한 모든 인원이 콜을 했거나 체크를 했을 때
        if(checkStack == gm.playerCount - gm.PlayerStateCount(1))
        {
            RoundEnd();
        }
        if (gm.GetCurrentPlayer().Index == lastRaisePlayer)
        {
            RoundEnd();
        }

        // 이번 차례 플레이어가 폴드라면 넘어감
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
        // 데이터 저장
    }

    void GameStart()
    {
        // 돈이 bigBlind보다 적은 플레이어 검사
        for(돈없는놈포문인덱스 = 0; 돈없는놈포문인덱스 < gm.playerCount; 돈없는놈포문인덱스++)
        {
            if(gm.GetPlayer(돈없는놈포문인덱스).Money < gm.bigBlind)
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

        //프리플랍 라운드 시작
        gm.gameRound = 0;

        gm.currentPlayer = 1;

        SelectCurrentPlayer();
    }

    void ShowRecharge()
    {
        textMessage.text = "<color=blue>" + gm.GetPlayer(돈없는놈포문인덱스).Name + "</color>의\r\n돈이 부족해요!\r\n\r\n<color=red>"
            + gm.initStack + "$</color>를 지닌 채로\r\n다시 참가하거나\r\n게임에서 제외시킬 수 있어요";

        goMoneyRecharge.SetActive(true);
    }

    void RechargeMoney()
    {
        gm.GetPlayer(돈없는놈포문인덱스).Money = gm.initStack;
        goMoneyRecharge.SetActive(false);
        GameStart();
    }

    void Exclude()
    {
        // 돈 없는 Player가 현재 Player면 다음으로 넘김
        //if(gm.GetPlayer(돈없는놈포문인덱스) == gm.GetCurrentPlayer())
            //gm.NextPlayer();

        gm.RemovePlayer(돈없는놈포문인덱스);

        // 돈 없는 Player UI 비활성화하고 배열에서 삭제
        Player[돈없는놈포문인덱스].SetActive(false);
        for (int i = 돈없는놈포문인덱스; i < Player.Length - 1; i++)
        {
            Player[i] = Player[i + 1];
        }
        Array.Resize(ref Player, Player.Length - 1);

        // 승자 선택 팝업에서도 삭제
        togPlayer[돈없는놈포문인덱스].SetActive(false);
        for(int i = 돈없는놈포문인덱스; i < togPlayer.Length - 1; i++)
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

        // 1번 인덱스 찾아서 스몰 블라인드 지불
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

        // 2번 인덱스 찾아서 빅 블라인드 지불
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
        // 현재 플레이어 하이라이트
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
        // 레이즈 했으면 마지막 레이즈 플레이어 업데이트하고 checkStack 초기화
        if (betMoney != minBetMoney)
        {
            lastRaisePlayer = gm.GetCurrentPlayer().Index;
        }

        gm.GetCurrentPlayer().Money -= betMoney;
        gm.pot += betMoney;
        gm.GetCurrentPlayer().MyBet += betMoney;

        TopBetMoneyUpdate();

        // 체크나 콜을 하면 checkStack 늘어남
        if (betMoney == 0)
            checkStack++;

        // 돈 다쓰면 올인
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

        // 라운드 = 1이면 123번 카드, 2면 4번 카드, 3이면 5번카드를 뒤집는다.
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
        // 승자에게 pot머니 지급
        // 승자가 복수면 나눠서 반올림
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
            // 플레이어 순서 돌리기
            gm.GetPlayer(i).Index--;
            if(gm.GetPlayer(i).Index <= 0)
                gm.GetPlayer(i).Index = gm.playerCount;

            // 승자 선택 토글 초기화
            togPlayer[i].GetComponent<Toggle>().isOn = false;

            gm.GetPlayer(i).State = 0;
        }

        // 카드 도로 뒤집기
        for(int i = 0; i <goCard.Length; i++)
        {
            goCard[i].SetActive(false);
        }
    }

    void LoadGame()
    {
        //불러오기
    }
}