using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Volume.Bot.Visual
{
	public class Blink : MonoBehaviour
	{
		[SerializeField, Tooltip("Skin mesh renderer that has all of the values that effect the mesh"), CannotBeNullObjectField]
		private SkinnedMeshRenderer blendShapes = null;

		[SerializeField, Tooltip("Minimum time between blinks")]
		private float minTime = 1.4f;
		[SerializeField, Tooltip("Maximum time between blinks")]
		private float maxTime = 4;
		[SerializeField, Tooltip("speed of characters blink")]
		private float blinkSpeedMultiplier = 10;
		[SerializeField]
		private int leftEyeLid = 0;
		[SerializeField]
		private int rightEyeLid = 1;

		private bool blinking = false;
		private float currentBlink = 0;
		private bool goingDown = true;
		[SerializeField, Tooltip("Leave at true for blinking, set to false to disable blinking")]
		private bool blinkLoopBool = true;

		// Start is called before the first frame update
		private void Start()
		{
			if (blendShapes != null)
			{
				StartCoroutine(BlinkLoop());
			}
		}

		private void Update()
		{
			if (blendShapes != null)
			{
				//Increase or decrease value depending on a boolean
				currentBlink = Mathf.Clamp(currentBlink + (Time.deltaTime * (goingDown ? blinkSpeedMultiplier : -blinkSpeedMultiplier)), 0, 100);
				//Is going down
				if (goingDown && currentBlink == 100)
				{
					goingDown = false;
				}
				//is going up.
				else if (currentBlink == 0)
				{
					blinking = false;
				}
				//Set the eyelids.
				blendShapes.SetBlendShapeWeight(leftEyeLid, currentBlink);
				blendShapes.SetBlendShapeWeight(rightEyeLid, currentBlink);
			}
		}

		//Loops forever and blinks. 
		private IEnumerator BlinkLoop()
		{
			while (blinkLoopBool)
			{
				yield return new WaitForSeconds(Random.Range(minTime, maxTime));
				if (!blinking)
				{
					blinking = true;
					goingDown = true;
				}
			}
		}


	}
}
