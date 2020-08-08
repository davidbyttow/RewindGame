using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

	public static SoundManager inst { get; private set; }

	public AudioClip[] jumpSounds;
	public AudioClip[] landSounds;
	public AudioClip beep;
	public AudioClip recordStop;
	public AudioClip rewindStop;
	public AudioSource staticSource;

	private AudioSource source;

	void Awake() {
		source = GetComponent<AudioSource>();
		if (inst != null) {
			throw new System.Exception("Can only have one sound manager");
		}
		inst = this;
	}

	public void PlayJump() {
		source.PlayOneShot(Pick(jumpSounds));
	}

	public void PlayLand() {
		source.PlayOneShot(Pick(landSounds));
	}

	public void PlayBeep() {
		source.PlayOneShot(beep);
	}

	public void PlayRecordStop() {
		source.PlayOneShot(recordStop);
	}

	public void PlayRewindStop() {
		source.PlayOneShot(rewindStop);
	}

	public void PlayStatic() {
		staticSource.Play();
	}

	public void StopStatic() {
		staticSource.Stop();
	}

	public AudioClip Pick(AudioClip[] clips) {
		return clips[Random.Range(0, clips.Length)];
	}
}
