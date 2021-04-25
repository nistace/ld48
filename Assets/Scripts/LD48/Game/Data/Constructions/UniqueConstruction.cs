using UnityEngine;

namespace LD48.Game.Data.Constructions {
	[RequireComponent(typeof(Construction))]
	public class UniqueConstruction : MonoBehaviour {
		[SerializeField] protected ConstructionType _type;

		private void Start() => GetComponent<Construction>().Init(_type);
	}
}