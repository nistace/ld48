using UnityEngine;

public class LdApp : MonoBehaviour {
	private static LdApp instance { get; set; }

	private void Awake() {
		if (instance) Destroy(gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}