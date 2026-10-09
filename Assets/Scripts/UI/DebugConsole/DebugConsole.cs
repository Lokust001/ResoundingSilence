/*
* Author: Brenden
* Contributors:
* Last Modified: 10/01/2026
* Summary: The debug console and all the code that comes with it
* To Do:   The Debug console SpreadSheet
*/
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    private enum ShownSettings
    {
        None,
        Refs,
        Colors
    }

    [SerializeField]
    private ShownSettings settings;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Refs)]
    TMPro.TMP_InputField inputs;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Refs)]
    TMPro.TMP_Text textArea;

    [ShowIf(nameof(settings), ShownSettings.Refs)]
    [SerializeField] 
    ScrollRect logScrollRect;

    
    //private GameObject[] enemies;

    /*[ShowIf(nameof(settings), ShownSettings.Refs]
    [SerializeField] 
    GameObject freeCamPrefab;
    public GameObject FreeCamInstance;
    [SerializeField] GameObject detachCamPrefab;
    public GameObject DetachCamInstance;*/

    //private bool noClipToggle = false;
    private bool godToggle = false;
    /*private bool cameraToggle = false;
    private bool freezeToggle = false;*/

    private PlayerController playerInstance;
    private GameObject cameraInstance;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Colors)]
    private Color inputColor;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Colors)]
    private Color incorrectInputColor;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Colors)]
    private Color commandCompletedColor;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Colors)]
    private Color newCommandBreakColor;



    /// <summary>
    /// called when the debug console opens up
    /// </summary>
    public void OpenDebugConsole()
    {
        inputs.ActivateInputField();
        if (playerInstance == null)
        {
            playerInstance = FindAnyObjectByType<PlayerController>();
        }
        ClearConsole();
    }

    /// <summary>
    /// runs through with what was inputted into the text field and runs the command that was typed
    /// </summary>
    public void CallFunction()
    {
        string Command = inputs.text.ToLower();

        inputs.text = "";
        inputs.ActivateInputField();

        if (Command.Length == 0)
        {
            Debug.LogWarning("Empty debug command");
            return;
        }

        if (Command == "help")
        {
            AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(inputColor)}>{Command}</color>");
            AppendConsoleLine("God Mode: god");
            AppendConsoleLine($"Set Player's Speed to Default ({playerInstance.defaultSpeed}): speed");
            AppendConsoleLine($"Change Players Speed: speed <Speed Value>");
            FinishCommand();
            return;
        }


        // God Mode
        if (Command == "god")
        {
            GodMode();
            AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(inputColor)}>{Command}</color>");
            AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(commandCompletedColor)}>Godmode: {godToggle}</color>");
            FinishCommand();
            return;
        }

        // player speed
        if (Command.StartsWith("speed"))
        {
            if (Command.Equals("speed default") || Command.Equals("speed"))
            {
                AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(inputColor)}>{Command}</color>");
                float defaultSpeed = playerInstance.defaultSpeed;
                AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(commandCompletedColor)}>Speed set to default: {defaultSpeed}</color>");

                if (Command.Equals("speed"))
                {
                    AppendConsoleLine("You can also set the player's speed to a specific number by typing 'speed <x>'");
                }

                PlayerSpeed(defaultSpeed);
                FinishCommand();
                return;
            }

            if (Command.Length >= 6)
            {
                int Temp;
                if (int.TryParse(Command.Substring(6, Command.Length - 6), out Temp))
                {
                    AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(inputColor)}>{Command}</color>");
                    AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(commandCompletedColor)}>Speed set to: {Temp}</color>");
                    PlayerSpeed(Temp);
                }
                else
                {
                    AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(incorrectInputColor)}>{Command}\nPlease put a number after the command</color>");
                }
            }
            FinishCommand();
            return;
        }

        AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(incorrectInputColor)}>{Command}" +
            $"</color>\n<color=orange>No command found, use Help for a list of all commands.</color>");
        FinishCommand();
        Debug.LogWarning("no command found found for " + Command);

    }

    private void FinishCommand()
    {
        AppendConsoleLine($"<color=#{ColorUtility.ToHtmlStringRGB(newCommandBreakColor)}>----------------------</color>");
    }


    /// <summary>
    /// adds the line of text after
    /// </summary>
    /// <param name="line"></param>
    private void AppendConsoleLine(string line)
    {
        if (textArea.text != string.Empty)
        {
            textArea.text += $"\n";
        }
        textArea.text += line;
        
        StartCoroutine(ScrollToBottomNextFrame());
    }

    /// <summary>
    /// used later if we want to make the debug console scrollable
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        logScrollRect.verticalNormalizedPosition = 0;
/*        if (logScrollRect == null)
            yield break;

        Canvas.ForceUpdateCanvases();*/

        
    }

    /// <summary>
    /// changes the player's speed
    /// </summary>
    /// <param name="Speed"></param>
    private void PlayerSpeed(float Speed)
    { 
        playerInstance.moveSpeed = Speed;
    }

    /// <summary>
    /// toggles weather the player can take damage or not
    /// </summary>
    private void GodMode()
    {
        godToggle = !godToggle;
        playerInstance.InGodMode = godToggle;
    }

    /// <summary>
    /// clears the text box
    /// </summary>
    private void ClearConsole()
    {
        textArea.text = "";
    }
}
