using System.Collections;
using LD48.Game.Data.Constructions;
using LD48.Game.Ui;
using LD48.Input;
using TDDG.Scenes.Game.States;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils.Extensions;
using Utils.StaticUtils;

namespace LD48.Game {
	public class PlaceConstructionState : AbstractState {
		private ConstructionType type            { get; }
		private Construction     ghost           { get; }
		private bool             isPositionValid { get; set; }
		private Vector2Int       position        { get; set; }

		public PlaceConstructionState(ConstructionType type) {
			this.type = type;
			ghost = Object.Instantiate(type.construction);
			ghost.Ghostify();

			RefreshGhost();
		}

		protected override void Enable() {
			routine.Start(DoRefreshGhost());
		}

		private IEnumerator DoRefreshGhost() {
			while (enabled) {
				RefreshGhost();
				yield return null;
			}
		}

		private void RefreshGhost() {
			if (EventSystem.current.IsPointerOverGameObject() || !Physics.Raycast(LdCamera.camera.ScreenPointToRay(Inputs.controls.Game.MousePosition.ReadValue<Vector2>()), out var hit, 20,
					LayerMask.GetMask("Block", "Background"))) {
				isPositionValid = false;
				DiffSet.ActiveGameObject(ghost, false);
				return;
			}
			DiffSet.ActiveGameObject(ghost, true);
			position = World.WorldPointToCoordinates(hit.point);
			isPositionValid = CheckPosition();
			ghost.transform.position = isPositionValid ? World.CoordinatesToWorldPoint(position) : hit.point + Vector3.back;
			ghost.GetComponentsInChildren<Renderer>().ForEach(t => t.material.color = isPositionValid ? Color.green : Color.red);
		}

		private bool CheckPosition() {
			if (LdGame.gold < type.cost) return false;
			if (!World.InGrid(position, false)) return false;
			var canPlaceThere = !World.IsThereAnythingAt(position, out var block, out var construction) && type.placeOverEmpty || block && type.placeOverBlock ||
										construction && type.CanTransform(construction.type, out _);
			if (!canPlaceThere) return false;
			return true;
		}

		protected override void Disable() {
			routine.Stop();
			Object.Destroy(ghost.gameObject);
		}

		protected override void SetInputListenersEnabled(bool enabled) {
			Inputs.controls.Game.Cancel.SetPerformListenerOnce(Cancel, enabled);
			Inputs.controls.Game.Cancel.SetEnabled(enabled);
			Inputs.controls.Game.Interact.SetPerformListenerOnce(PlaceConstruction, enabled);
			Inputs.controls.Game.Interact.SetEnabled(enabled);
		}

		protected override void SetUiListenersEnabled(bool enabled) {
			ConstructionButtonUi.onConstructionClicked.SetListenerActive(HandleSelectOtherConstruction, enabled);
			LdGame.onGoldChanged.SetListenerActive(HandleGoldChanged, enabled);
		}

		private void HandleGoldChanged(int amount) {
			if (!isPositionValid && amount > type.cost) RefreshGhost();
		}

		private void PlaceConstruction(InputAction.CallbackContext obj) {
			if (!isPositionValid) return;
			LdGame.PayGold(type.cost);
			World.CreateConstruction(type, position);
			RefreshGhost();
		}

		private static void Cancel(InputAction.CallbackContext obj) => ChangeState(DefaultState.instance);

		private void HandleSelectOtherConstruction(ConstructionType construction) {
			if (construction != type) ChangeState(new PlaceConstructionState(construction));
		}
	}
}