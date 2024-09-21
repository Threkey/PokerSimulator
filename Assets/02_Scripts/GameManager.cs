using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager s_instance;  //유일성이 보장된다.
    // 유일한 매니저를 갖고온다. // 프로퍼티 // 읽기 전용
    public static GameManager Instance { get { Init(); return s_instance; } }

    public int playerCount { get; set; } = 2;
    public int gameRound { get; set; } = 0;         // 0:프리플랍, 1:플랍, 2:턴, 3:리버
    public int currentPlayer { get; set; } = 1;     // 1~10;
    public int initStack { get; set; }              // 초기 보유금액
    public int smallBlind { get; set; }
    public int bigBlind { get; set; }
    public int pot { get; set; } = 0;

    public bool isLoaded { get; set; } = false;     // 불러오기한 게임인가

    public class Player
    {
        int index = -1;         // 플레이어 게임 순서
        int money = -1;         // 플레이어 소지 금액
        string name;            // 플레이어 이름
        int state = 0;          // 0:플레이, 1:폴드, 2:올인
        int myBet = 0;          // 플레이어의 배팅 금액

        public int Index { get { return index; } set { index = value; } }
        public int Money { get { return money; } set { money = value; } }
        public string Name{get { return name; } set { name = value; } }
        public int State { get { return state; } set { state = value; } }
        public int MyBet { get { return myBet; } set { myBet = value; } }
        public Player(int index, int money, string name)
        {
            Index = index;
            Money = money;
            Name = name;
            State = 0;
            myBet = 0;
        }

        public Player(int index, int money, string name, int state, int myBet)
        {
            Index = index;
            Money = money;
            Name = name;
            State = state;
            MyBet = myBet;
        }
    }

    List<Player> playerList = new List<Player>();


    private void Awake()
    {
        if(Screen.width / Screen.height < 1080.0f / 1920.0f)
            SetResolution();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }


    void Start()
    {
        Init();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null) // go가 없으면
            {
                go = new GameObject { name = "@Manager" }; // 코드상으로 오브젝트를 만들어 줌
                go.AddComponent<GameManager>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<GameManager>();
        }
    }

    public void AddPlayer(int index, int money, string name)
    {
        Player player = new Player(index, money, name);
        playerList.Add(player);
        playerCount++;
    }

    public void AddPlayer(int index, int money, string name, int state, int myBet)
    {
        Player player = new Player(index, money, name, state, myBet);
        playerList.Add(player);
        playerCount++;
    }

    public void RemovePlayer(int index)
    {
        // 인덱스 땡기기
        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].Index > playerList[index].Index)
                playerList[i].Index--;
        }

        playerList.RemoveAt(index);
        playerCount--;

        // 삭제된 플레이어가 마지막인덱스 플레이어였으면 1번인덱스로 넘김
        if (currentPlayer > playerList.Count)
            currentPlayer = 1;

    }

    public void ResetData()
    {
        playerList.Clear();
        playerCount = 0;
        smallBlind = 0;
        bigBlind = 0;
        gameRound = 0;
    }

    public Player GetPlayer(int index)
    {
        return playerList[index];
    }

    public List<Player> GetPlayerList()
    {
        return playerList;
    }

    public Player GetCurrentPlayer()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].Index == currentPlayer)
                return playerList[i];
        }
        return null;
    }

    public void NextPlayer()
    {
        currentPlayer++;
        if (currentPlayer > playerCount)
            currentPlayer = 1;
    }

    public int PlayerStateCount(int state)
    {
        int play = 0, fold = 0, allIn = 0;

        foreach(Player player in playerList)
        {
            if (player.State == 0)
                play++;
            else if (player.State == 1)
                fold++;
            else if (player.State == 2)
                allIn++;
            else
                return -1;
        }

        switch (state)
        {
            case 0:
                return play;
            case 1:
                return fold;
            case 2:
                return allIn;
            default:
                return -1;
        }
    }

    public void SetResolution()
    {
        int setWidth = 1080; // 사용자 설정 너비
        int setHeight = 1920; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }

}