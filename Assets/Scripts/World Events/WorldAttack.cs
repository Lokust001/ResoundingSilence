/*
* Author: Brenden
* Contributors:
* Last Modified: 09/23/2026
* Summary: warns the player how long they have until they are gonna get hit
* To Do:   add hitboxes once I can get those working
*/
using System.Collections;
using UnityEngine;

public class WorldAttack : MonoBehaviour
{
    [SerializeField] private float windupTime;
    [SerializeField] private GameObject hitbox;
    [SerializeField] private GameObject attackIndicator;
    [SerializeField] private Material attackFresh;
    [SerializeField] private Material attackDanger;

    public IEnumerator windup()
    {
        float timer = 0.0f;
        while(timer <= windupTime)
        {
            timer += 0.1f;
            attackIndicator.GetComponent<Renderer>().material.Lerp(attackFresh, attackDanger, timer/windupTime);
            yield return new WaitForSeconds(0.1f);
        }
        //spawnhitbox
        Destroy(this.gameObject);
    }
}
