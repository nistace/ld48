using UnityEngine;
using Utils.Extensions;

namespace LD48.Game.Data.Constructions {
	public class Construction : MonoBehaviour {
		[SerializeField] protected GameObject[] _inactiveGhostObjects;

		public void Ghostify() {
			_inactiveGhostObjects.ForEach(t => t.SetActive(false));
			GetComponentsInChildren<Collider>().ForEach(t => Destroy(t));
		}
	}
}