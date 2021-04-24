using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LD48.Constants;
using LD48.Game.Data.Blocks;
using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	public class Dwarf : MonoBehaviour {
		public class Event : UnityEvent<Dwarf> { }

		private static IReadOnlyList<SurroundingData> surroundingData { get; } = EnumUtils.Values<SurroundingData.Direction>().Select(t => new SurroundingData(t)).ToArray();

		[SerializeField] protected DwarfType _type;
		[SerializeField] protected Rigidbody _rigidbody;
		[SerializeField] protected Renderer  _renderer;
		[SerializeField] protected int       _health;

		private     Transform sTransform       { get; set; }
		private new Transform transform        => sTransform ? sTransform : sTransform = base.transform;
		private     float     startFallingTime { get; set; }

		public static Event onDamaged { get; } = new Event();

		public void Init(DwarfType type) {
			_type = type;
			_health = type.maxHealth;
			_renderer.material = _type.randomMaterial;
			enabled = true;
		}

		private void Start() {
			Init(LdMemory.dwarfTypes.Values.Random());
			StartCoroutine(PlayAi());
		}

		private IEnumerator PlayAi() {
			while (enabled) {
				if (IsFalling()) yield return StartCoroutine(TrackFalling());
				if (!enabled) yield break;
				yield return new WaitForSeconds(1);
				if (IsFalling()) yield return StartCoroutine(TrackFalling());
				if (!enabled) yield break;
				surroundingData.ForEach(t => t.Refresh(transform.position));
				if (TrySolveDecision(surroundingData.Random(t => t.motivation), out var coroutine)) yield return coroutine;
				transform.forward = Vector3.back;
			}
		}

		private bool TrySolveDecision(SurroundingData decision, out Coroutine coroutine) {
			coroutine = null;
			if (decision == null) return false;
			switch (decision.preferredAction) {
				case SurroundingData.Action.Nothing: return false;
				case SurroundingData.Action.Mine:
					coroutine = StartCoroutine(Mine(decision.block));
					return true;
				case SurroundingData.Action.Move:
					coroutine = StartCoroutine(Move(decision.destination));
					return true;
				case SurroundingData.Action.Climb: return false;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private IEnumerator TrackFalling() {
			startFallingTime = Time.time;
			while (IsFalling()) yield return null;
			if (Time.time - startFallingTime < .3f) yield break;
			Damage(Mathf.Pow((Time.time - startFallingTime) * 5, 2).RandomRound());
		}

		private void Damage(int amount) => SetHealth(_health - amount);

		private void SetHealth(int health) {
			if (_health <= 0) return;
			_health = health;
			enabled = _health > 0;
			if (enabled) return;
			onDamaged.Invoke(this);
		}

		private IEnumerator Mine(Block block) {
			var blockPosition = block.transform.position;
			var selfPosition = transform.position;
			transform.forward = Mathf.Abs(blockPosition.y - selfPosition.y) < Mathf.Abs(blockPosition.x - selfPosition.x) ? new Vector3(blockPosition.x - selfPosition.x, 0, 0) : Vector3.back;
			while (block.health > 0 && !IsFalling()) {
				var progress = 0f;
				while (progress < 1) {
					progress += Time.deltaTime * _type.digSpeed;
					yield return null;
				}
				block.Damage(_type.digStrength);
				yield return null;
			}
		}

		private IEnumerator Move(Vector3 destination) {
			transform.forward = new Vector3(destination.x - transform.position.x, 0, 0);
			while (transform.position != destination && !IsFalling()) {
				transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed);
				yield return null;
			}
		}

		private bool IsFalling() => _rigidbody.velocity.y < -.8f;

		private class SurroundingData {
			public enum Direction {
				Up    = 0,
				Left  = 1,
				Right = 2,
				Down  = 3,
			}

			public enum Action {
				Nothing = 0,
				Mine    = 1,
				Move    = 2,
				Climb   = 3
			}

			private Direction direction       { get; }
			public  Action    preferredAction { get; private set; }
			public  float     motivation      { get; private set; }

			public Block   block       { get; private set; }
			public Vector3 destination { get; private set; }

			public SurroundingData(Direction direction) => this.direction = direction;

			public void Refresh(Vector3 currentPosition) {
				var currentCoordinates = World.WorldPointToCoordinates(currentPosition + Vector3.up / 2);
				var destinationCoordinates = GetDestinationCoordinates(currentCoordinates);
				if (!World.InGrid(destinationCoordinates, true)) {
					preferredAction = Action.Nothing;
					motivation = 0;
					return;
				}
				if (World.TryGetBlock(destinationCoordinates, out var blockAtDestination)) {
					preferredAction = Action.Mine;
					block = blockAtDestination;
					motivation = 2 + block.type.goldValue;
					return;
				}
				preferredAction = Action.Move;
				destination = World.CoordinatesToWorldPoint(destinationCoordinates);
				motivation = direction == Direction.Up ? 0 : 1;
			}

			private Vector2Int GetDestinationCoordinates(Vector2Int currentCoordinates) {
				switch (direction) {
					case Direction.Up: return currentCoordinates.With(yFunc: t => t - 1);
					case Direction.Left: return currentCoordinates.With(t => t - 1);
					case Direction.Right: return currentCoordinates.With(t => t + 1);
					case Direction.Down: return currentCoordinates.With(yFunc: t => t + 1);
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}