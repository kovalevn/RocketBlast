using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.Netcode;
using Dynamitey.DynamicObjects;

public class UIController : MonoBehaviour
{
    private GameController gameController;
    private PlayerCrewMember player;
    private VisualElement root;
    private bool crewIsSet = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerCrewMember>();
        gameController = FindObjectOfType<GameController>();
        root = GetComponent<UIDocument>().rootVisualElement;

        Button restart = root.Q<Button>("Restart");
        restart.clicked += () => Restart();
        root.Q<Label>("Tickets").text = FindObjectOfType<PlayerCrewMember>().Tickets.ToString();

        root.Q<Label>("Hint").style.display = DisplayStyle.None;
        root.Q<GroupBox>("MainMenu").style.display = DisplayStyle.None;
        root.Q<GroupBox>("StakeChoose").style.display = DisplayStyle.None;
        root.Q<VisualElement>("StakePool").style.display = DisplayStyle.None;
        root.Q<VisualElement>("Money").style.display = DisplayStyle.None;
        root.Q<Button>("Back").style.display = DisplayStyle.None;
        root.Q<GroupBox>("EndGameStats").style.display = DisplayStyle.None;

        root.Q<Button>("Start").clicked += () => ShowChooseBet();
        root.Q<Button>("Back").clicked += () => ShowMainMenu();
        root.Q<Button>("BackToMenu").clicked += () => GoBackToMenu();
        root.Q<Button>("100").clicked += () => StartGameWithBet(100);
        root.Q<Button>("500").clicked += () => StartGameWithBet(500);
        root.Q<Button>("1000").clicked += () => StartGameWithBet(1000);
        root.Q<Button>("2000").clicked += () => StartGameWithBet(2000);

        SetCrewMembers();

        //Assign network test buttons
        //root.Q<Button>("Server").clicked += () => NetworkManager.Singleton.StartServer();
        //root.Q<Button>("Host").clicked += () => NetworkManager.Singleton.StartHost();
        //root.Q<Button>("Client").clicked += () => NetworkManager.Singleton.StartClient();
    }
    public void ShowMainMenu()
    {
        root.Q<GroupBox>("MainMenu").style.display = DisplayStyle.Flex;
        root.Q<GroupBox>("StakeChoose").style.display = DisplayStyle.None;
        root.Q<VisualElement>("Money").style.display = DisplayStyle.Flex;
        root.Q<Button>("Back").style.display = DisplayStyle.None;
        root.Q<GroupBox>("EndGameStats").style.display = DisplayStyle.None;
        root.Q<Label>("Hint").style.display = DisplayStyle.None;
        root.Q<VisualElement>("GameName").style.display = DisplayStyle.None;
        root.Q<VisualElement>("SeasonName").style.display = DisplayStyle.None;
    }
    private void ShowChooseBet()
    {
        root.Q<GroupBox>("MainMenu").style.display = DisplayStyle.None;
        root.Q<GroupBox>("StakeChoose").style.display = DisplayStyle.Flex;
        root.Q<Button>("Back").style.display = DisplayStyle.Flex;

        if (player.Tickets >= 100) root.Q<Button>("100").SetEnabled(true);
        else root.Q<Button>("100").SetEnabled(false);

        if (player.Tickets >= 500) root.Q<Button>("500").SetEnabled(true);
        else root.Q<Button>("500").SetEnabled(false);

        if (player.Tickets >= 1000) root.Q<Button>("1000").SetEnabled(true);
        else root.Q<Button>("1000").SetEnabled(false);

        if (player.Tickets >= 2000) root.Q<Button>("2000").SetEnabled(true);
        else root.Q<Button>("2000").SetEnabled(false);
    }
    private void StartGameWithBet(int bet)
    {
        if (player.Tickets >= bet)
        {
            player.Tickets -= bet;
            root.Q<GroupBox>("StakeChoose").style.display = DisplayStyle.None;
            root.Q<VisualElement>("Money").style.display = DisplayStyle.None;
            root.Q<VisualElement>("StakePool").style.display = DisplayStyle.Flex;
            root.Q<Label>("Hint").style.display = DisplayStyle.Flex;
            root.Q<Button>("Back").style.display = DisplayStyle.None;

            gameController.CurrentBet = bet;
            root.Q<Label>("StakeAmount").text = $"{gameController.CurrentBet * gameController.CrewMembers.Count}T";
            root.Q<Label>("StakeAmountStats").text = $"{gameController.CurrentBet * gameController.CrewMembers.Count}T";
            gameController.isGameStarted = true;
        }
    }
    public void ShowEndGame()
    {
        root.Q<Label>("Tickets").text = player.Tickets.ToString();
        root.Q<VisualElement>("Money").style.display = DisplayStyle.Flex;
        root.Q<VisualElement>("StakePool").style.display = DisplayStyle.None;
        root.Q<GroupBox>("EndGameStats").style.display = DisplayStyle.Flex;
        root.Q<Label>("Hint").style.display = DisplayStyle.None;
    }
    public void GoBackToMenu()
    {
        gameController.RestartGame();
        ShowMainMenu();
    }
    public void Restart()
    {
        if (FindObjectOfType<PlayerCrewMember>().Tickets >= gameController.CurrentBet)
        {
            gameController.RestartGame();
            root.Q<GroupBox>("EndGameStats").style.display = DisplayStyle.None;
            root.Q<VisualElement>("StakePool").style.display = DisplayStyle.Flex;
            root.Q<VisualElement>("Money").style.display = DisplayStyle.None;
            StartGameWithBet(gameController.CurrentBet);
        }
    }
    public void SetCrewMembers()
    {
        foreach (var member in gameController.CrewMembers)
        {
            var label = new Label(member.CrewMemberName) { name = member.CrewMemberName };
            label.style.fontSize = 55;
            root.Q<GroupBox>("CrewBox").Add(label);
        }
        crewIsSet = true;
    }

    private void FixedUpdate()
    {
        if (crewIsSet)
        {
            foreach (var member in gameController.CrewMembers)
            {
                root.Q<Label>(name: member.CrewMemberName).style.color = member.CrewMemberEjected ? Color.yellow :
                    gameController.RocketDestroyed ? Color.red : Color.green;
                root.Q<Label>(name: member.CrewMemberName).text = member.CrewMemberEjected ?
                    $"{member.CrewMemberName}: {member.EjectTime:0.000} sec {(member.PrizeMoney > 0 ? member.PrizeMoney : "???")}$" :
                    gameController.RocketDestroyed ? $"{member.CrewMemberName}: RIP" : member.CrewMemberName;
            }

            if (!gameController.RocketDestroyed && !FindObjectOfType<PlayerCrewMember>().CrewMemberEjected)
            {
                root.Q<Label>("Hint").visible = true;
            }
            else
            {
                root.Q<Label>("Hint").visible = false;
            }
        }
    }
}
