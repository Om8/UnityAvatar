using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Volume.Bot.Visual
{
	[RequireComponent(typeof(AudioSource))]
	public class HeadBob : MonoBehaviour
	{
		AudioSource audioSource;
		float[] samples = new float[12];
		float volume = 0;

		[Header("Speed")]
		[SerializeField, Tooltip("Speed of the lerp that moves the head to the desired rotation")]
		float headLerpRotation = 1;

		[SerializeField, Tooltip("How far to the side can the character look?")]
		float sideLookDirection = 20;
		[SerializeField, Tooltip("How far up can the character look?")]
		float upLookDirection = 25;

		[SerializeField, Tooltip("Max time between each of the head bobs")]
		float maxTimeBetweenBobs = 1;
		[SerializeField, Tooltip("Min time between head bobs")]
		float minTimeBetweenBobs = .3f;

		float currentCooldown;

		[HideInInspector]
		public Vector3 bobDirection;
		[HideInInspector]
		public Vector3 headOffset;

		Vector3 desiredDirection;


		private void Awake()
		{
			if (audioSource == null)
			{
				audioSource = this.GetComponent<AudioSource>();
			}
		}

		private void Update()
		{
			//Test set rotation
			if (Input.GetKeyDown(KeyCode.B))
			{
				RotateDirection();
			}
			//Get the volume and call cooldown.
			GetLatestVolume();
			CooldownAndCallBob();
		}


		void GetLatestVolume()
		{
			if (audioSource != null)
			{
				volume = 0;
				audioSource.GetOutputData(samples, 0);
				foreach (float samp in samples)
				{
					volume += Mathf.Abs(samp);
				}
			}
		}

		void CooldownAndCallBob()
		{
			currentCooldown = Mathf.Clamp(currentCooldown - Time.deltaTime, 0, maxTimeBetweenBobs);
			//If the cooldown == 0, get a random direction. 
			if (currentCooldown == 0)
			{
				currentCooldown = Random.Range(minTimeBetweenBobs, maxTimeBetweenBobs);
				RotateDirection();
			}

			//Slerp the head about to the desired direction.
			bobDirection = Vector3.Slerp(bobDirection, desiredDirection, Time.deltaTime * headLerpRotation);
		}

		void RotateDirection()
		{
			desiredDirection = new Vector2(Random.Range(-sideLookDirection, sideLookDirection) * volume, Random.Range(-upLookDirection, upLookDirection) * volume);
			headOffset.x = desiredDirection.x / sideLookDirection;
			headOffset.y = desiredDirection.y / upLookDirection;
		}
	}
}
