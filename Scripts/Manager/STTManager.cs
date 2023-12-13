using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;
using Marvrus.Util;
using UnityEngine;

namespace Marvrus
{
    public class STTManager : Singleton<STTManager>
    {
        private GCSpeechRecognition googleSTT;

        protected override void Awake()
        {
            base.Awake();

			if (googleSTT is null)
			{
				googleSTT = Instantiate(Resources.Load<GCSpeechRecognition>(Const.GoogleSTT_PATH), transform);
			}

            googleSTT = GCSpeechRecognition.Instance;
            googleSTT.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
            googleSTT.RecognizeFailedEvent += RecognizeFailedEventHandler;
            googleSTT.LongRunningRecognizeSuccessEvent += LongRunningRecognizeSuccessEventHandler;
            googleSTT.LongRunningRecognizeFailedEvent += LongRunningRecognizeFailedEventHandler;
            googleSTT.GetOperationSuccessEvent += GetOperationSuccessEventHandler;
            googleSTT.GetOperationFailedEvent += GetOperationFailedEventHandler;
            googleSTT.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
            googleSTT.ListOperationsFailedEvent += ListOperationsFailedEventHandler;

            googleSTT.FinishedRecordEvent += FinishedRecordEventHandler;
            googleSTT.StartedRecordEvent += StartedRecordEventHandler;
            googleSTT.RecordFailedEvent += RecordFailedEventHandler;

            googleSTT.BeginTalkigEvent += BeginTalkigEventHandler;
            googleSTT.EndTalkigEvent += EndTalkigEventHandler;

			MicrophoneDevicesDropdownOnValueChangedEventHandler(0);
		}

        private void OnDestroy()
        {
            googleSTT.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
            googleSTT.RecognizeFailedEvent -= RecognizeFailedEventHandler;
            googleSTT.LongRunningRecognizeSuccessEvent -= LongRunningRecognizeSuccessEventHandler;
            googleSTT.LongRunningRecognizeFailedEvent -= LongRunningRecognizeFailedEventHandler;
            googleSTT.GetOperationSuccessEvent -= GetOperationSuccessEventHandler;
            googleSTT.GetOperationFailedEvent -= GetOperationFailedEventHandler;
            googleSTT.ListOperationsSuccessEvent -= ListOperationsSuccessEventHandler;
            googleSTT.ListOperationsFailedEvent -= ListOperationsFailedEventHandler;

            googleSTT.FinishedRecordEvent -= FinishedRecordEventHandler;
            googleSTT.StartedRecordEvent -= StartedRecordEventHandler;
            googleSTT.RecordFailedEvent -= RecordFailedEventHandler;

            googleSTT.BeginTalkigEvent -= BeginTalkigEventHandler;
            googleSTT.EndTalkigEvent -= EndTalkigEventHandler;
        }

		private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
		{
			if (!googleSTT.HasConnectedMicrophoneDevices())
			{
				Debug.LogError($"STT Manager :: MicrophoneDevicesDropdownOnValueChangedEventHandler :: {value}");
				return;
			}

			googleSTT.SetMicrophoneDevice(googleSTT.GetMicrophoneDevices()[value]);
		}

		private Action<RecognitionResponse> successCallback = null;

		public void StartRecord()
		{
			successCallback = null;
			googleSTT.StartRecord(false);
		}

		public void StopRecord(Action<RecognitionResponse> _callback = null)
		{
			successCallback = _callback;
			googleSTT.StopRecord();
		}

		private void StartedRecordEventHandler()
		{
			Debug.Log($"STT Manager :: StartedRecordEventHandler");
		}

		private void RecordFailedEventHandler()
		{
			Debug.Log($"STT Manager :: RecordFailedEventHandler");
		}

		private void BeginTalkigEventHandler()
		{
			Debug.Log($"STT Manager :: BeginTalkigEventHandler");
		}

		private void EndTalkigEventHandler(AudioClip clip, float[] raw)
		{
			Debug.Log($"STT Manager :: EndTalkigEventHandler");
			FinishedRecordEventHandler(clip, raw);
		}

		private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
		{
			Debug.Log($"STT Manager :: FinishedRecordEventHandler");

			if (clip == null)
			{
				Debug.LogError($"STT Manager :: FinishedRecordEventHandler :: clip is null");
				return;
			}

			RecognitionConfig config = RecognitionConfig.GetDefault();

			config.languageCode = Enumerators.LanguageCode.en_US.Parse();
			config.speechContexts = new SpeechContext[]
			{
				new SpeechContext()
				{
					//phrases = _contextPhrasesInputField.text.Replace(" ", string.Empty).Split(',')
				}
			};
			config.audioChannelCount = clip.channels;
			// configure other parameters of the config if need

			GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
			{
				audio = new RecognitionAudioContent()
				{
					content = raw.ToBase64()
				},
				//audio = new RecognitionAudioUri() // for Google Cloud Storage object
				//{
				//	uri = "gs://bucketName/object_name"
				//},
				config = config
			};

			googleSTT.Recognize(recognitionRequest);
		}

