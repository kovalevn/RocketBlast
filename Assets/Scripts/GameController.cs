using MetaMask;
using MetaMask.Models;
using MetaMask.Unity;
using evm.net;
using evm.net.Models;
using Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using MetaMask.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Unity.Netcode;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public float TimeFromStart;
    [HideInInspector]
    public float DestroyTime;
    [HideInInspector]
    public bool RocketDestroyed = false;
    [HideInInspector]
    public List<CrewMember> CrewMembers = new List<CrewMember>();

    public int CurrentBet = 100;
    public int NumberOfCrewMembers = 3;
    public Sprite[] NftSprites;
    public Sprite[] BodySprites;

    [HideInInspector]
    public NPCCrewMember crewMember;
    private RocketBehavior rocket;
    private UIController UIController;

    public bool isWalletAuthorized = false;
    public bool isGameStarted = false;
    MetaMaskWallet wallet;

    // Start is called before the first frame update
    void Awake()
    {
        FindRocketDestroyTime();
        CreateCrewMembers();

        CrewMembers = FindObjectsOfType<CrewMember>().ToList();
        rocket = FindObjectOfType<RocketBehavior>();
        UIController = FindObjectOfType<UIController>();

        PositionBackgroundObjects();
    }

    void Start()
    {
        MetaMaskUnity.Instance.Initialize();
        wallet = MetaMaskUnity.Instance.Wallet;
        wallet.Connect();
        wallet.WalletAuthorized += OnWalletAuthorized;
        wallet.EthereumRequestResultReceived += OnEthereumRequestResultReceived;

        //UIController.ShowMainMenu();
    }

    private void OnWalletAuthorized(object sender, EventArgs e)
    {
        isWalletAuthorized = true;
        //FindObjectOfType<PlayerCrewMember>().CrewMemberName = walletAdress;
        UIController.ShowMainMenu();
    }

    private void OnEthereumRequestResultReceived(object sender, MetaMaskEthereumRequestResultEventArgs e)
    {
        var obj = JObject.Parse(e.Result);
        var request = e.Request.Method;
        var hex = obj["result"].ToString();

        if (request == "eth_getBalance")
        {
            HexBigInteger hexBigInteger = new HexBigInteger(hex);
            System.Numerics.BigInteger bigInteger = hexBigInteger.Value;
            decimal ether = UnitConversion.Convert.FromWei(bigInteger);

            Debug.Log(ether);
        }
    }

    private async void GetBalance()
    {
        var transactionParams = new MetaMaskTransaction
        {
            To = "0xd0059fB234f15dFA9371a7B45c09d451a2dd2B5a",
            From = wallet.SelectedAddress,
            Value = "0x0"
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_getBalance",
            Parameters = new object[] { wallet.SelectedAddress, "latest" }
            //Parameters = new MetaMaskTransaction[] { transactionParams }
        };
        await wallet.Request(request);     
    }

    private async void UseContract()
    {
        var address = "0xB26bCaB062580C67C194FC25fB2F170bd1E6F3d8";

        var contract = Contract.Attach<Contract3.Contract3>(wallet, address);
        var result = await contract.PlayerTickets(wallet.SelectedAddress);
        Debug.Log(result);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && isWalletAuthorized) GetBalance();

        if (!RocketDestroyed && isGameStarted)
        {
            TimeFromStart += Time.deltaTime;
            //Debug.Log(TimeFromStart);

            var nonejectedNPCMembers = CrewMembers.Where(x => x.CrewMemberEjected == false && x is NPCCrewMember);

            if (nonejectedNPCMembers.Count() == 1)
            {
                (nonejectedNPCMembers.First() as NPCCrewMember).ChangeEjectTime();
            }

            if (TimeFromStart >= DestroyTime)
            {
                RocketDestroyed = true;
                rocket.StopRocket();
                CalculateWinAmounts();
                UIController.ShowEndGame();
            }
        }
    }

    private void FindRocketDestroyTime()
    {
        DestroyTime = Random.Range(2f, 10f);
    }
    public void RestartGame()
    {
        isGameStarted = false;
        FindRocketDestroyTime();
        TimeFromStart = 0;
        RocketDestroyed = false;
        rocket.RestoreRocket();
        foreach(var member in CrewMembers) { member.RestoreCrewMember(); }
    }

    private void CreateCrewMembers()
    {
        var crewMemberYPosition = -1.4f;
        var crewNames = Enum.GetNames(typeof(CrewNames)).ToList();
        for (int i = 0; i < NumberOfCrewMembers; i++)
        {
            var member = Instantiate(crewMember, new Vector3(0, crewMemberYPosition, 0), Quaternion.identity);
            var randNameNumber = Random.Range(0, crewNames.Count);
            member.CrewMemberName = crewNames.ElementAt(randNameNumber);
            crewNames.RemoveAt(randNameNumber);
            member.transform.SetParent(FindObjectOfType<RocketBehavior>().transform);
            crewMemberYPosition += 1.2f;
        }
    }

    private void PositionBackgroundObjects()
    {
        var backgroundObjects = FindObjectsOfType<BackgroundObjMove>().ToList();
        foreach (var obj in backgroundObjects)
        {
            obj.transform.position = new Vector3(Random.Range(-8f, 8f), Random.Range(-5f, 5f), 0);
        }
    }

    private void CalculateWinAmounts()
    {
        int prizePool = CurrentBet * CrewMembers.Count;
        var WinningCrewMembers = CrewMembers.Where(x => x.CrewMemberEjected).OrderByDescending(x => x.EjectTime).ToList();

        if (WinningCrewMembers.Count == 1) WinningCrewMembers.First().PrizeMoney = prizePool;
        else if (WinningCrewMembers.Count == 2)
        {
            WinningCrewMembers.ElementAt(0).PrizeMoney = prizePool * 0.7f;
            WinningCrewMembers.ElementAt(1).PrizeMoney = prizePool * 0.3f;
        }
        else if (WinningCrewMembers.Count == 3)
        {
            WinningCrewMembers.ElementAt(0).PrizeMoney = prizePool * 0.6f;
            WinningCrewMembers.ElementAt(1).PrizeMoney = prizePool * 0.4f;
            WinningCrewMembers.ElementAt(2).PrizeMoney = prizePool * 0.2f;
        }
        else if (WinningCrewMembers.Count == 4)
        {
            WinningCrewMembers.ElementAt(0).PrizeMoney = prizePool * 0.4f;
            WinningCrewMembers.ElementAt(1).PrizeMoney = prizePool * 0.3f;
            WinningCrewMembers.ElementAt(2).PrizeMoney = prizePool * 0.2f;
            WinningCrewMembers.ElementAt(3).PrizeMoney = prizePool * 0.1f;
        }
        else if (WinningCrewMembers.Any())
        {
            WinningCrewMembers.ElementAt(0).PrizeMoney = prizePool * 0.4f;
            WinningCrewMembers.ElementAt(1).PrizeMoney = prizePool * 0.3f;
            WinningCrewMembers.ElementAt(2).PrizeMoney = prizePool * 0.2f;
            for (int i = 3; i < WinningCrewMembers.Count; i++)
            {
                WinningCrewMembers.ElementAt(i).PrizeMoney = prizePool * 0.1f / (WinningCrewMembers.Count - 3);
            }
        }
        foreach (var member in WinningCrewMembers) { member.Tickets += (int)member.PrizeMoney; }
    }
}

enum CrewNames
{
    Frank,
    Joe,
    Nick,
    Billy,
    Van,
    Jonathan,
    Joseph,
    Jotaro,
    Vladimir,
    V,
    Geralt,
    Tony 
}
