using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public partial class DialogBox : Sprite2D
{
    // Singletons
    private CustomSignals customSignals;
    private GameState gameState;
    private AudioManager audioManager;

    private Label currentTextObject = null;
    [Export] private Label textObjectPortrait;
    [Export] private Label textObjectNoPortrait;
    [Export] private AnimatedSprite2D characterPortrait;
    [Export] private AnimatedSprite2D portraitBackground;
    [Export] private Sprite2D nameSlotSmall;
    [Export] private Sprite2D nameSlotBig;
    [Export] private Label nameLabel;

    private const int noCharsForBigNameSlot = 6;

    protected AnimatedSprite2D arrowIndicator;

    private List<Dictionary<string, string>> currentDialog = null;
    private string currentDialogLine = null;
    private string currentPortrait = null;
    private int currentDialogLineID;
    private bool currentDialogLineDisplayed = false;
    private int currentLetterID;

    const float dialogMinimalSkipTime = 0.1f;

    public const float timeBetweenLetters = 0.01f;
    public const float timeAfterComa = 0.1f;
    public const float timeAfterDot = 0.3f;

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
        customSignals.Connect(CustomSignals.SignalName.StartHintDialog, new Callable(this, MethodName.StartHintDialog));
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

    public void StartDialog(string textID) // Start dialog cutscene for everything but hints
    {
        if (gameState.DialogData.ContainsKey(textID))
        {
            currentDialog = gameState.DialogData[textID];
        }
        else // in case there is no such ID
        {
            currentDialog = gameState.DialogData[""];
            currentDialog[0]["text"] = textID;
        }

        TriggerDialogSequence();
    }

    public void StartHintDialog(string worldID, string levelID, int noHint) // Start dialog cutscene for hints specifically
    {
        if (!gameState.HintsData.ContainsKey(worldID))
        {
            GD.Print("No such world exists! (hints data)");
            return;
        }

        Dictionary<string, string> dialogTemplate = gameState.DialogData[""][0];
        dialogTemplate["background"] = worldID;

        if (gameState.HintsData[worldID].ContainsKey(levelID))
        {
            dialogTemplate["text"] = gameState.HintsData[worldID][levelID][noHint - 1];
            /*int l = gameState.HintsData[worldID][levelID].Count;
            
            currentDialog = new();
            for (int i = 0; i < l; i++)
            {
                Dictionary<string, string> line = new(dialogTemplate);
                line["text"] = gameState.HintsData[worldID][levelID][i];
                currentDialog.Add(line);
            }*/
        }
        else
        {
            dialogTemplate["text"] = "This level has no hint, sorry :(";
            //currentDialog = new() { dialogTemplate };
        }
        currentDialog = new() { dialogTemplate };

        TriggerDialogSequence();
    }

    private void TriggerDialogSequence()
    {
        gameState.CanProgressDialog = false;
        arrowIndicator.Hide();

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
            currentTextObject.VisibleCharacters = -1;
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
        currentPortrait = currentDialog[currentDialogLineID]["portrait"];
        string name = currentDialog[currentDialogLineID]["name"];
        currentLetterID = 0;

        if (name == "")
        {
            nameLabel.Hide();
            nameSlotBig.Hide(); nameSlotSmall.Hide();
            currentTextObject = textObjectNoPortrait;
            textObjectNoPortrait.Show(); textObjectPortrait.Hide();
            characterPortrait.Hide(); portraitBackground.Hide();
        }
        else
        {
            if (name.Length >= noCharsForBigNameSlot)
            {
                nameSlotBig.Show(); nameSlotSmall.Hide();
            }
            else
            {
                nameSlotBig.Hide(); nameSlotSmall.Show();
            }

            nameLabel.Text = name.ToUpper();
            nameLabel.Show();
            currentTextObject = textObjectPortrait;
            textObjectPortrait.Show(); textObjectNoPortrait.Hide();
            characterPortrait.Show(); portraitBackground.Show();
            characterPortrait.Play(currentPortrait);
            portraitBackground.Play(currentDialog[currentDialogLineID]["background"]);
        }

        currentTextObject.VisibleCharacters = 0;
        currentTextObject.Text = currentDialogLine;

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

        currentLetterID++;

        if (currentLetterID > currentDialogLine.Length)
        {
            currentDialogLineDisplayed = true;
            arrowIndicator.Show();
            return;
        }

        currentTextObject.VisibleCharacters = currentLetterID; // More of text is displayed each iteration
        audioManager.PlayDialogSoundEffect(currentPortrait);

        letterTimer = timeBetweenLetters;

        char latestLetter = currentDialogLine[currentLetterID - 1];

        if (latestLetter == ',')
            letterTimer = timeAfterComa;
        if (".!?".Contains(latestLetter))
            letterTimer = timeAfterDot;
    }

    public void EndDialog()
    {
        currentDialog = null;
        currentDialogLineDisplayed = true;
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
