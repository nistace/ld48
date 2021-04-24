using LD48.Game.Data.Constructions;
using LD48.Game.Ui;
using TDDG.Scenes.Game.States;
using Utils.Extensions;

namespace LD48.Game {
	public class DefaultState : AbstractState {
		public static DefaultState instance { get; } = new DefaultState();

		private DefaultState() { }

		protected override void Enable() { }

		protected override void Disable() { }

		protected override void SetInputListenersEnabled(bool enabled) { }

		protected override void SetUiListenersEnabled(bool enabled) {
			ConstructionButtonUi.onConstructionClicked.SetListenerActive(HandlePlaceConstruction, enabled);
		}

		private static void HandlePlaceConstruction(ConstructionType construction) => ChangeState(new PlaceConstructionState(construction));
	}
}