/*
* Author: Brenden
* Contributors:
* Last Modified: 09/30/2026
* Summary: just used to reload the scene
* To Do:   N/A
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempReloadScript : MonoBehaviour
{
    /// <summary>
    /// reloads the current scene
    /// </summary>
    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
