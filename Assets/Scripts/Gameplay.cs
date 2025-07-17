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
            gameState.text = "it not Number bro ;(";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"YO! ENTER A NUMBER ONLY! {minNumber} - {maxNumber} !!"; 
            return;
        }
        else if (guess > minNumber || guess < maxNumber)
        {
            gameState.text = "Good!";
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";
    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string playerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{playerName} guessed: {guess}\n";

        if (guess == targetNumber)
        {
            //WIN
            gameLog.text += $"{playerName} got it right (^oᴥo^)\n";
            EndGame();
        }
        else if (currentAttemps >= maxAttempts)
        {
            gameLog.text += $"HaHa! you Noop! the correct number was {targetNumber}. l(¯3¯)l \n";
            EndGame();
        }
        else
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Too Low!" : "Too High!";
            gameLog.text += $"{hint} nearby!\n";

            //Switch players
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "Player" : "Computer";
            attempsLeft.text = $"Attempts Left: {maxAttempts - currentAttemps}";

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
        yield return new WaitForSeconds(1f); // Wait to simulate thinking
        if(!GameActive) yield break;
        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }

    void EndGame()
    {
        GameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Press Click New Game to play again! (//O_O//)";
        Canvas.ForceUpdateCanvases();
        currentPlayer.text = "EndGame";
        attempsLeft.text = "Attempts Left: 0";
    }


    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        GameActive = true;

        currentPlayer.text = "Player Turn";
        attempsLeft.text = $"Attempts Left: {maxAttempts}";
        gameLog.text = "";
        gameState.text = "New game started! Player goes first!(>O<)";

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