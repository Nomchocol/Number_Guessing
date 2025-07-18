using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI attempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;
    public TextMeshProUGUI endgameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttempts = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool GameActive;

    void InitializeUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!GameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (!int.TryParse(input, out guess))
        {
            gameState.text = "it not Number bro <color=white>;(</color>";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"YO! ENTER A NUMBER \nONLY! <color=white>{minNumber} - {maxNumber}</color> !!"; 
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";
    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string playerName = isPlayerTurn ? "<color=green>Player</color>" : "<color=red>Computer</color>";

        gameLog.text += $"{playerName} guessed: <color=white>{guess}</color>\n";

        if (guess == targetNumber)
        {
            //WIN
            endgameLog.text += $"{playerName} <color=white>WIN!</color> that right! <color=white>(^oᴥo^)</color>\n";
            EndGame();
        }
        else if (currentAttemps >= maxAttempts)
        {
            endgameLog.text += $"HaHa! you Noop! the correct number was <color=white>{targetNumber}</color> l(¯3¯)l \n";
            EndGame();
        }
        else
        {
            gameState.text = "Good!";

            //Wrong guess - give hint
            string hint = guess < targetNumber ? "<color=white>Too Low!</color>" : "<color=white>Too High!</color>";
            gameLog.text += $"{hint} nearby!\n";

            //Switch players
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "<color=green>Player Turn</color>" : "<color=red>Computer Turn</color>";
            attempsLeft.text = $"Attempts Left: <color=white>{maxAttempts - currentAttemps}</color>";

            if (!isPlayerTurn)
            {
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
            }
            else
            {
                guessInputField.interactable = true;
                submitButton.interactable = true;
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }

    IEnumerator ComputerTurn(bool targetISHigher)
    {
        yield return new WaitForSeconds(2f); // Wait to simulate thinking
        if(!GameActive) yield break;
        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }

    void EndGame()
    {
        GameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        gameState.text = "Press Click New Game \nto play again! <color=white>(//O_O//)</color>";
        Canvas.ForceUpdateCanvases();
        currentPlayer.text = "EndGame";
        attempsLeft.text = "Attempts Left: <color=white>0</color>";
    }


    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        GameActive = true;

        currentPlayer.text = "<color=green>Player Turn</color>";
        attempsLeft.text = $"Attempts Left: <color=white>{maxAttempts}</color>";
        gameLog.text = "";
        endgameLog.text = "";
        gameState.text = "New game started! \nPlayer goes first! <color=white>(>O<)</color>";

        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.text = "";
        guessInputField.Select();
        guessInputField.ActivateInputField();
    }

    void Start()
    {
        InitializeUI();
        StartNewGame();
    }

    void Update()
    {
        
    }
}