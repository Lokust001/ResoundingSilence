/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 11/23/2026
 * Brief: Executes status effect code for as long as they persist.
 * TODO: Add status effect functinality as needed.
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class StatusBehaviour : MonoBehaviour
{
    protected bool decayApplied = false;
    protected DecayData decayRef;
    int decayTotalDamage = 0;
    int decayDamage;
    int maxDecayDamage;
    float decayTimeElapsed = 0;

    /// <summary>
    /// Causes the entity to start taking decay damage
    /// </summary>
    public void InflictDecay(DecayData dRef)
    {
        decayTimeElapsed = 0;
        decayRef = dRef;
        maxDecayDamage = decayRef.DecayDamagePerStack * decayRef.MaxDecayStacks;
        decayTotalDamage = decayTotalDamage + decayRef.DecayDamagePerStack <= maxDecayDamage ? decayTotalDamage + decayRef.DecayDamagePerStack : maxDecayDamage;

        decayDamage = Mathf.CeilToInt(decayTotalDamage / decayRef.DecayTicks);

        if (!decayApplied)
        {
            decayApplied = true;
            StartCoroutine(DecayDamage());
        }
    }

    /// <summary>
    /// Deals x decay damage every y seconds
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DecayDamage()
    {
        float delay = decayRef.DecayDuration / decayRef.DecayTicks;

        do
        {
            if(GetComponent<Enemy>())
            {
                GetComponent<Enemy>().EnemyTakeDamage(decayDamage);
            }
            else if(GetComponent<PlayerController>())
            {
                GetComponent<PlayerController>().TakeDamage(decayDamage);
            }

            yield return new WaitForSeconds(delay);
            decayTimeElapsed += delay;
        } while (decayTimeElapsed < decayRef.DecayDuration);

        decayApplied = false;
    }
}
