/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 11/19/2026
* Summary: A series of methods and variables to debug enemies
* To Do:   
*/

using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyDebug : MonoBehaviour
{
    [SerializeField] TMP_Text healthDisplay;
    [SerializeField] Canvas debuggingCanvas;

    private Enemy enemy;

    [SerializeField]
    private string lookAtPointName;

    /// <summary>
    /// Grab the enemy to debug, and setup LookAtConstraint
    /// </summary>
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        var constraint = GetComponentInChildren<LookAtConstraint>();
        constraint.AddSource(new ConstraintSource() { sourceTransform = Camera.main.transform.Find(lookAtPointName), weight = 1f });
    }

    /// <summary>
    /// Enable the debugging canvas, and add/setup various listeners
    /// </summary>
    private void OnEnable()
    {
        debuggingCanvas.enabled = true;
        enemy.healthChanged += UpdateHealth;
        enemy.InvokeHealthChanged();
        
    }

    /// <summary>
    /// Disable the debugging canvas, and remove various listeners
    /// </summary>
    private void OnDisable()
    {
        debuggingCanvas.enabled = false;
        enemy.healthChanged -= UpdateHealth;
    }

    /// <summary>
    /// Update the enemy's WorldCanvas with a health display
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxHealth"></param>
    public void UpdateHealth(float health, float maxHealth)
    {
        healthDisplay.text = health + " / " + maxHealth;
    }
}
