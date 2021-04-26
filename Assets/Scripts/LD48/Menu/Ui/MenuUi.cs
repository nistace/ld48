using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.Audio;
using Utils.Extensions;

namespace LD48.Menu.Ui {
	public class MenuUi : MonoBehaviour {
		private static MenuUi instance { get; set; }

		[SerializeField] protected Button _startButton;
		[SerializeField] protected Slider _musicSlider;
		[SerializeField] protected Slider _sfxSlider;

		public static UnityEvent onStartButtonClicked => instance._startButton.onClick;

		private void Awake() {
			instance = this;
			_musicSlider.onValueChanged.AddListenerOnce(t => AudioManager.Music.volume = t);
			_sfxSlider.onValueChanged.AddListenerOnce(t => AudioManager.Sfx.volume = t);
		}

		private void Start() {
			_musicSlider.SetValueWithoutNotify(AudioManager.Music.volume);
			_sfxSlider.SetValueWithoutNotify(AudioManager.Sfx.volume);
		}
	}
}