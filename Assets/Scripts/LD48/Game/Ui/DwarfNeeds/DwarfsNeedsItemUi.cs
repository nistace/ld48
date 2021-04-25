using UnityEngine;
using UnityEngine.UI;
using Utils.Ui;

namespace LD48.Game.Ui {
	public class DwarfsNeedsItemUi : MonoBehaviourUi {
		[SerializeField] protected Image _icon;
		[SerializeField] protected Image _fill;

		public Sprite icon {
			get => _icon.sprite;
			set => _icon.sprite = value;
		}

		public float fill {
			get => _fill.fillAmount;
			set => _fill.fillAmount = value;
		}
	}
}