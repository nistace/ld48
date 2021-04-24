using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utils.Id;

namespace LD48.Game.Data.Constructions {
	[CreateAssetMenu(menuName = "Data/Construction Type")]
	public class ConstructionType : DataScriptableObject {
		public class Event : UnityEvent<ConstructionType> { }

		[SerializeField] protected Construction          _construction;
		[SerializeField] protected string                _displayName;
		[SerializeField] protected Sprite                _sprite;
		[SerializeField] protected int                   _cost;
		[SerializeField] protected int                   _unlockedAfterCount;
		[SerializeField] protected bool                  _placeOverEmpty = true;
		[SerializeField] protected bool                  _placeOverBlock;
		[SerializeField] protected UpgradeConstruction[] _upgrades = { };

		public Construction construction       => _construction;
		public string       displayName        => _displayName;
		public Sprite       sprite             => _sprite;
		public int          cost               => _cost;
		public int          unlockedAfterCount => _unlockedAfterCount;
		public bool         placeOverEmpty     => _placeOverEmpty;
		public bool         placeOverBlock     => _placeOverBlock;

		public bool CanTransform(ConstructionType other, out ConstructionType result) => result = _upgrades.SingleOrDefault(t => t.upgradeType == other)?.upgradeInto;

		[Serializable]
		public class UpgradeConstruction {
			[SerializeField] protected ConstructionType _upgradeType;
			[SerializeField] protected ConstructionType _upgradeInto;

			public ConstructionType upgradeType => _upgradeType;
			public ConstructionType upgradeInto => _upgradeInto;
		}
	}
}