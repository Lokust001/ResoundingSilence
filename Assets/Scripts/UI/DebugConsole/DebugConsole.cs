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
    private MapGenerator mapGenInstance;
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
        if(mapGenInstance == null)
        {
            mapGenInstance = FindAnyObjectByType<MapGenerator>();
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
            AppendConsoleLine(Command, inputColor);
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
            AppendConsoleLine(Command, inputColor);
            AppendConsoleLine($"Godmode: {godToggle}", commandCompletedColor);
            FinishCommand();
            return;
        }

        // player speed
        if (Command.StartsWith("speed"))
        {
            if (Command.Equals("speed default") || Command.Equals("speed"))
            {
                AppendConsoleLine(Command, inputColor);
                float defaultSpeed = playerInstance.defaultSpeed;
                AppendConsoleLine($"Speed set to default: {defaultSpeed}", commandCompletedColor);

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
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Speed set to: {Temp}", commandCompletedColor);
                    PlayerSpeed(Temp);
                }
                else
                {
                    AppendConsoleLine($"{Command} \nPlease put a number after the command</color>", incorrectInputColor);
                }
            }
            FinishCommand();
            return;
        }

        if(Command.StartsWith("changeisland") || Command.StartsWith("ci"))
        {
            string postCommand = "";
            if (Command.StartsWith("changeisland"))
            {
                postCommand = Command.Substring(12);
            }
            else
            {
                postCommand = Command.Substring(3);
            }
            int TempIsland;
            if (postCommand.StartsWith("current") || postCommand.StartsWith("cur"))
            {
                int closestIndex = -1;
                float closestDistance = Mathf.Infinity;
                int i = 0;
                foreach (GameObject island in mapGenInstance.islands)
                {
                    float distance = Vector3.Distance(playerInstance.gameObject.transform.position, island.transform.position);
                    if (closestDistance > distance)
                    {
                        closestIndex = i;
                        closestDistance = distance;
                    }
                    i++;
                }

                if (postCommand.StartsWith("current"))
                {
                    postCommand = postCommand.Substring(7);
                }
                else
                {
                    postCommand = postCommand.Substring(3);
                }

                if(postCommand.StartsWith(" "))
                {
                    postCommand = postCommand.Substring(1);
                }

                int Temp;
                if (postCommand.Equals("") || postCommand.Equals(" "))
                {
                    mapGenInstance.SwitchIsland(closestIndex);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {closestIndex} Has switched layouts", commandCompletedColor);
                    return;
                }
                else if (int.TryParse(Command, out Temp))
                {
                    mapGenInstance.SwitchIsland(closestIndex, Temp);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {closestIndex} Has switched to layout {Temp}", commandCompletedColor);
                    return;
                }
                else
                {
                    AppendConsoleLine($"{Command} \nPlease put a number after the command</color>", incorrectInputColor);
                    return;
                }
            }
            else if (int.TryParse(postCommand.Substring(0, 1), out TempIsland))
            {
                int Temp;
                postCommand = postCommand.Substring(1);
                if (postCommand.StartsWith(" "))
                {
                    postCommand = postCommand.Substring(1);
                }
                if (postCommand.StartsWith("") || postCommand.Equals(" "))
                {
                    mapGenInstance.SwitchIsland(TempIsland);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {TempIsland} Has switched layouts", commandCompletedColor);
                    return;
                }
                else if(int.TryParse(Command, out Temp))
                {
                     mapGenInstance.SwitchIsland(TempIsland, Temp);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {TempIsland} Has switched to layout {Temp}", commandCompletedColor);
                    return;
                }
            }
            else
            {
                AppendConsoleLine($"{Command} \nPlease put a number after the command</color>", incorrectInputColor);
            }

        }

        if(Command.StartsWith("mapreset") || Command.StartsWith("mr"))
        {
            mapGenInstance.DeleteIslands();
            AppendConsoleLine(Command, inputColor);
            AppendConsoleLine($"Map Has Been Reset", commandCompletedColor);
            mapGenInstance.spawnMap();
        }

        AppendConsoleLine(Command, incorrectInputColor);
        AppendConsoleLine($"No command found, use Help for a list of all commands.", Color.orange);
        FinishCommand();
        Debug.LogWarning("no command found found for " + Command);

    }

    private void FinishCommand()
    {
        AppendConsoleLine($"----------------------", newCommandBreakColor);
    }


    /// <summary>
    /// adds the line of text after
    /// </summary>
    /// <param name="line"></param>
    private void AppendConsoleLine(string line, Color TextColor = default)
    {
        if(TextColor == default)
        {
            TextColor = Color.white;
        }
        if (textArea.text != string.Empty)
        {
            textArea.text += $"\n";
        }
        textArea.text += $"<color=#{ColorUtility.ToHtmlStringRGB(TextColor)}>{line}</color>";
        
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
