using UnityEngine;
using Utils.Extensions;
using Utils.Id;

namespace LD48.Game.Data.Dwarfs {
	[CreateAssetMenu(menuName = "Data/Dwarf Type")]
	public class DwarfType : DataScriptableObject {
		[SerializeField] protected Material[] _materials;
		[SerializeField] protected int        _maxHealth     = 20;
		[SerializeField] protected float      _digSpeed      = .5f;
		[SerializeField] protected float      _digStrength   = .2f;
		[SerializeField] protected float      _movementSpeed = .1f;

		public float    digSpeed       => _digSpeed;
		public float    digStrength    => _digStrength;
		public Material randomMaterial => _materials.Random();
		public float    movementSpeed  => _movementSpeed;
		public int      maxHealth      => _maxHealth;
	}
}