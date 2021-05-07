using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeadBob : MonoBehaviour
{
	AudioSource audioSource;
	float[] samples = new float[12];
	float volume = 0;

	[Header("Speed")]
	[SerializeField, Tooltip("Speed of the lerp that moves the head to the desired rotation")]
	float headLerpRotation = 1;

	[SerializeField]
	float sideLookDirection = 20;
	[SerializeField]
	float upLookDirection = 25;

	[SerializeField]
	float maxTimeBetweenBobs = 1;
	[SerializeField]
	float minTimeBetweenBobs = .3f;

	float currentCooldown;

	[HideInInspector]
	public Vector3 bobDirection;
	[HideInInspector]
	public Vector3 headOffset;

	Vector3 desiredDirection;


	private void Awake()
	{
		if(audioSource == null)
		{
			audioSource = this.GetComponent<AudioSource>();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			RotateDirection();
		}
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
		if(currentCooldown == 0)
		{
			currentCooldown = Random.Range(minTimeBetweenBobs, maxTimeBetweenBobs);
			RotateDirection();
		}

		bobDirection = Vector3.Slerp(bobDirection, desiredDirection, Time.deltaTime * headLerpRotation);
	}

	void RotateDirection()
	{
		desiredDirection = new Vector2(Random.Range(-sideLookDirection, sideLookDirection) * volume, Random.Range(-upLookDirection, upLookDirection) * volume);
		headOffset.x = desiredDirection.x / sideLookDirection;
		headOffset.y = desiredDirection.y / upLookDirection;
	}
}
