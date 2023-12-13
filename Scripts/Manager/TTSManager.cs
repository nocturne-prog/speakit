using System;
using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using UnityEngine;

namespace Marvrus
{
    public class TTSManager : Singleton<TTSManager>
    {
        private GoogleTTS tts;
        private AudioSource audioSource;
        public AudioSource GetAudioSource => audioSource;

        protected override void Awake()
        {
            base.Awake();

            if(audioSource is null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (tts is null)
            {
                tts = Instantiate(Resources.Load<GoogleTTS>(Const.GoggleTTS_PATH), transform);
                tts.SetApiKey(Const.GoogleCloudApiKey);
            }
        }

        public void GetTTSAudioClip(string _text, string _mp3FileName, Action<AudioClip> _success, Action _error = null)
        {
            audioSource.Stop();
            audioSource.clip = null;

            tts.GetSpeechAudioFromGoogle(_text, _mp3FileName, (clip) =>
            {
                _success(clip);
            }, (error) =>
            {
                Debug.LogError($"TTS ERROR {error.error.code} : {error.error.message}");
                _error?.Invoke();
            });
        }

        public void PlayTTS(string _text, string _mp3FileName, Action _success = null, Action _error = null)
        {
            GetTTSAudioClip(
                _text: _text,
                _mp3FileName: _mp3FileName,
                _success: (clip) =>
               {
                   audioSource.clip = clip;
                   audioSource.Play();
                   _success?.Invoke();
               },
                _error: _error);
        }
    }
}