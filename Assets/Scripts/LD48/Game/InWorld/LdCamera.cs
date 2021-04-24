using LD48.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.Events;
using Utils.Extensions;

namespace LD48 {
	public class LdCamera : MonoBehaviour {
		private static    LdCamera instance { get; set; }
		public new static Camera   camera   => instance._camera;

		[SerializeField] protected Camera _camera;
		[SerializeField] protected float  _maxY;
		[SerializeField] protected float  _minY;
		[SerializeField] protected float  _speed;

		public static FloatEvent onYChanged { get; } = new FloatEvent();

		public static void SetMinY(int minY) => instance._minY = minY;

		private void Awake() => instance = this;

		private void Start() {
			Inputs.controls.Game.Scroll.Enable();
		}

		private void Update() {
			if (EventSystem.current.IsPointerOverGameObject()) return;
			if (Inputs.controls.Game.Scroll.ReadValue<float>() == 0) return;
			var newY = (transform.position.y + Inputs.controls.Game.Scroll.ReadValue<float>() * Time.deltaTime * _speed).Clamp(_minY, _maxY);
			if (newY == transform.position.y) return;
			transform.position = transform.position.With(y: newY);
			onYChanged.Invoke(newY);
		}
	}
}