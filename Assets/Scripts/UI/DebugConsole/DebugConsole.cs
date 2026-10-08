/*
* Author: Brenden
* Contributors:
* Last Modified: 10/01/2026
* Summary: The debug console and all the code that comes with it
* To Do:   The Debug console SpreadSheet
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField inputs;
    [SerializeField] TMPro.TMP_Text textArea;
    [SerializeField] ScrollRect logScrollRect;
    [SerializeField] GameObject[] enemies;

    [SerializeField] GameObject freeCamPrefab;
    public GameObject FreeCamInstance;
    [SerializeField] GameObject detachCamPrefab;
    public GameObject DetachCamInstance;

    private bool noClipToggle = false;
    private bool godToggle = false;
    private bool cameraToggle = false;
    private bool freezeToggle = false;

    [SerializeField]GameObject playerInstance;
    [SerializeField]GameObject cameraInstance;

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
            AppendConsoleLine(
                Command + "\nGod Mode: god\n" +
                "Change Players Speed: speed <Speed Value(or \"default\")>"
            );
            return;
        }


        // God Mode
        if (Command == "god")
        {
            GodMode();
            AppendConsoleLine(Command + " " + godToggle);
            return;
        }

        // player speed
        if (Command.StartsWith("speed"))
        {
            if (Command.Equals("speed default"))
            {
                float defaultSpeed = playerInstance.GetComponent<PlayerController>().defaultSpeed;
                AppendConsoleLine(Command + " ~ Speed set to default: " + defaultSpeed);
                PlayerSpeed(defaultSpeed);
                return;
            }

            if (Command.Length >= 7)
            {
                int Temp;
                if (int.TryParse(Command.Substring(6, Command.Length - 6), out Temp))
                {
                    AppendConsoleLine(Command + "~ Speed set to: " + Temp);
                    PlayerSpeed(Temp);
                }
                else
                {
                    AppendConsoleLine(Command + " Please put a number after the command");
                }
            }
            else
            {
                AppendConsoleLine(Command + " Please put the speed number (or \"default\") after the command");
            }
            return;
        }

        AppendConsoleLine(Command + " No command found, use Help for all commands");
        Debug.LogWarning("no command found found for " + Command);

    }


    /// <summary>
    /// adds the line of text after
    /// </summary>
    /// <param name="line"></param>
    private void AppendConsoleLine(string line)
    {
        textArea.text = textArea.text + "\n" + line;
        StartCoroutine(ScrollToBottomNextFrame());
    }

    /// <summary>
    /// used later if we want to make the debug console scrollable
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;

        if (logScrollRect == null)
            yield break;

        Canvas.ForceUpdateCanvases();
        if (logScrollRect.content != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(logScrollRect.content);

        logScrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// changes the player's speed
    /// </summary>
    /// <param name="Speed"></param>
    private void PlayerSpeed(float Speed)
    {
        playerInstance.GetComponent<PlayerController>().moveSpeed = Speed;
    }

    /// <summary>
    /// toggles weather the player can take damage or not
    /// </summary>
    private void GodMode()
    {
        godToggle = !godToggle;
        playerInstance.GetComponent<PlayerController>().InGodMode = godToggle;
    }
}
