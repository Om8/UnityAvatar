using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using AI.Volume.Bot.Events;

namespace AI.Volume.Bot.Understanding
{
	public class WatsonUnderstanding : MonoBehaviour
	{
		#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
		[Space(10)]
		[SerializeField, Tooltip("The IAM apikey.")]
		private string iamApikey;
		[SerializeField, Tooltip("The service URL (optional). This defaults to \"https://api.us-south.assistant.watson.cloud.ibm.com\"")]
		private string serviceUrl;
		[SerializeField, Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
		private string versionDate;
		[SerializeField, Tooltip("The assistantId to run the example.")]
		private string assistantId;

		public AudioEvent returnedMessage;

		#endregion

		private AssistantService service;

		private string sessionId;

		private void Start()
		{
			LogSystem.InstallDefaultReactors();
			Runnable.Run(CreateService());
		}

		private IEnumerator CreateService()
		{
			if (string.IsNullOrEmpty(iamApikey))
			{
				throw new IBMException("Plesae provide IAM ApiKey for the service.");
			}

			//  Create credential and instantiate service
			IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

			//  Wait for tokendata
			while (!authenticator.CanAuthenticate())
				yield return null;

			service = new AssistantService(versionDate, authenticator);
			if (!string.IsNullOrEmpty(serviceUrl))
			{
				service.SetServiceUrl(serviceUrl);
			}

			CreateSession();
		}


		/// <summary>
		/// Input a string for Watson to try give a response.
		/// </summary>
		/// <param name="message">Message you want to send to Watson</param>
		public void InputMessage(string message)
		{
			var input1 = new MessageInput()
			{
				Text = message,
				Options = new MessageInputOptions()
				{
					ReturnContext = true
				}
			};
			service.Message(OnMessageReturned, assistantId, sessionId, input: input1);
		}

		//Create session.
		private void CreateSession()
		{
			service.CreateSession(OnCreateSession, assistantId);
		}

		//Call delete session.
		private void DeleteSession()
		{
			service.DeleteSession(OnDeleteSession, assistantId, sessionId);
		}

		//Session is deleted.
		private void OnDeleteSession(DetailedResponse<object> response, IBMError error)
		{
			Log.Debug("ExampleAssistantV2.OnDeleteSession()", "Session deleted.");
		}

		//Watson has given us a response.
		private void OnMessageReturned(DetailedResponse<MessageResponse> response, IBMError error)
		{
			if(returnedMessage != null) returnedMessage.Invoke(response.Result.Output.Generic[0].Text);
		}

		//Session created.
		private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
		{
			sessionId = response.Result.SessionId;
		}
	}
}

