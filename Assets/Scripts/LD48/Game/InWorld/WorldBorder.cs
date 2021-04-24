using UnityEngine;
using Utils.Extensions;

namespace LD48 {
	public class WorldBorder : MonoBehaviour {
		[SerializeField] protected int   _maxY;
		[SerializeField] protected float _borderStep = 6;

		private void OnEnable() => LdCamera.onYChanged.AddListenerOnce(HandleCameraPositionChanged);
		private void OnDisable() => LdCamera.onYChanged.RemoveListener(HandleCameraPositionChanged);

		private void HandleCameraPositionChanged(float y) {
			var depth = Mathf.Min(_maxY, Mathf.CeilToInt(y / _borderStep) + 1);
			transform.position = new Vector3(0, depth * _borderStep, 0);
		}
	}
}