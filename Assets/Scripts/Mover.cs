using ECM.Examples.Common;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour {

	public float moveTime = 3.0f;
	public bool waitForTrigger;
	public bool loop = true;

	[SerializeField]
	private Vector3 offset;

	private bool shouldUpdate;
	private Rigidbody rb;

	private Vector3 startPosition;
	private Vector3 targetPosition;
	private float time;
	private float recordTime;
	private bool triggered = false;

	public void Trigger() {
		triggered = true;
	}

	public void Awake() {
		shouldUpdate = true;
		time = 0;
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

		startPosition = transform.position;
		targetPosition = startPosition + offset;

		var recordable = GetComponent<Recordable>();
		if (recordable) {
			recordable.onRecordingStarted.AddListener(() => {
				recordTime = time;
			});
			recordable.onRecordingStopped.AddListener(() => {
				shouldUpdate = false;
			});
			recordable.onRewindStopped.AddListener(() => {
				shouldUpdate = true;
				time = recordTime;
			});
		}
	}

	public void FixedUpdate() {
		if (shouldUpdate && (!waitForTrigger || triggered)) {
			time += Time.deltaTime;

			var t = 0.0f;
			if (loop) {
				t = Utils.EaseInOut(Mathf.PingPong(time, moveTime), moveTime);
			} else {
				if (time >= moveTime) {
					t = 1;
				} else {
					t = Utils.EaseInOut(time, moveTime);
				}
			}
			var p = Vector3.Lerp(startPosition, targetPosition, t);
			rb.MovePosition(p);
		}
	}
}
