/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 11/13/2026
 * Brief: Allows for animation events in the enemy animator.
 * TODO: Ideally, this should be removed later. I'm using this for now because 
 * we don't have a way to call anim event for the enemies. It'll take too long to 
 * reorganize the enemy prefabs so the animator is where it should be, so this is a temp fix.
 * ***************************************************************************/
using UnityEngine;

public class EnemyAnimationEventController : MonoBehaviour
{
    public void EnemyDie()
    {
        GetComponentInParent<Enemy>().EnemyDeath();
    }
}
