/*
* Author: Brenden
* Contributors:
* Last Modified: 10/26/2026
* Summary: The debug console and all the code that comes with it
* To Do:   The Debug console SpreadSheet
*/
using NaughtyAttributes;
using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Linq;
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
    private bool pivotToggle = false;
    private bool detachToggle = false;
    private bool freeToggle = false;
    //private bool freezeToggle = false;

    private PlayerController playerInstance;
    private MapGenerator mapGenInstance;
    private GameObject cameraInstance;
    private PivotCamera PivotInstance;
    private DetachCamera DetachInstance;
    private FreeCamera FreeCameraInstance;

    [SerializeField] FreeCamera freecamPrefab;

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
        if (mapGenInstance == null)
        {
            mapGenInstance = FindAnyObjectByType<MapGenerator>();
        }
        if (PivotInstance == null)
        {
            PivotInstance = FindAnyObjectByType<PivotCamera>();
        }
        if (DetachInstance == null)
        {
            DetachInstance = FindAnyObjectByType<DetachCamera>();
        }
        if (FreeCameraInstance == null)
        {
            FreeCameraInstance = Instantiate(freecamPrefab, DetachInstance.transform.position, DetachInstance.transform.rotation, playerInstance.transform);
            FreeCameraInstance.gameObject.SetActive(false);
        }
        playerInstance.StopPlayerMovementAndInput();
        ClearConsole();
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// When you close the menu it restarts
    /// </summary>
    private void OnDisable()
    {
        if(!pivotToggle && !detachToggle && !freeToggle)
        {
            playerInstance.RestartPlayerMovementAndInput();
        }
    }


    /// <summary>
    /// runs through with what was inputted into the text field and runs the command that was typed
    /// </summary>
    public void CallFunction()
    {
        string Command = inputs.text.ToLower();
        Command = Command.Trim();
        AppendConsoleLine(Command);
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
            AppendConsoleLine($"Change Map Section: ChangeIsland <island index (or current)> <layout index>");
            AppendConsoleLine($"Reset Map: MapReset");
            AppendConsoleLine($"Change to Pivot Camera Mode: PivotCam or PC");
            AppendConsoleLine($"Change to Free Camera Mode: FreeCam or FC");
            AppendConsoleLine($"Change to Detatch Camera Mode: DetachCam or DC");
            AppendConsoleLine($"Add a specific pin to inventory: AddPin or AP <index of pin (int) or name of pin (string).");
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
                postCommand = Command.Substring(2);
            }
            if (postCommand.StartsWith(" "))
            {
                postCommand = postCommand.Substring(1);
            }
            int TempIsland;
            if (postCommand == null || postCommand.Length == 0 || postCommand.StartsWith("current") || postCommand.StartsWith("cur"))
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

                if (postCommand == null || postCommand.Length == 0)
                {
                    mapGenInstance.SwitchIsland(closestIndex);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {closestIndex} Has switched layouts", commandCompletedColor);
                    FinishCommand();
                    return;
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
                if (postCommand == null || postCommand.Equals("") || postCommand.Equals(" "))
                {
                    mapGenInstance.SwitchIsland(closestIndex);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {closestIndex} Has switched layouts", commandCompletedColor);
                    FinishCommand();
                    return;
                }
                else if (int.TryParse(Command, out Temp))
                {
                    mapGenInstance.SwitchIsland(closestIndex, Temp);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {closestIndex} Has switched to layout {Temp}", commandCompletedColor);
                    FinishCommand();
                    return;
                }
                else
                {
                    AppendConsoleLine($"{Command} \nPlease put a number after the command</color>", incorrectInputColor);
                    FinishCommand();
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
                    FinishCommand();
                    return;
                }
                else if(int.TryParse(Command, out Temp))
                {
                     mapGenInstance.SwitchIsland(TempIsland, Temp);
                    AppendConsoleLine(Command, inputColor);
                    AppendConsoleLine($"Island {TempIsland} Has switched to layout {Temp}", commandCompletedColor);
                    FinishCommand();
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
            FinishCommand();
            return;
        }

        if(Command.StartsWith("pivotcam") || Command.StartsWith("pc"))
        {
            pivotToggle = !pivotToggle;
            playerInstance.MoveActive = !pivotToggle;
            PivotInstance.isActive = pivotToggle;
            detachToggle = false;
            MainCameraManager.Instance.startFollow();
            DetachInstance.isActive = false;
            freeToggle = false;
            MainCameraManager.Instance.startFollow();
            FreeCameraInstance.isActive = false;
            FreeCameraInstance.gameObject.SetActive(false);
            DetachInstance.gameObject.SetActive(true);
            if (pivotToggle)
            {
                playerInstance.StopPlayerMovementAndInput();
            }
            AppendConsoleLine(Command, inputColor);
            AppendConsoleLine($"Pivot command: {pivotToggle}", commandCompletedColor);
            FinishCommand();
            return;
        }

        if (Command.StartsWith("detachcam") || Command.StartsWith("dc"))
        {
            detachToggle = !detachToggle;
            pivotToggle = false;
            playerInstance.MoveActive = !pivotToggle;
            PivotInstance.isActive = pivotToggle;
            freeToggle = false;
            MainCameraManager.Instance.startFollow();
            FreeCameraInstance.isActive = false;
            FreeCameraInstance.gameObject.SetActive(false);
            DetachInstance.gameObject.SetActive(true);
            if (detachToggle)
            {
                MainCameraManager.Instance.stopFollow();
                DetachInstance.isActive = true;
                playerInstance.StopPlayerMovementAndInput();
            }
            else
            {
                MainCameraManager.Instance.startFollow();
                DetachInstance.isActive = false;
            }
            AppendConsoleLine(Command, inputColor);
            AppendConsoleLine($"Detach command: {detachToggle}", commandCompletedColor);
            FinishCommand();
            return;
        }

        if (Command.StartsWith("freecam") || Command.StartsWith("fc"))
        {
            freeToggle = !freeToggle;
            detachToggle = false;
            MainCameraManager.Instance.startFollow();
            DetachInstance.isActive = false;
            pivotToggle = false;
            playerInstance.MoveActive = !pivotToggle;
            PivotInstance.isActive = pivotToggle;
            if(freeToggle)
            {
                FreeCameraInstance.gameObject.SetActive(true);
                DetachInstance.gameObject.SetActive(false);
                FreeCameraInstance.isActive = true;
                MainCameraManager.Instance.stopFollow();
                playerInstance.StopPlayerMovementAndInput();
            }
            else
            {
                FreeCameraInstance.gameObject.SetActive(false);
                DetachInstance.gameObject.SetActive(true);
                MainCameraManager.Instance.startFollow();
                FreeCameraInstance.isActive = false;
            }
            AppendConsoleLine(Command, inputColor);
            AppendConsoleLine($"Free Cam command: {freeToggle}", commandCompletedColor);
            FinishCommand();
            return;
        }

        if (Command.StartsWith("addpin") || Command.StartsWith("ap"))
        {
            string postCommand = "";
            if (Command.StartsWith("addpin"))
            {
                postCommand = Command.Substring(6);
            }
            else
            {
                postCommand = Command.Substring(2);
            }

            postCommand = RemoveWhitespaces(postCommand);

            if (postCommand == null || postCommand.Length == 0)
            {
                AppendConsoleLine(Command, incorrectInputColor);
                AppendConsoleLine($"The Add Pins command requires either\n - A number (the index of the pin in the static data manager)\n - A name (the name of the pin).", Color.orange);
                FinishCommand();
                return;
            }

            PinScriptable pin = null;

            if (int.TryParse(postCommand, out int pinInt))
            {
                if (pinInt < 0 || pinInt >= StaticDataManager.Instance.Pins.Count)
                {
                    AppendConsoleLine(Command, incorrectInputColor);
                    AppendConsoleLine($"The Add Pins <int> command must have an int between 0 (inclusive) and {StaticDataManager.Instance.Pins.Count - 1} (inclusive).", Color.orange);
                    FinishCommand();
                    return;
                }

                pin = StaticDataManager.Instance.Pins[pinInt];
            }
            else
            {
                pin = StaticDataManager.Instance.Pins.FirstOrDefault(x => RemoveWhitespaces(x.PinName.ToLower()) == postCommand);
            }

            if (pin == null)
            {
                AppendConsoleLine(Command, incorrectInputColor);
                AppendConsoleLine($"Pin Not Found.", Color.orange);
                FinishCommand();
                return;
            }

            MidRunDataManager.Instance.AddPinToInventory(pin.CreateNonRefCopy<PinScriptable>());
            AppendConsoleLine(Command, inputColor);
            AppendConsoleLine($"The Pin {pin.PinName} has been added to your inventory", commandCompletedColor);
            FinishCommand();
            return;
        }


        AppendConsoleLine(Command, incorrectInputColor);
        AppendConsoleLine($"No command found, use Help for a list of all commands.", Color.orange);
        FinishCommand();
        Debug.LogWarning("no command found found for " + Command);

    }

    /// <summary>
    /// adds a line to break up commands
    /// </summary>
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

    /// <summary>
    /// returns a copy of the given string with all whitespaces removed
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string RemoveWhitespaces(string text)
    {
        return string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
    }
}
