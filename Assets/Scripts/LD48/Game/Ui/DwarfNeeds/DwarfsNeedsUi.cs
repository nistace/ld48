using System.Collections.Generic;
using System.Linq;
using LD48.Game.Data.Dwarfs;
using UnityEngine;
using Utils.Extensions;
using Utils.Libraries;
using Utils.Ui;

namespace LD48.Game.Ui {
	public class DwarfsNeedsUi : MonoBehaviourUi {
		[SerializeField] protected DwarfsNeedsItemUi _itemPrefab;
		[SerializeField] protected Vector3           _targetOffset = Vector3.up;

		private Dictionary<Dwarf, DwarfsNeedsItemUi> uiPerDwarf { get; } = new Dictionary<Dwarf, DwarfsNeedsItemUi>();
		private Queue<DwarfsNeedsItemUi>             pool       { get; } = new Queue<DwarfsNeedsItemUi>();

		private void OnEnable() => SetListenersActive(true);
		private void OnDisable() => SetListenersActive(false);

		private void SetListenersActive(bool active) {
			Dwarf.onHasCriticalNeed.SetListenerActive(SetNeed, active);
			Dwarf.onHasNoMoreCriticalNeed.SetListenerActive(ClearNeed, active);
			Dwarf.onDamaged.SetListenerActive(HandleDwarfDamaged, active);
			LdGame.onGameOver.SetListenerActive(ClearAll, active);
		}

		private void ClearAll() => uiPerDwarf.Keys.ToArray().ForEach(ClearNeed);

		private void HandleDwarfDamaged(Dwarf dwarf) {
			if (dwarf.health <= 0) ClearNeed(dwarf);
		}

		private void ClearNeed(Dwarf dwarf) {
			if (!uiPerDwarf.ContainsKey(dwarf)) return;
			pool.Enqueue(uiPerDwarf[dwarf]);
			uiPerDwarf[dwarf].gameObject.SetActive(false);
			uiPerDwarf.Remove(dwarf);
		}

		private void SetNeed(Dwarf dwarf, DwarfNeed need) {
			var existsInDictionary = uiPerDwarf.ContainsKey(dwarf);
			var itemUi = existsInDictionary ? uiPerDwarf[dwarf] : GetNewItemUi();
			if (!existsInDictionary) uiPerDwarf.Add(dwarf, itemUi);
			itemUi.icon = Sprites.Of($"needs.{need}");
			itemUi.fill = dwarf.GetNeedValue(need);
		}

		private DwarfsNeedsItemUi GetNewItemUi() {
			if (pool.Count == 0) return Instantiate(_itemPrefab, transform);
			var itemUi = pool.Dequeue();
			itemUi.gameObject.SetActive(true);
			return itemUi;
		}

		private void Update() {
			uiPerDwarf.ForEach(Refresh);
		}

		private void Refresh(KeyValuePair<Dwarf, DwarfsNeedsItemUi> pair) {
			pair.Value.fill = pair.Key.GetNeedValue(pair.Key.criticalNeed);
			pair.Value.transform.MoveOverWorldTransform(pair.Key.transform, _targetOffset);
		}
	}
}