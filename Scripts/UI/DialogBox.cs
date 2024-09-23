using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public partial class DialogBox : Sprite2D
{
    // Singletons
    private CustomSignals customSignals;
    private GameState gameState;
    private AudioManager audioManager;

    [Export] private Label textObject;

    protected AnimatedSprite2D arrowIndicator;

    private List<Dictionary<string, string>> currentDialog = null;
    private string currentDialogLine = null;
    private int currentDialogLineID;
    private bool currentDialogLineDisplayed = false;
    private int currentLetterID;

    const float dialogMinimalSkipTime = 0.1f;

    public const float timeBetweenLetters = 0.01f;
    private float letterTimer = -0.1f;

    /*private readonly Dictionary<string, string> keyDict = new()
    { {"$kcJump", GlobalUtils.GetKeyName("inputJump")},
        {"$kcAbility", GlobalUtils.GetKeyName("inputAbility")},
        {"$kcRetry", GlobalUtils.GetKeyName("inputRetry")},
        {"$kcFreeView", GlobalUtils.GetKeyName("inputFreeView")} };*/

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.StartDialog, new Callable(this, MethodName.StartDialog));
        customSignals.Connect(CustomSignals.SignalName.ProgressDialog, new Callable(this, MethodName.ProgressDialog));
        customSignals.Connect(CustomSignals.SignalName.EndDialog, new Callable(this, MethodName.EndDialog));

        arrowIndicator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        arrowIndicator.Hide();

        EndDialog();
    }

    public override void _Process(double delta)
    {
        DisplayDialog(delta);
    }

    public void StartDialog(string textID)
    {
        gameState.CanProgressDialog = false;
        arrowIndicator.Hide();

        if (gameState.DialogData.ContainsKey(textID))
        {
            currentDialog = gameState.DialogData[textID];
        }
        else // in case there is no such ID
        {
            currentDialog = gameState.DialogData[""];
            currentDialog[0]["text"] = textID;
        }

        Show();

        gameState.IsDialogActive = true;
        currentDialogLineID = 0;

        currentDialogLineDisplayed = true; // So the scroll progress doesnt trigger immediately upon starting a new dialog
        ProgressDialog();
    }

    public async void ProgressDialog()
    {
        if (currentDialog == null) return;

        if (!currentDialogLineDisplayed) // Progress the text scroll immediately
        {
            currentDialogLineDisplayed = true;
            textObject.Text = currentDialogLine;
            arrowIndicator.Show();
            return;
        }

        gameState.CanProgressDialog = false;
        arrowIndicator.Hide();

        if (currentDialog.Count <= currentDialogLineID)
        {
            EndDialog();
            return;
        }

        currentDialogLine = currentDialog[currentDialogLineID]["text"].Replace("\\n", "\n");
        currentLetterID = 0;
        textObject.Text = "";
        currentDialogLineDisplayed = false;

        currentDialogLineID++;

        await ToSignal(GetTree().CreateTimer(dialogMinimalSkipTime, processInPhysics: true), "timeout");

        gameState.CanProgressDialog = true;
    }

    private void DisplayDialog(double delta)
    {
        if (currentDialogLineDisplayed) return;
        if (currentDialogLine == null) return;

        if (letterTimer > 0.0f)
        {
            letterTimer -= (float)delta;
            return;
        }

        letterTimer = timeBetweenLetters;
        textObject.Text = currentDialogLine[..currentLetterID]; // More of text is displayed each iteration
        audioManager.textNoise.Play();
        currentLetterID++;

        if (currentLetterID > currentDialogLine.Length)
        {
            currentDialogLineDisplayed = true;
            arrowIndicator.Show();
        }
    }

    public void EndDialog()
    {
        gameState.IsDialogActive = false;

        Hide();
    }

    /*private string AddKeyboardKeysToText(string text)
    {
        bool collectingWordToReplace = false;
        List<string> wordsToReplace = new List<string>();
        string currentWordToReplace = "";
        foreach (char character in text)
        {
            if (character.Equals('$')) collectingWordToReplace = true;
            if (character.Equals(' ') && collectingWordToReplace)
            {
                collectingWordToReplace = false;
                wordsToReplace.Add(currentWordToReplace);
                currentWordToReplace = "";
            }

            if (collectingWordToReplace) currentWordToReplace += character;
        }

        foreach (var word in wordsToReplace)
        {
            text = text.Replace(word, "\"" + keyDict[word] + "\"");
        }

        return text;
    }*/
}
