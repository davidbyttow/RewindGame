using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Spinner : MonoBehaviour {

	public float rotationSpeed = 180;

	private Rigidbody rb;
	private float angle;
	private bool shouldUpdate = true;

	void Awake() {
		rb = GetComponent<Rigidbody>();

		var recordable = GetComponent<Recordable>();
		if (recordable) {
			recordable.onRecordingStopped.AddListener(() => { shouldUpdate = false; });
			recordable.onRewindStopped.AddListener(() => {
				shouldUpdate = true;
				angle = transform.rotation.eulerAngles.z;
			});
		}
	}

	void Start() {
		rb.isKinematic = true;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
	}

	void FixedUpdate() {
		if (shouldUpdate) {
			angle += rotationSpeed * Time.deltaTime;
			angle = WrapAngle(angle);
			var rotation = Quaternion.Euler(0.0f, 0.0f, angle);
			rb.MoveRotation(rotation);
		}
	}

	static float WrapAngle(float degrees) {
		if (degrees > 360.0f)
			degrees -= 360.0f;
		else if (degrees < 0.0f)
			degrees += 360.0f;
		return degrees;
	}
}
