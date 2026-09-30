/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/29/2026
* Summary: Basic bullet travel logic
* To Do:   Add more variables as needed.
*/
using System.Collections;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{

	const float NINETY_DEGREES_ROTATION = 90f;

	/// <summary>
	/// Initialize the bullet's velocity & rotation, then start its aging
	/// </summary>
	/// <param name="bVelocity"></param>
	/// <param name="bulletSpeed"></param>
	/// <param name="bLifetime"></param>
	/// <param name="startPos"></param>
	/// <returns></returns>
	public BulletBehavior Init(Vector3 bVelocity, float bulletSpeed, float bLifetime, Vector3 startPos) 
	{
		bVelocity.y = 0;

        GetComponent<Rigidbody>().linearVelocity = bVelocity * bulletSpeed;

		var rot = Quaternion.LookRotation(bVelocity).eulerAngles;
		rot.x = NINETY_DEGREES_ROTATION;
		transform.SetPositionAndRotation(startPos, Quaternion.Euler(rot));

		StartCoroutine(BulletAging(bLifetime));

		return this;
    }

	/// <summary>
	/// Destroy the bullet after a specified amount of time
	/// </summary>
	/// <param name="bulletLifetime"></param>
	/// <returns></returns>
	private IEnumerator BulletAging(float bulletLifetime) 
	{
		yield return new WaitForSeconds(bulletLifetime);
		Destroy(gameObject);
	}
	
	/// <summary>
	/// Make the player take damage when it comes into contact
	/// </summary>
	/// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController)) 
		{
			playerController.TakeDamage();
			Destroy(gameObject);
		}
    }

}
