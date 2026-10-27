using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempDeathScreen : MonoBehaviour
{
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
