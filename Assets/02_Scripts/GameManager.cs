using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager s_instance;  //���ϼ��� ����ȴ�.
    // ������ �Ŵ����� ����´�. // ������Ƽ // �б� ����
    public static GameManager Instance { get { Init(); return s_instance; } }

    public int playerCount { get; set; } = 2;
    public int gameRound { get; set; } = 0;         // 0:�����ö�, 1:�ö�, 2:��, 3:����
    public int currentPlayer { get; set; } = 1;     // 1~10;
    public int initStack { get; set; }              // �ʱ� �����ݾ�
    public int smallBlind { get; set; }
    public int bigBlind { get; set; }
    public int pot { get; set; } = 0;

    public bool isLoaded { get; set; } = false;     // �ҷ������� �����ΰ�

    public class Player
    {
        int index = -1;         // �÷��̾� ���� ����
        int money = -1;         // �÷��̾� ���� �ݾ�
        string name;            // �÷��̾� �̸�
        int state = 0;          // 0:�÷���, 1:����, 2:����
        int myBet = 0;          // �÷��̾��� ���� �ݾ�

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
            if (go == null) // go�� ������
            {
                go = new GameObject { name = "@Manager" }; // �ڵ������ ������Ʈ�� ����� ��
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
        // �ε��� �����
        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].Index > playerList[index].Index)
                playerList[i].Index--;
        }

        playerList.RemoveAt(index);
        playerCount--;

        // ������ �÷��̾ �������ε��� �÷��̾���� 1���ε����� �ѱ�
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
        int setWidth = 1080; // ����� ���� �ʺ�
        int setHeight = 1920; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
    }

}