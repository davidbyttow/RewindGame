using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ECM.Examples.Common;

public class Switch : MonoBehaviour {

	public UnityEvent onTrigger;

	private bool triggered;
	private float moveDuration = 1;
	private float elapsed = 0;
	private Vector3 startPosition;
	private Vector3 targetPosition;
	private Rigidbody rb;

	void Awake() {
		rb = GetComponent<Rigidbody>();
	}

	void Update() {
		if (triggered && elapsed < 1) {
			elapsed += Time.deltaTime;
			var t = Utils.EaseInOut(elapsed, moveDuration);
			if (t >= 0.999) {
				t = 1;
				if (onTrigger != null) {
					onTrigger.Invoke();
				}
			}
			var p = Vector3.Lerp(startPosition, targetPosition, t);
			rb.MovePosition(p);
		}
	}

	private void TriggerSwitch() {
		triggered = true;
		startPosition = transform.position;
		targetPosition = transform.position + Vector3.down * 0.5f;
	}

	void OnTriggerEnter(Collider other) {
		if (!triggered) {
			if (other.gameObject.CompareTag("Player")) {
				TriggerSwitch();
			}
		}
	}
}
