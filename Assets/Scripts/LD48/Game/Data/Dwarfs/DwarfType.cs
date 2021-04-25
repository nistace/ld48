using System;
using UnityEngine;
using Utils.Extensions;
using Utils.Id;

namespace LD48.Game.Data.Dwarfs {
	[CreateAssetMenu(menuName = "Data/Dwarf Type")]
	public class DwarfType : DataScriptableObject, IDwarfNeedsData {
		[SerializeField] protected Material[] _materials;
		[SerializeField] protected int        _maxHealth     = 20;
		[SerializeField] protected float      _digSpeed      = .5f;
		[SerializeField] protected float      _digStrength   = .2f;
		[SerializeField] protected float      _movementSpeed = .1f;
		[SerializeField] protected float      _sleepDecline  = .01f;
		[SerializeField] protected float      _foodDecline   = .021f;
		[SerializeField] protected float      _beerDecline   = .043f;

		public float    digSpeed       => _digSpeed;
		public float    digStrength    => _digStrength;
		public Material randomMaterial => _materials.Random();
		public float    movementSpeed  => _movementSpeed;
		public int      maxHealth      => _maxHealth;

		public float GetDeclineSpeed(DwarfNeed need) {
			switch (need) {
				case DwarfNeed.Sleep: return _sleepDecline;
				case DwarfNeed.Food: return _foodDecline;
				case DwarfNeed.Beer: return _beerDecline;
				default: throw new ArgumentOutOfRangeException(nameof(need), need, null);
			}
		}
	}
}