using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using UnityEngine.Events;
using AI.Volume.Bot.Events;

namespace AI.Volume.Bot.Audio
{
	[RequireComponent(typeof(SpatialChecker))]
	public class WindowsVoiceInput : MonoBehaviour
	{
		//Voice detection recogniser
		private DictationRecognizer voiceDictation;
		//Calls when the user first speaks to the bot.
		public UnityEvent hasInteracted;
		//Calls when the bot has finished understanding what the player has said.
		public AudioEvent spokenTo;
		[SerializeField, Tooltip("Is spatial, does the voice input have range?")]
		private bool spatial = false;
		[SerializeField, Tooltip("How far can the bot hear? This requires spatial to be turned on")]
		private float spatialRange = 1f;
		private SpatialChecker spatialChecker => this.gameObject.GetComponent<SpatialChecker>();


		// Start is called before the first frame update
		private void Start()
		{
			//Create a new voice recogniser.
			voiceDictation = new DictationRecognizer();

			//Hypothesis, quick responses.
			voiceDictation.DictationHypothesis += (text) =>
			{
				if (spatial && Camera.main != null)
				{
					if (Vector3.Distance(Camera.main.transform.position, this.transform.position) <= spatialRange)
					{
						if(hasInteracted != null) hasInteracted.Invoke();
					}
				}
				else
				{
					if (hasInteracted != null) hasInteracted.Invoke();
				}
			};

			//Assign result and error.
			voiceDictation.DictationResult += DictationResult;
			voiceDictation.DictationError += DictationError;

			//Start voice detection
			voiceDictation.Start();
			voiceDictation.AutoSilenceTimeoutSeconds = 20000;
			voiceDictation.InitialSilenceTimeoutSeconds = 20000;
		}

		/// <summary>
		/// Dictation result, this is the result function that returns when there is a successful result.
		/// </summary>
		/// <param name="text">Final text</param>
		/// <param name="confidence">How confident the system is</param>
		private void DictationResult(string text, ConfidenceLevel confidence)
		{
			//Check if spatial
			if (spatial && Camera.main != null)
			{
				if (spatialChecker != null)
				{
					//Check the spatial checker if the player is in range. It does not have to be a player, could be anything, even another AI.
					if (spatialChecker.IsInRange(this.gameObject, Camera.main.gameObject, spatialRange) && spatialChecker.LineOfSightChecker(this.gameObject, Camera.main.gameObject))
					{
						if (spokenTo != null)
						{
							//If the spoken to event is valid, call it.
							spokenTo.Invoke(text);
							voiceDictation.Stop();
						}
					}
				}
			}
			//Play anyway
			else
			{
				if (spokenTo != null)
				{
					spokenTo.Invoke(text);
					voiceDictation.Stop();
				}
			}
		}

		/// <summary>
		/// The user can speak to the bot again, call at end of voice output.
		/// </summary>
		public void CanSpeakAgain()
		{
			if (voiceDictation.Status != SpeechSystemStatus.Running)
			{
				voiceDictation.Start();
			}
		}

		/// <summary>
		/// On refocus, turn on the voice again. Windows voice turns off as soon as focus is lost.
		/// </summary>
		/// <param name="focus"></param>
		private void OnApplicationFocus(bool focus)
		{
			if (voiceDictation != null)
			{
				if (voiceDictation.Status != SpeechSystemStatus.Running)
				{
					voiceDictation.Start();
				}
			}
		}

		/// <summary>
		/// End of the session, dispose of voice threads.
		/// </summary>
		private void OnDestroy()
		{
			if (voiceDictation.Status == SpeechSystemStatus.Running)
			{
				voiceDictation.Stop();
				voiceDictation.Dispose();
			}
		}

		//On error, debug log the error.
		private void DictationError(string error, int result)
		{
			Debug.Log(error + " || number: " + result);
		}
	}
}
