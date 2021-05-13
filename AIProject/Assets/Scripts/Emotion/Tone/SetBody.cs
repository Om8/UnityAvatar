using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Volume.Bot.Tone
{
	public class SetBody : MonoBehaviour
	{
		[SerializeField, CannotBeNullObjectField]
		public Animator animationController;

		[SerializeField]
		string animationIntName = "Emotion";

		//Dictionary to convert emotions to a number, which will set face and body.
		Dictionary<string, int> emotionStates = new Dictionary<string, int>()
		{
			{"Base", 0 },
			{"Anger", 5 },
			{"Fear", 3 },
			{"Joy", 1 },
			{"Sadness", 6 },
			{"Analytical", 2 },
			{"Confident", 0 },
			{"Tentative", 4 },
		};

		/// <summary>
		/// Set the body emotion of the avatar. 
		/// </summary>
		/// <param name="emotion">Has to be one of the following to get a result: Base, Anger, Fear, Joy, Sadness, Analytical, Confident, Tentative</param>
		public void SetBodyEmotion(string emotion)
		{
			//If it is in the dictionary, then set the value
			if(emotionStates.TryGetValue(emotion, out int val))
			{
				setAnim(val);
			}
			//Else set it to base.
			else
			{
				setAnim(0);
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
}
