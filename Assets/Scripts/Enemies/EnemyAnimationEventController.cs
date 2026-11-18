/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 11/17/2026
 * Brief: Allows for animation events in the enemy animator.
 * TODO: Ideally, this should be removed later. I'm using this for now because 
 * we don't have a way to call anim event for the enemies. It'll take too long to 
 * reorganize the enemy prefabs so the animator is where it should be, so this is a temp fix.
 * ***************************************************************************/
using UnityEngine;

public class EnemyAnimationEventController : MonoBehaviour
{
    /// <summary>
    /// Calls the enemy death function
    /// </summary>
    public void EnemyDie()
    {
        GetComponentInParent<Enemy>().EnemyDeath();
    }

    /// <summary>
    /// Anim  event that calls the single shot attack
    /// </summary>
    public void ShootSingleShot()
    {
        GetComponentInParent<Enemy>().GetComponentInChildren<EnemyAttack>().FireBullet(Quaternion.identity, true);
    }

    /// <summary>
    /// Anim event that tells the enemy if they should look at the player
    /// </summary>
    public void LookAtPlayer()
    {
        GetComponentInParent<EnemyWalk>().LookAtPlayer = true;
        GetComponentInParent<EnemyWalk>().StopApproachingPlayer();
    }

    /// <summary>
    /// Anim event to have the enemy move again
    /// </summary>
    public void MoveAgain()
    {
        GetComponent<Animator>().SetBool("Attacking", false);
        GetComponentInParent<EnemyWalk>().LookAtPlayer = false;
        GetComponentInParent<Enemy>().WaitToAttack();
    }
}
