using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AI.Volume.Bot.Attention {
	public class CountDownAttention : MonoBehaviour
	{
		[SerializeField]
		float timePayingAttention = 10;
		//Current cooldown
		float currentCooldownTime;
		[SerializeField]
		UnityEvent endAttention;

		bool isAttending;

		private void Start()
		{
			currentCooldownTime = timePayingAttention;
		}

		public void IsStillInteracting()
		{
			currentCooldownTime = timePayingAttention;
			isAttending = true;
		}

		// Update is called once per frame
		void Update()
		{
			CountDown();
		}

		void CountDown()
		{
			if (isAttending)
			{
				currentCooldownTime = Mathf.Clamp(currentCooldownTime - Time.deltaTime, 0, timePayingAttention);
				if (currentCooldownTime == 0)
				{
					if (endAttention != null) endAttention.Invoke();
					isAttending = false;
				}
			}
		}
	}
}