		private void GetOperationFailedEventHandler(string error)
		{
			Debug.LogError($"STT Manager :: GetOperationFailedEventHandler :: {error}");
		}

		private void ListOperationsFailedEventHandler(string error)
		{
			Debug.LogError($"STT Manager :: ListOperationsFailedEventHandler :: {error}");
		}

		private void RecognizeFailedEventHandler(string error)
		{
			Debug.LogError($"STT Manager :: RecognizeFailedEventHandler :: {error}");
		}

		private void LongRunningRecognizeFailedEventHandler(string error)
		{
			Debug.LogError($"STT Manager :: LongRunningRecognizeFailedEventHandler :: {error}");
		}

		private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
		{
			Debug.Log($"STT Manager :: ListOperationsSuccessEventHandler");

			if (operationsResponse.operations != null)
			{
				StringBuilder sb = new StringBuilder("Operations:\n");
				foreach (var item in operationsResponse.operations)
				{
					sb.Append("name: " + item.name + "; done: " + item.done + "\n");
				}
			}
		}

		private void GetOperationSuccessEventHandler(Operation operation)
		{
			Debug.Log($"STT Manager :: GetOperationSuccessEventHandler");

			if (operation.done && (operation.error == null || string.IsNullOrEmpty(operation.error.message)))
			{
				InsertRecognitionResponseInfo(operation.response);
			}
		}

		private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
		{
			Debug.Log($"STT Manager :: RecognizeSuccessEventHandler");
			InsertRecognitionResponseInfo(recognitionResponse);
		}
        private void LongRunningRecognizeSuccessEventHandler(Operation operation)
		{
			if (operation.error != null || !string.IsNullOrEmpty(operation.error.message))
				return;

			Debug.Log($"STT Manager :: LongRunningRecognizeSuccessEventHandler");

			if (operation != null && operation.response != null && operation.response.results.Length > 0)
			{
				StringBuilder sb = new StringBuilder("Long Running Recognize Success.");

				sb.AppendLine(operation.response.results[0].alternatives[0].transcript);

				string other = "\nDetected alternatives:\n";

				foreach (var result in operation.response.results)
				{
					foreach (var alternative in result.alternatives)
					{
						if (operation.response.results[0].alternatives[0] != alternative)
						{
							other += alternative.transcript + ", ";
						}
					}
				}

				sb.AppendLine(other);
				Debug.Log(sb.ToString());
			}
			else
			{
				Debug.LogError("Long Running Recognize Success. Words not detected.");
			}
		}

		private void InsertRecognitionResponseInfo(RecognitionResponse recognitionResponse)
		{
			if (recognitionResponse == null || recognitionResponse.results.Length == 0)
			{
                Debug.LogWarning("Words not detected.");
                successCallback?.Invoke(recognitionResponse);
                return;
			}

			StringBuilder sb = new StringBuilder("\n" + recognitionResponse.results[0].alternatives[0].transcript);

			var words = recognitionResponse.results[0].alternatives[0].words;

			if (words != null)
			{
				string times = string.Empty;

				foreach (var item in recognitionResponse.results[0].alternatives[0].words)
				{
					times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";
				}

				sb.Append("\n" + times);
			}

			string other = "\nDetected alternatives: ";

			foreach (var result in recognitionResponse.results)
			{
				foreach (var alternative in result.alternatives)
				{
					if (recognitionResponse.results[0].alternatives[0] != alternative)
					{
						other += alternative.transcript + ", ";
					}
				}
			}

			sb.Append(other);
			Debug.Log(sb.ToString());

			successCallback?.Invoke(recognitionResponse);
		}

		public void GetSpeekingInfo(RecognitionResponse recognitionResponse, out string _answer, out float _time, out int _wordsNum)
		{
			float time = 0;
			int wordsNum = 0;
            StringBuilder sb = new StringBuilder();
            if (recognitionResponse == null || recognitionResponse.results.Length == 0)
            {
                Debug.LogWarning("Words not detected.");
				_answer = sb.ToString();
				_time = time;
				_wordsNum = wordsNum;
                return;
            };

            foreach (var result in recognitionResponse.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    if (alternative.words !=null)
                    {
						sb.Append(alternative.transcript);
						_answer = sb.ToString();
                        _wordsNum = alternative.words.Length;
						foreach(var word in  alternative.words)
						{
                            time += SubtractStringSecond(word.startTime,
                            word.endTime);
                        }
						_time = time;
                        return;
                    }
                }
            }
            _answer = sb.ToString();
            _time = time;
            _wordsNum = wordsNum;
            
        }
        


		public float StringSecondToFloat(string number)
		{
            number = number.TrimEnd('s');
            float num = float.Parse(number);
			return num;
		}
        public float SubtractStringSecond(string number1, string number2)
        {

            // Convert the input strings to float
            float num1 = StringSecondToFloat(number1);
            float num2 = StringSecondToFloat(number2);

            // Perform the subtraction
            float result = num1 - num2;

            return Math.Abs(result);
        }



    }
}