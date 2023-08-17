using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtilities
{
    [RequireComponent(typeof(AudioSource))]
    public class BGMConfig : MonoBehaviour
    {
        private static AudioSource audioSource;
        private static BGMConfig instance;
        private static bool isConfigured = false;


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
                audioSource = GetComponent<AudioSource>();
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Configuration(AudioClip audioClip)
        {

            if (isConfigured)
                return;

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.priority = 256;
            audioSource.playOnAwake = false;
            audioSource.Play();
            isConfigured = true;
        }


        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }


    }
}
