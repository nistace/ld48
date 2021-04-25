using LD48.Game.Data.Dwarfs;
using UnityEngine;

namespace LD48.Game.Data.Constructions {
	[RequireComponent(typeof(Construction))]
	public class RestorationPlace : MonoBehaviour {
		[SerializeField] protected DwarfNeed _need;
		[SerializeField] protected float     _restorationPerSecond;

		private Construction sConstruction        { get; set; }
		public  Construction construction         => sConstruction ? sConstruction : sConstruction = GetComponent<Construction>();
		public  DwarfNeed    need                 => _need;
		public  float        restorationPerSecond => _restorationPerSecond;

		public bool Restore(DwarfNeeds needs) {
			needs.Restore(need, restorationPerSecond * Time.deltaTime);
			return needs[need] < 1;
		}
	}
}