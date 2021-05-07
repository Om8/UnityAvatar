using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	bool blinking = false;
	float currentBlink = 0;
	bool goingDown = true;
	[SerializeField, Tooltip("Leave at true for blinking, set to false to disable blinking")]
	bool blinkLoopBool = true;

    // Start is called before the first frame update
    void Start()
    {
        if(blendShapes != null)
		{
			StartCoroutine(BlinkLoop());
		}
    }

	private void Update()
	{
		if(blendShapes != null)
		{
			currentBlink = Mathf.Clamp(currentBlink + (Time.deltaTime * (goingDown ? blinkSpeedMultiplier : -blinkSpeedMultiplier)), 0, 100);
			if (goingDown && currentBlink == 100)
			{
				goingDown = false;
			}
			else if (currentBlink == 0)
			{
				blinking = false;
			}
			blendShapes.SetBlendShapeWeight(0, currentBlink);
			blendShapes.SetBlendShapeWeight(1, currentBlink);
		}
	}

	//Loops forever and blinks. 
	IEnumerator BlinkLoop()
	{
		while (blinkLoopBool){
			yield return new WaitForSeconds(Random.Range(minTime, maxTime));
			if (!blinking)
			{
				blinking = true;
				goingDown = true;
			}
		}
	}


}
