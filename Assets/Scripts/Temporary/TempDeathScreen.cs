/*
* Author: Tyler
* Contributors:
* Last Modified: 11/10/2026
* Summary: temp death screen manager
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TempDeathScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject button;

    /// <summary>
    /// controller support for when the menu opens
    /// </summary>
    private void OnEnable()
    {
        if (InputManager.Instance.ControllerIsEnabled)
        {
            SelectRestartBtn();   
        }
        InputPublicEvents.ControllerEnabled += SelectRestartBtn;
    }

    /// <summary>
    /// Selects the restart button
    /// </summary>
    private void SelectRestartBtn()
    {
        EventSystem.current.SetSelectedGameObject(button);
    }

    /// <summary>
    /// restarts da game
    /// </summary>
    public void RestartGame()
    {
        AppManager.Instance = null;
        InputManager.Instance = null;
        UIManager.Instance = null;
        StaticDataManager.Instance = null;
        MidRunDataManager.Instance = null;
        GameTimerManager.Instance = null;
        MainCameraManager.Instance = null;
        MapManager.Instance = null;
        PlayerManager.Instance = null;
        ControllerUIService.Instance = null;
        MouseUIService.Instance = null;
        TooltipUIService.Instance = null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
