using UnityEngine;
using Utils.Extensions;

namespace LD48.Game.Data.Constructions {
	public class Construction : MonoBehaviour {
		[SerializeField] protected GameObject[] _inactiveGhostObjects;

		public ConstructionType type { get; set; }

		public void Init(ConstructionType type) {
			this.type = type;
		}

		public void Ghostify() {
			_inactiveGhostObjects.ForEach(t => t.SetActive(false));
			GetComponentsInChildren<Collider>().ForEach(t => Destroy(t));
		}
	}
}