using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Volume.Bot.Visual
{
	public class Blink : MonoBehaviour
	{
		[SerializeField, Tooltip("Skin mesh renderer that has all of the values that effect the mesh"), CannotBeNullObjectField]
		SkinnedMeshRenderer blendShapes = null;

		[SerializeField, Tooltip("Minimum time between blinks")]
		float minTime = 1.4f;
		[SerializeField, Tooltip("Maximum time between blinks")]
		float maxTime = 4;
		[SerializeField, Tooltip("speed of characters blink")]
		float blinkSpeedMultiplier = 10;
		[SerializeField]
		int leftEyeLid = 0;
		[SerializeField]
		int rightEyeLid = 1;

		bool blinking = false;
		float currentBlink = 0;
		bool goingDown = true;
		[SerializeField, Tooltip("Leave at true for blinking, set to false to disable blinking")]
		bool blinkLoopBool = true;

		// Start is called before the first frame update
		void Start()
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
		IEnumerator BlinkLoop()
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
