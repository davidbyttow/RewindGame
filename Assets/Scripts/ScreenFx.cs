using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(Volume))]
public class ScreenFx : MonoBehaviour {

	public VolumeProfile rewindProfile;

	private Volume volume;
	private VolumeProfile defaultProfile;

	public enum State {
		Default,
		Recording,
		Rewinding,
	};

	private State state = State.Default;

	void Awake() {
		volume = GetComponent<Volume>();
		defaultProfile = volume.profile;
	}

	public void SetState(State state) {
		if (state == this.state) {
			return;
		}
		this.state = state;
		switch (state) {
			case State.Rewinding:
				if (rewindProfile) {
					volume.profile = rewindProfile;
				}
				break;
			default:
				volume.profile = defaultProfile;
				break;
		}
	}
}
