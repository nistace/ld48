using UnityEngine;
using Utils.Id;

namespace LD48.Game.Data.Blocks {
	[CreateAssetMenu(menuName = "Data/Block Type")]
	public class BlockType : DataScriptableObject {
		[SerializeField] protected Material _material;
		[SerializeField] protected int      _goldValue;
		[SerializeField] protected int      _minDepth;
		[SerializeField] protected int      _maxDepth;
		[SerializeField] protected float    _frequency = 1;

		public Material material  => _material;
		public int      goldValue => _goldValue;
		public int      minDepth  => _minDepth;
		public int      maxDepth  => _maxDepth;
		public float    frequency => _frequency;
	}
}