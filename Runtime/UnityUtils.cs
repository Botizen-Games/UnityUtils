using UnityEngine;
using System.Collections.Generic;

namespace BG.UnityUtils.Runtime
{
    public static class UnityUtils
    {
        /// <summary>Determines whether the condition is satisfied according to the probability of success.</summary>
        /// <param name="chance">The probability of success.</param>
        public static bool HasRandomChanceSuccess(float chance) => Random.value <= chance;

        /// <summary>Determines whether the condition is satisfied according to the elapsed time.</summary>
        /// <param name="time">The total time elapsed to satisfy condition.</param>
        /// <param name="timer">The timer used to keep track of elapsed time.</param>
        public static bool HasReachedTime(float time, ref float timer)
        {
            timer += Time.deltaTime;
            return timer >= time;
        }

        /// <summary>Determines whether the condition is satisfied according to the number of frames reached.</summary>
        /// <param name="frameCount">The number of frames to reach.</param>
        public static bool HasReachedFrameCount(int frameCount) => Time.frameCount % frameCount == 0;

        /// <summary>Toggles the behavior component of the object.</summary>
        /// <typeparam name="T">The type that inherits from the Behavior class.</typeparam>
        /// <param name="o">The object that has the behavior.</param>
        public static void ToggleBehaviour<T>(GameObject o) where T : Behaviour => o.GetComponent<T>().enabled = !o.GetComponent<T>().enabled;

        /// <summary>Removes component from the object.</summary>
        /// <typeparam name="T">The type that should be removed.</typeparam>
        /// <param name="go">The object that has the component.</param>
        public static void RemoveComponent<T>(GameObject go) where T : Component
        {
            if (go.TryGetComponent(typeof(T), out Component component))
            {
                Object.Destroy(component);
            }
        }

        /// <summary>Plays audio clip on an audio source. If interrupt is enabled, calling this method will immediately stop the current audio clip and play the new one.</summary>
        /// <param name="source">The audio source.</param>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="interrupt">Should the audio source play the new clip even if the source is already playing?</param>
        public static void PlayAudio(AudioSource source, AudioClip clip, bool interrupt)
        {
            if (interrupt)
            {
                source.PlayOneShot(clip);
            }
            else
            {
                if (!source.isPlaying)
                {
                    source.PlayOneShot(clip);
                }
            }
        }

        /// <summary>Plays a random audio clip from a list. If interrupt is enabled, calling this method will immediately stop the current audio clip and play the new one.</summary>
        /// <param name="source">The audio source.</param>
        /// <param name="clips">The list of audio clips.</param>
        /// <param name="interrupt">Should the audio source play the new clip even if the source is already playing?</param>
        public static void PlayAudio(AudioSource source, List<AudioClip> clips, bool interrupt)
        {
            if (interrupt)
            {
                int index = Random.Range(0, clips.Count);
                source.PlayOneShot(clips[index]);
            }
            else
            {
                if (!source.isPlaying)
                {
                    int index = Random.Range(0, clips.Count);
                    source.PlayOneShot(clips[index]);
                }
            }
        }

        /// <summary>Plays audio clip on an audio source by chance. If interrupt is enabled, calling this method will immediately stop the current audio clip and play the new one.</summary>
        /// <param name="source">The audio source.</param>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="chance">The probability of playing the audio clip.</param>
        /// <param name="interrupt">Should the audio source play the new clip even if the source is already playing?</param>
        public static void PlayAudio(AudioSource source, AudioClip clip, float chance, bool interrupt)
        {
            if (HasRandomChanceSuccess(chance))
            {
                PlayAudio(source, clip, interrupt);
            }
        }

        /// <summary>Plays a random audio clip from a list by chance. If interrupt is enabled, calling this method will immediately stop the current audio clip and play the new one.</summary>
        /// <param name="source">The audio source.</param>
        /// <param name="clips">The list of audio clips.</param>
        /// <param name="chance">The probability of playing an audio clip.</param>
        /// <param name="interrupt">Should the audio source play the new clip even if the source is already playing?</param>
        public static void PlayAudio(AudioSource source, List<AudioClip> clips, float chance, bool interrupt)
        {
            if (HasRandomChanceSuccess(chance))
            {
                PlayAudio(source, clips, interrupt);
            }
        }
    }
}