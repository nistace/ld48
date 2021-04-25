using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.Ui;

namespace LD48.Game.Ui {
	public class GameOverUi : MonoBehaviourUi {
		[SerializeField] protected TMP_Text _endStats;
		[SerializeField] protected Button   _endButton;

		public UnityEvent onEndButtonClicked => _endButton.onClick;

		public void Show(int depth, int blocksDug) {
			_endStats.text = $"The dwarfs have been digging to a depth of {depth}, clearing {blocksDug} blocks of mud and minerals.";
			gameObject.SetActive(true);
		}
	}
}