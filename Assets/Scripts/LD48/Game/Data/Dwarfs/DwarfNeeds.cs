using System;
using System.Linq;
using UnityEngine;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	[Serializable]
	public class DwarfNeeds : IReadDwarfNeeds {
		private static bool[] activeNeeds { get; set; } = { };

		public static void SetNeedActive(DwarfNeed need, bool active) {
			if (activeNeeds.Length <= (int) need) activeNeeds = activeNeeds.WithLength(EnumUtils.SizeOf<DwarfNeed>());
			activeNeeds[(int) need] = active;
		}

		private static bool IsNeedActive(DwarfNeed need) => activeNeeds.GetSafe((int) need);

		[SerializeField] protected IDwarfNeedsData _data;
		[SerializeField] protected float[]         _needsFill = { };
		[SerializeField] protected bool            _declineFrozen;

		public float this[DwarfNeed need] {
			get => _needsFill.GetSafe((int) need);
			private set {
				if (_needsFill.Length <= (int) need) _needsFill = _needsFill.WithLength(EnumUtils.SizeOf<DwarfNeed>());
				_needsFill[(int) need] = value.Clamp(0, 1);
			}
		}

		public bool declineFrozen {
			get => _declineFrozen;
			set => _declineFrozen = value;
		}

		public void Init(IDwarfNeedsData data) {
			_data = data;
			EnumUtils.Values<DwarfNeed>().ForEach(t => this[t] = 1);
		}

		public int GetAdditionalMotivationToGo(Direction direction) {
			switch (direction) {
				case Direction.Up: return ((.8f - Mathf.Min(_needsFill)) * 10).RandomRound();
				case Direction.Down: return ((Mathf.Min(_needsFill) - .5f) * 10).RandomRound();
				case Direction.Left: return 0;
				case Direction.Right: return 0;
				case Direction.Self: return 0;
				default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public int GetAdditionalMotivationToUse(DwarfNeed need) => ((.8f - this[need]) * 10).RandomRound();
		public void Restore(DwarfNeed need, float amount) => this[need] += amount;

		public void ContinueDecline() {
			if (_declineFrozen) return;
			EnumUtils.Values<DwarfNeed>().Where(IsNeedActive).ForEach(t => this[t] -= Time.deltaTime * _data.GetDeclineSpeed(t));
		}

		public bool TryGetCriticalNeed(out DwarfNeed dwarfNeed) {
			dwarfNeed = DwarfNeed.Sleep;
			var min = 1f;
			for (var i = 0; i < _needsFill.Length; ++i) {
				if (_needsFill[i] >= min) continue;
				min = _needsFill[i];
				dwarfNeed = (DwarfNeed) i;
			}
			return min < .3f;
		}
	}
}