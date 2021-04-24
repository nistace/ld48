using LD48.Input;
using UnityEngine;
using Utils.Events;
using Utils.Extensions;

namespace LD48 {
	public class LdCamera : MonoBehaviour {
		public static LdCamera instance { get; private set; }

		[SerializeField] protected float _maxY;
		[SerializeField] protected float _minY;
		[SerializeField] protected float _speed;

		public static FloatEvent onYChanged { get; } = new FloatEvent();

		private     Transform sTransform { get; set; }
		private new Transform transform  => sTransform ? sTransform : sTransform = base.transform;

		public static void SetMinY(int minY) => instance._minY = minY;

		private void Awake() => instance = this;

		private void Start() {
			Inputs.controls.Camera.Scroll.Enable();
		}

		private void Update() {
			if (Inputs.controls.Camera.Scroll.ReadValue<float>() == 0) return;
			var newY = (transform.position.y + Inputs.controls.Camera.Scroll.ReadValue<float>() * Time.deltaTime * _speed).Clamp(_minY, _maxY);
			if (newY == transform.position.y) return;
			transform.position = transform.position.With(y: newY);
			onYChanged.Invoke(newY);
		}
	}
}