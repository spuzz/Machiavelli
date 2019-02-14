using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips;

    int currentClip = 0;
    // Update is called once per frame
    void Update () {
		if(audioSource.isPlaying == false && audioClips.Count > 0)
        {
            if(audioClips.Count == 1)
            {
                audioSource.PlayOneShot(audioClips[0]);
            }
            else
            {
                int clipNumber = -1;
                while (clipNumber < 0 || clipNumber == currentClip)
                {
                    clipNumber = Random.Range(0, audioClips.Count);
                }

                audioSource.clip = audioClips[clipNumber];
                audioSource.Play();
                currentClip = clipNumber;
            }

        }
	}
}
