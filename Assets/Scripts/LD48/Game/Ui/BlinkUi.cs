using UnityEngine;
using UnityEngine.UI;
using Utils.Types;
using Utils.Ui;

namespace LD48.Game.Ui {
	public class BlinkUi : MonoBehaviourUi {
		[SerializeField] protected Image      _image;
		[SerializeField] protected ColorRange _colors;
		[SerializeField] protected float      _speed;

		private void Update() => _image.color = _colors.Lerp((Mathf.Sin(_speed * Time.time) + 1) / 2);
	}
}