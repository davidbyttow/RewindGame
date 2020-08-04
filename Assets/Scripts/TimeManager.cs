using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour {

	public static TimeManager inst;

	public ScreenFx screenFx;
	public TextMeshProUGUI statusText;
	public float maxRecordingDuration = 3;
	public bool autoRewind = true;

	enum State {
		None,
		Recording,
		Recorded,
		Rewinding,
	}

	private List<Recordable> recordables = new List<Recordable>();
	private State state = State.None;
	private bool recording { get { return state == State.Recording; } }
	private bool recorded { get { return state == State.Recorded; } }
	private bool rewinding { get { return state == State.Rewinding; } }
	private float durationRecorded = 0;
	private float rewindTimestamp;

	private bool canRecord {  get { return durationRecorded == 0; } }
	private bool canRewind { get { return durationRecorded > 0 && !rewinding; } }

	void Awake() {
		if (inst != null) {
			throw new System.Exception("inst already set");
		}
		inst = this;
		statusText.text = "";
	}

	void Start() {
		recordables.AddRange(FindObjectsOfType<Recordable>());
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
			if (canRecord) {
				Record();
			} else if (recording) {
				StopRecording();
			} else if (canRewind) {
				Rewind();
			}
		}

		if (recording) {
			durationRecorded += Time.deltaTime;
			statusText.text = "Recording";
			statusText.color = Color.red;

			if (durationRecorded >= maxRecordingDuration) {
				StopRecording();
			}
		} else if (rewinding) {
			statusText.text = "Rewinding";
			statusText.color = Color.yellow;

			rewindTimestamp += Time.deltaTime;
			var timestamp = Mathf.Clamp(durationRecorded - rewindTimestamp, 0, durationRecorded);
			recordables.ForEach((r) => { r.RewindToPosition(timestamp); });

			if (rewindTimestamp >= durationRecorded) {
				EraseTape();
			}
		} else if (canRewind && !recording) {
			statusText.text = "Recorded";
			statusText.color = Color.green;
		} else {
			statusText.text = "Ready";
			statusText.color = Color.white;
		}
	}

	void LateUpdate() {
		if (recording) {
			recordables.ForEach((r) => { r.RecordPosition(durationRecorded); });
		}
	}

	void Record() {
		if (!canRecord) {
			return;
		}
		state = State.Recording;
		durationRecorded = 0;
		recordables.ForEach((r) => { r.StartRecording(); });
		if (screenFx) {
			screenFx.SetState(ScreenFx.State.Recording);
		}
	}

	void StopRecording() {
		state = State.Recorded;
		recordables.ForEach((r) => { r.StopRecording(durationRecorded); });
		if (autoRewind) {
			Rewind();
		}
	}

	public void Rewind() {
		if (!canRewind) {
			return;
		}
		state = State.Rewinding;
		rewindTimestamp = 0;
		recordables.ForEach((r) => { r.StartRewinding(durationRecorded); });
		if (screenFx) {
			screenFx.SetState(ScreenFx.State.Rewinding);
		}
	}

	void EraseTape() {
		state = State.None;
		durationRecorded = 0;
		rewindTimestamp = 0;
		recordables.ForEach((r) => { r.StopRewinding(); });
		if (screenFx) {
			screenFx.SetState(ScreenFx.State.Default);
		}
	}
}