using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Recordable : MonoBehaviour {

	struct Snapshot {
		public float timestamp;
		public Vector3 position;
		public Quaternion rotation;
	}

	public UnityEvent onRecordingStarted;
	public UnityEvent onRecordingStopped;
	public UnityEvent onRewindStarted;
	public UnityEvent onRewindStopped;

	private Rigidbody rb;
	private bool recording;
	private bool rewinding;

	private List<Snapshot> snapshots = new List<Snapshot>();
	private bool wasKinematic;
	private RigidbodyConstraints cachedConstraints;

	void Awake() {
		rb = GetComponent<Rigidbody>();	
	}

	public void StartRecording() {
		recording = true;
		snapshots.Clear();
		TakeSnapshot(0);
		MaybeInvoke(onRecordingStarted);
	}

	public void StopRecording(float timestamp) {
		recording = false;
		cachedConstraints = rb.constraints;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		MaybeInvoke(onRecordingStopped);
	}

	public void RecordPosition(float timestamp) {
		TakeSnapshot(timestamp);
	}

	public void RewindToPosition(float time) {
		if (snapshots.Count == 0) {
			return;
		}

		int prevIndex = 0;
		for (var i = 0; i < snapshots.Count; ++i) {
			var snapshot = snapshots[i];
			if (snapshot.timestamp == time) {
				ApplySnapshot(snapshot);
				return;
			}
			if (snapshot.timestamp > time) {
				break;
			}
			prevIndex = i;
		}
		if (prevIndex == snapshots.Count -1) {
			ApplySnapshot(snapshots[prevIndex]);
		} else {
			var prev = snapshots[prevIndex];
			var next = snapshots[prevIndex + 1];
			float t = (time - prev.timestamp) / (next.timestamp - prev.timestamp);
			MoveTo(Vector3.Lerp(prev.position, next.position, t), Quaternion.Lerp(prev.rotation, next.rotation, t));
		}
	}

	public void StartRewinding(float timestamp) {
		rewinding = true;
		if (rb) {
			wasKinematic = rb.isKinematic;
			rb.isKinematic = true;
			rb.constraints = cachedConstraints;
		}
		RewindToPosition(timestamp);
		MaybeInvoke(onRewindStarted);
	}

	public void StopRewinding() {
		rewinding = false;
		if (rb) {
			rb.isKinematic = wasKinematic;
		}

		RewindToPosition(0);
		MaybeInvoke(onRewindStopped);
	}

	void TakeSnapshot(float timestamp) {
		var snapshot = new Snapshot {
			timestamp = timestamp,
			position = transform.position,
			rotation = transform.rotation
		};
		snapshots.Add(snapshot);
	}

	void ApplySnapshot(Snapshot snapshot) {
		MoveTo(snapshot.position, snapshot.rotation);
	}

	void MoveTo(Vector3 position, Quaternion rotation) {
		if (rb) {
			rb.MovePosition(position);
			rb.MoveRotation(rotation);
		} else {
			transform.position = position;
			transform.rotation = rotation;
		}
	}

	void Update() {
		
	}

	void MaybeInvoke(UnityEvent e) {
		if (e != null) {
			e.Invoke();
		}
	}
}
