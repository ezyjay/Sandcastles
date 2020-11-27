using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerFX : MonoBehaviour
{
	[Header("Particle systems")]
	public ParticleSystem bubbleBurst; 
	public ParticleSystem bubbleIdle;
	public ParticleSystem bubblesMoving;
	public ParticleSystem deathFX;
	
	[Header("Player Idle")]
	public bool bobPlayerOnIdle = true;
	public float bobDistance = 3f;
    public float smoothTime = 0.3f;

	[Header("Events")]
	public UnityEvent PlayerStartedMoving;
	public UnityEvent PlayerStoppedMoving;

	private GameObject playerParent;
	private bool isIdle = false;
	private float localPositionY = 0f;
	private float topBobPositionY, bottomBobPositionY;
	private float closeDistance = 0.05f;
	private float lerpPosition = 0f;
    private float yVelocity = 0.0f;

	private void OnEnable()
	{
		bottomBobPositionY = localPositionY;
		playerParent = GameUtil.Player.transform.parent.gameObject;
		localPositionY = playerParent.transform.localPosition.y;
		topBobPositionY = localPositionY + bobDistance;
		GameUtil.Player.controller.PlayerMoving += OnPlayerMoving;
	}

	private void OnDisable() {
		GameUtil.Player.controller.PlayerMoving -= OnPlayerMoving;
	}

	private void Update() {
		if (bobPlayerOnIdle) {
			if (isIdle) {
				if (Mathf.Abs(playerParent.transform.localPosition.y - topBobPositionY) < closeDistance) {
					lerpPosition = bottomBobPositionY;
				}
				else if (Mathf.Abs(playerParent.transform.localPosition.y - bottomBobPositionY) < closeDistance) {
					lerpPosition = topBobPositionY;
				}

				float newY = Mathf.SmoothDamp(playerParent.transform.localPosition.y, lerpPosition, ref yVelocity, smoothTime);
				playerParent.transform.localPosition = new Vector3(playerParent.transform.localPosition.x, newY,playerParent.transform.localPosition.z);
			}
		}
	}

	public void OnPlayerMoving(bool isMoving) {
		if (isMoving) {
			
			isIdle = false;
			bubbleBurst.Play();
			bubblesMoving.Play();
			bubbleIdle.Stop();
			PlayerStartedMoving.Invoke();
		}
		else {
			isIdle = true;
			bubbleBurst.Stop();
			bubblesMoving.Stop();
			bubbleIdle.Play();
			PlayerStoppedMoving.Invoke();
		}
	}
	
}
