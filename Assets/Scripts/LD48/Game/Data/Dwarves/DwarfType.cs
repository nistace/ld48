using UnityEngine;
using Utils.Id;

namespace LD48.Game.Data.Dwarfs {
	[CreateAssetMenu(menuName = "Data/Dwarf Type")]
	public class DwarfType : DataScriptableObject {
		[SerializeField] protected float _digSpeed    = .5f;
		[SerializeField] protected float _digStrength = .2f;

		public float digSpeed    => _digSpeed;
		public float digStrength => _digStrength;
	}
}