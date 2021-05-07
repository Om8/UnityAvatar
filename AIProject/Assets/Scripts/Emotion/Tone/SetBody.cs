using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBody : MonoBehaviour
{
	[SerializeField, CannotBeNullObjectField]
	public Animator animationController;

	[SerializeField]
	string animationIntName = "Emotion";
	
	/// <summary>
	/// Set the body emotion of the avatar. 
	/// </summary>
	/// <param name="emotion">Has to be one of the following to get a result: Base, Anger, Fear, Joy, Sadness, Analytical, Confident, Tentative</param>
	public void SetBodyEmotion(string emotion)
	{
		switch (emotion)
		{
			case "Base":
				setAnim(0);
				break;
			case "Anger":
				setAnim(5);
				break;
			case "Fear":
				setAnim(3);
				break;
			case "Joy":
				setAnim(1);
				break;
			case "Sadness":
				setAnim(6);
				break;
			case "Analytical":
				setAnim(2);
				break;
			case "Confident":
				setAnim(0);
				break;
			case "Tentative":
				setAnim(4);
				break;
			default:
				setAnim(0);
				break;
		}
	
	}

	/// <summary>
	/// Set the animation in the animation controller
	/// </summary>
	/// <param name="num">Number has to be between 0-6 to give you a result.</param>
	void setAnim(int num)
	{
		if (num >= 0 && num <= 6)
		{
			animationController.SetInteger(animationIntName, num);
		}
		else
		{
			Debug.LogWarning("Set anim number out of range");
		}
	}
}
