using ECM.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CharacterController2D : BaseCharacterController {

	private bool faceMoveDirection = false;
	private bool orientUpInAir = true;
	private bool jumped = false;

	protected override void Animate() {
	}

	protected override void HandleInput() {
		if (Input.GetKeyDown(KeyCode.P)) {
			pause = !pause;
		}

		if (Input.GetKeyDown(KeyCode.Tab)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		jump = Input.GetButton("Jump");

		crouch = Input.GetKey(KeyCode.C);

		moveDirection = new Vector3 {
			x = Input.GetAxisRaw("Horizontal"),
			y = 0.0f,
			z = Input.GetAxisRaw("Vertical")
		};
	}

	protected override void Jump() {
		bool jumping = isJumping;
		base.Jump();
		if (!jumping && isJumping) {
			SoundManager.inst.PlayJump();
		}
	}

	public override void FixedUpdate() {

		bool wasFalling = isFalling || isJumping;

		base.FixedUpdate();

		if (wasFalling && movement.isGrounded) {
			// landed
			Debug.Log("landed");
			SoundManager.inst.PlayJump();
		}
	}

	protected override void UpdateRotation() {
		if (faceMoveDirection) {
			RotateTowardsMoveDirection();
		}

		Vector3 surfaceNormal;

		if (!movement.isGrounded && orientUpInAir) {
			surfaceNormal = Vector3.up;
		} else {
			var castOrigin = transform.TransformPoint(movement.capsuleCollider.center);
			var castDirection = -transform.up;
			var castDistance = 2.0f;
			var castMask = movement.groundMask;

			RaycastHit hitInfo;
			bool hit = Physics.Raycast(castOrigin, castDirection, out hitInfo, castDistance, castMask);

			surfaceNormal = hit ? hitInfo.normal : movement.surfaceNormal;
		}

		var characterUp = transform.up;
		Vector3 smoothedNormal = Vector3.Slerp(characterUp, surfaceNormal, 20.0f * Time.deltaTime);


		movement.rotation = Quaternion.FromToRotation(characterUp, smoothedNormal) * movement.rotation;

		// TODO: Ensure the rotation is flat on the ground
		//Debug.Log($"Rotation: {characterUp}, {surfaceNormal}, {smoothedNormal} = {movement.rotation}");
	}
}
