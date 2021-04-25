using UnityEngine;
using Utils.Libraries;

namespace LD48.Game {
	public class LdApp : MonoBehaviour {
		private static LdApp instance { get; set; }

		private void Awake() {
			if (instance) Destroy(gameObject);
			else {
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
		}

		private void Start() {
			AudioClips.LoadLibrary(Resources.Load<AudioClipLibrary>("Libraries/Audio"));
			Particles.LoadLibrary(Resources.Load<ParticlesLibrary>("Libraries/Particles"));
		}
	}
}