using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour {
	void Start() {

	}

	void Update() {

	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}	
	}
}
