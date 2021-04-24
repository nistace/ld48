using UnityEngine;
using UnityEngine.Events;
using Utils.Types;

namespace LD48.Game.Data.Blocks {
	public class Block : MonoBehaviour {
		public class Event : UnityEvent<Block> { }

		[SerializeField] protected BlockType  _type;
		[SerializeField] protected Ratio      _health;
		[SerializeField] protected Vector2Int _coordinates;
		[SerializeField] protected Renderer   _renderer;

		public     BlockType  type        => _type;
		public     Ratio      health      => _health;
		public     Vector2Int coordinates => _coordinates;
		public new Renderer   renderer    => _renderer;

		public static Event onBlockHealthChanged { get; } = new Event();

		public void Init(BlockType type, Vector2Int coordinates) {
			_type = type;
			_coordinates = coordinates;
			SetHealth(1);
			renderer.material = type.material;
		}

		public void Damage(float amount) => SetHealth(_health - amount);

		public void SetHealth(Ratio health) {
			if (_health == health) return;
			_health = health;
			transform.localScale = new Vector3(1, _health, 1);
			onBlockHealthChanged.Invoke(this);
		}
	}
}