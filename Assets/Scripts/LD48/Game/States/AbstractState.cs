using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Coroutines;
using Utils.Loading;

namespace TDDG.Scenes.Game.States {
	public abstract class AbstractState {
		private static   AbstractState   currentState { get; set; }
		protected static SingleCoroutine routine      { get; private set; }

		protected bool enabled { get; private set; }

		public static void Init(MonoBehaviour routineRunner, AbstractState initialState) {
			routine = new SingleCoroutine(routineRunner);
			ChangeState(initialState);
		}

		protected abstract void Enable();
		protected abstract void Disable();
		protected abstract void SetInputListenersEnabled(bool enabled);
		protected abstract void SetUiListenersEnabled(bool enabled);

		public static void ChangeState(AbstractState state) {
			if (currentState != null) {
				currentState.enabled = false;
				currentState.Disable();
				currentState.SetInputListenersEnabled(false);
				currentState.SetUiListenersEnabled(false);
			}
			routine.Stop();
			currentState = state;
			if (currentState == null) return;
			currentState.enabled = true;
			currentState.Enable();
			currentState.SetInputListenersEnabled(true);
			currentState.SetUiListenersEnabled(true);
		}

		public static void Terminate() {
			currentState?.SetInputListenersEnabled(false);
		}
	}
}