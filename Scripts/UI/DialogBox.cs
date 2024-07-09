using Godot;
using System;
using System.Collections.Generic;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public partial class DialogBox : Sprite2D
{
    // Signals
    private CustomSignals customSignals;

    [Export] private Label textObject;

    /*private readonly Dictionary<string, string> keyDict = new()
    { {"$kcJump", GlobalUtils.GetKeyName("inputJump")},
        {"$kcAbility", GlobalUtils.GetKeyName("inputAbility")},
        {"$kcRetry", GlobalUtils.GetKeyName("inputRetry")},
        {"$kcFreeView", GlobalUtils.GetKeyName("inputFreeView")} };*/

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.DialogBoxShow, new Callable(this, MethodName.UpdateDialogBox));
        customSignals.Connect(CustomSignals.SignalName.DialogBoxHide, new Callable(this, MethodName.HideDialogBox));

        HideDialogBox();
    }

    public void UpdateDialogBox(string text)
    {
        Show();

        //textObject.Text = AddKeyboardKeysToText(text);
        textObject.Text = text.Replace("\\n", "\n");
    }

    public void HideDialogBox()
    {
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
