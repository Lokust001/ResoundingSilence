using System.Collections;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{

	const float NINETY_DEGREES_ROTATION = 90f;

	public BulletBehavior Init(Vector3 bVelocity, float bulletSpeed, float bLifetime, Vector3 startPos) 
	{
        GetComponent<Rigidbody>().linearVelocity = bVelocity * bulletSpeed;

		var rot = Quaternion.LookRotation(bVelocity).eulerAngles;
		rot.x = NINETY_DEGREES_ROTATION;
		transform.SetPositionAndRotation(startPos, Quaternion.Euler(rot));

		StartCoroutine(BulletAging(bLifetime));

		return this;
    }

	private IEnumerator BulletAging(float bulletLifetime) 
	{
		yield return new WaitForSeconds(bulletLifetime);
		Destroy(gameObject);
	}


}
