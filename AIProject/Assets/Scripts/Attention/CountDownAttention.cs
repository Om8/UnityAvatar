using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AI.Volume.Bot.Attention {
	//Counts down in between attention grabs. Resets the AI to look around mode if the player does not interact in a while.
	public class CountDownAttention : MonoBehaviour
	{

		[SerializeField, Tooltip("How long should the AI look at the player before continuing what they were doing")]
		private float timePayingAttention = 10;
		//Current cooldown
		private float currentCooldownTime;
		//At the end of the attention grab, call this event.
		[SerializeField, Tooltip("Called once the timer has reached 0")]
		private UnityEvent endAttention;

		bool isAttending;

		private void Start()
		{
			//Sets the value of the current cooldown time to the max time that the AI can pay attention to the player. This is pre set by the user and can change.
			currentCooldownTime = timePayingAttention;
		}

		/// <summary>
		/// Call this to reset the count down timer, this keeps the AIs attention on the player.
		/// </summary>
		public void IsStillInteracting()
		{
			//Again, sets the current cooldown time to the max, but this time it is meant to reset it when someone speaks to the AI.
			currentCooldownTime = timePayingAttention;
			//Is the AI current attending a person, if they are, set it to true. 
			isAttending = true;
		}

		// Update is called once per frame. Happens every frame. Keeps ticking away every frame.
		void Update()
		{
			CountDown();
		}

		//This function is called every frame by the Update above, which calls every frame. It reduces a value and if it hits 0, calls the Unity event to send the AI back into pondering.
		private void CountDown()
		{
			if (isAttending)
			{
				//Clamp the value as we don't really want it counting down forever.
				currentCooldownTime = Mathf.Clamp(currentCooldownTime - Time.deltaTime, 0, timePayingAttention);
				//Is the countdown value equals 0? 
				if (currentCooldownTime == 0)
				{
					//If the unity event is not empty, call the event. This should not be empty most of the time.
					if (endAttention != null) endAttention.Invoke();
					//The bot has stopped attending the player.
					isAttending = false;
				}
			}
		}
	}
}
