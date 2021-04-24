using LD48;
using UnityEngine;
using Utils.Extensions;
using Utils.Types;

public class LightColorBasedOnCameraY : MonoBehaviour {
	[SerializeField] protected Light      _light;
	[SerializeField] protected ColorRange _colorRange;
	[SerializeField] protected FloatRange _yRange;

	private void OnEnable() => LdCamera.onYChanged.AddListenerOnce(HandleCameraPositionChanged);
	private void OnDisable() => LdCamera.onYChanged.RemoveListener(HandleCameraPositionChanged);

	private void HandleCameraPositionChanged(float y) => _light.color = _colorRange.Lerp(_yRange.RatioOf(y));
}