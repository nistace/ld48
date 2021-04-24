using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LD48.Game.Data.Blocks;
using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	public class Dwarf : MonoBehaviour {
		public class Event : UnityEvent<Dwarf> { }

		private static IReadOnlyList<DwarfDirectionStrategy> surroundingData { get; } = EnumUtils.Values<DwarfDirectionStrategy.Direction>().Select(t => new DwarfDirectionStrategy(t)).ToArray();

		[SerializeField] protected DwarfType     _type;
		[SerializeField] protected Rigidbody     _rigidbody;
		[SerializeField] protected Renderer      _renderer;
		[SerializeField] protected int           _health;
		[SerializeField] protected Block         _miningBlock;
		[SerializeField] protected DwarfAnimator _animator;

		private     Transform sTransform       { get; set; }
		private new Transform transform        => sTransform ? sTransform : sTransform = base.transform;
		private     float     startFallingTime { get; set; }
		public      int       health           => _health;

		public static Event onDamaged      { get; } = new Event();
		public static Event onStartDigging { get; } = new Event();

		public void Init(DwarfType type) {
			_type = type;
			_health = type.maxHealth;
			_renderer.material = _type.randomMaterial;
			enabled = true;
		}

		private void Start() => _animator.onMiningImpact.AddListenerOnce(DoMiningImpact);

		public void WalkUpToTheSign(Vector3 spawn, Vector3 destination) {
			transform.position = spawn;
			_rigidbody.isKinematic = true;
			transform.forward = new Vector3(destination.x - spawn.x, 0, 0);
			StartCoroutine(DoWalkUpToTheSignThenStartAi(destination));
		}

		private IEnumerator DoWalkUpToTheSignThenStartAi(Vector3 destination) {
			_animator.SetMoving(true);
			_animator.SetDirection((int) (destination.x > transform.position.x ? DwarfDirectionStrategy.Direction.Right : DwarfDirectionStrategy.Direction.Left));
			_animator.SetSpeed(_type.movementSpeed * 200);
			while (transform.position != destination) {
				transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed);
				yield return null;
			}
			_animator.SetMoving(false);
			_animator.ResetSpeed();
			_animator.TriggerTakePickax();
			yield return new WaitForSeconds(2);
			_animator.pickax.gameObject.SetActive(true);
			_rigidbody.isKinematic = false;
			onStartDigging.Invoke(this);
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

		private bool TrySolveDecision(DwarfDirectionStrategy decision, out Coroutine coroutine) {
			coroutine = null;
			if (decision == null) return false;
			_animator.SetDirection((int) decision.direction);
			switch (decision.preferredAction) {
				case DwarfDirectionStrategy.Action.Nothing: return false;
				case DwarfDirectionStrategy.Action.Mine:
					coroutine = StartCoroutine(Mine(decision.block));
					return true;
				case DwarfDirectionStrategy.Action.Move:
					coroutine = StartCoroutine(Move(decision.destination));
					return true;
				case DwarfDirectionStrategy.Action.Climb: return false;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private IEnumerator TrackFalling() {
			_animator.SetFalling(true);
			startFallingTime = Time.time;
			while (IsFalling()) yield return null;
			_animator.SetFalling(false);
			if (Time.time - startFallingTime < .3f) yield break;
			Damage(Mathf.Pow((Time.time - startFallingTime) * 5, 2).RandomRound());
		}

		private void Damage(int amount) => SetHealth(_health - amount);

		private void SetHealth(int health) {
			if (_health <= 0) return;
			_health = health;
			enabled = _health > 0;
			onDamaged.Invoke(this);
			if (_health <= 0) _animator.TriggerDie();
		}

		private IEnumerator Mine(Block block) {
			_miningBlock = block;
			var blockPosition = block.transform.position;
			var selfPosition = transform.position;
			transform.forward = Mathf.Abs(blockPosition.y - selfPosition.y) < Mathf.Abs(blockPosition.x - selfPosition.x) ? new Vector3(blockPosition.x - selfPosition.x, 0, 0) : Vector3.back;
			_animator.SetMining(true);
			_animator.SetSpeed(_type.digSpeed);
			while (block.health > 0 && !IsFalling()) yield return null;
			_miningBlock = null;
			_animator.SetMining(false);
			_animator.ResetSpeed();
		}

		private void DoMiningImpact() {
			if (!_miningBlock) return;
			_miningBlock.Damage(_type.digStrength);
		}

		private IEnumerator Move(Vector3 destination) {
			_animator.SetMoving(true);
			_animator.SetSpeed(_type.movementSpeed * 200);
			transform.forward = new Vector3(destination.x - transform.position.x, 0, 0);
			while (transform.position != destination && !IsFalling()) {
				transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed);
				yield return null;
			}
			_animator.SetMoving(false);
			_animator.ResetSpeed();
		}

		private bool IsFalling() => _rigidbody.velocity.y < -3f;
	}
}