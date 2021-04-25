using UnityEngine;
using Utils.Id;

namespace LD48.Game.Data.Blocks {
	[CreateAssetMenu(menuName = "Data/Block Type")]
	public class BlockType : DataScriptableObject {
		[SerializeField] protected Material _material;
		[SerializeField] protected int      _goldValue;
		[SerializeField] protected int      _minDepth;
		[SerializeField] protected float    _frequency = 1;
		[SerializeField] protected bool     _explodesWhenDestroyed;
		[SerializeField] protected int      _radiusOfExplosion = 1;

		public Material material              => _material;
		public int      goldValue             => _goldValue;
		public int      minDepth              => _minDepth;
		public float    frequency             => _frequency;
		public bool     explodesWhenDestroyed => _explodesWhenDestroyed;
		public int      radiusOfExplosion     => _radiusOfExplosion;
	}
}