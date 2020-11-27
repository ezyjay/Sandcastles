using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
	public Transform spawnPoint;
	public ParticleSystem saved;

	private SphereCollider sphereCollider;

	private void OnTriggerEnter(Collider other) {
		
		if (other.CompareTag("Player") 
		&& SaveSystem.Instance.SaveData.playerPosition != spawnPoint.position && spawnPoint.position.x > SaveSystem.Instance.SaveData.playerPosition.x) {
			SaveSystem.Instance.SavePlayerPosition(spawnPoint.position);
			SaveSystem.Instance.SaveGame();
			saved.Play();
		}

	}

	void OnDrawGizmos()
	{
		if (sphereCollider == null) 
			sphereCollider = GetComponent<SphereCollider>();

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
	}
}
