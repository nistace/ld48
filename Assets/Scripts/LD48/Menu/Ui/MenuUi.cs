using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LD48.Menu.Ui {
	public class MenuUi : MonoBehaviour {
		private static MenuUi instance { get; set; }

		[SerializeField] protected Button _startButton;

		public static UnityEvent onStartButtonClicked => instance._startButton.onClick;

		private void Awake() => instance = this;
	}
}