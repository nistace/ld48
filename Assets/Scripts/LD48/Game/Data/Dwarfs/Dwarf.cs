using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LD48.Game.Data.Blocks;
using LD48.Game.Data.Constructions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Utils.Audio;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	public class Dwarf : MonoBehaviour {
		public class Event : UnityEvent<Dwarf> { }

		public class NeedEvent : UnityEvent<Dwarf, DwarfNeed> { }

		private static IReadOnlyDictionary<Direction, DwarfDirectionStrategy> surroundingData { get; } = EnumUtils.Values<Direction>().ToDictionary(t => t, t => new DwarfDirectionStrategy(t));

		[SerializeField] protected DwarfType     _type;
		[SerializeField] protected DwarfNeeds    _needs = new DwarfNeeds();
		[SerializeField] protected Rigidbody     _rigidbody;
		[SerializeField] protected Renderer      _renderer;
		[SerializeField] protected GameObject    _visualsObject;
		[SerializeField] protected int           _health;
		[SerializeField] protected Block         _miningBlock;
		[SerializeField] protected DwarfAnimator _animator;

		private     Transform  sTransform                 { get; set; }
		private new Transform  transform                  => sTransform ? sTransform : sTransform = base.transform;
		private     float      startFallingTime           { get; set; }
		private     Vector2Int positionBeforeLastDecision { get; set; } = new Vector2Int(-15, -15);
		public      int        health                     => _health;
		public      bool       hasCriticalNeed            { get; private set; }
		public      DwarfNeed  criticalNeed               { get; private set; }

		public static Event     onDamaged               { get; } = new Event();
		public static Event     onStartDigging          { get; } = new Event();
		public static NeedEvent onHasCriticalNeed       { get; } = new NeedEvent();
		public static Event     onHasNoMoreCriticalNeed { get; } = new Event();

		public void Init(DwarfType type) {
			_type = type;
			_needs.Init(type);
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
			_animator.SetDirection((int) (destination.x > transform.position.x ? Direction.Right : Direction.Left));
			_animator.SetSpeed(_type.movementSpeed * 1.5f);
			while (transform.position != destination) {
				transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed * Time.deltaTime);
				yield return null;
			}
			_animator.SetMoving(false);
			_animator.ResetSpeed();
			_animator.TriggerTakePickax();
			AudioManager.Sfx.PlayRandom("dwarf.gemsSign");
			yield return new WaitForSeconds(2);
			_animator.pickax.gameObject.SetActive(true);
			_rigidbody.isKinematic = false;
			onStartDigging.Invoke(this);
			StartCoroutine(PlayAi());
			StartCoroutine(DoUpdateNeeds());
		}

		private IEnumerator DoUpdateNeeds() {
			var progressDamagesFromFood = 0f;
			var progressDamagesFromBeer = 0f;
			while (enabled) {
				_needs.ContinueDecline();
				ProgressTakingDamagesFrom(DwarfNeed.Food, ref progressDamagesFromFood);
				ProgressTakingDamagesFrom(DwarfNeed.Beer, ref progressDamagesFromBeer);
				if (_needs.TryGetCriticalNeed(out var need)) {
					if ((!hasCriticalNeed || criticalNeed != need)) {
						hasCriticalNeed = true;
						criticalNeed = need;
						onHasCriticalNeed.Invoke(this, need);
					}
				}
				else if (hasCriticalNeed) {
					hasCriticalNeed = false;
					onHasNoMoreCriticalNeed.Invoke(this);
				}
				yield return null;
			}
		}

		private void ProgressTakingDamagesFrom(DwarfNeed need, ref float progress) {
			if (_needs[need] > 0) return;
			progress += Time.deltaTime;
			if (progress < 8) return;
			Damage(4);
			progress = 0;
		}

		private IEnumerator PlayAi() {
			while (enabled) {
				if (IsFalling()) yield return StartCoroutine(TrackFalling());
				if (!enabled) yield break;
				yield return new WaitForSeconds(.4f);
				if (IsFalling()) yield return StartCoroutine(TrackFalling());
				if (!enabled) yield break;
				surroundingData.Values.ForEach(t => t.Refresh(transform.position, _needs, positionBeforeLastDecision));
				SaveCurrentPositionAsBeforeLastDecision();
				if (TrySolveDecision(surroundingData.Values.Where(t => t.motivation > 0).ToArray().Random(t => t.motivation), out var coroutine)) yield return coroutine;
				transform.forward = Vector3.back;
			}
		}

		private void SaveCurrentPositionAsBeforeLastDecision() => positionBeforeLastDecision = World.WorldPointToCoordinates(transform.position + Vector3.up / 2);

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
				case DwarfDirectionStrategy.Action.Climb:
					coroutine = StartCoroutine(Climb(decision.destination));
					return true;
				case DwarfDirectionStrategy.Action.Use:
					coroutine = StartCoroutine(Use(decision.restorationPlaceToUse));
					return true;
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

		public void Damage(int amount) => SetHealth(_health - amount);

		private void SetHealth(int health) {
			if (_health <= 0) return;
			_health = health;
			enabled = _health > 0;
			onDamaged.Invoke(this);
			AudioManager.Sfx.PlayRandom(_health <= 0 ? "dwarf.die" : "dwarf.damaged");
			if (_health <= 0) _animator.TriggerDie();
		}

		private IEnumerator Mine(Block block) {
			_miningBlock = block;
			var blockPosition = block.transform.position;
			var selfPosition = transform.position;
			transform.forward = Mathf.Abs(blockPosition.y - selfPosition.y) < Mathf.Abs(blockPosition.x - selfPosition.x) ? new Vector3(blockPosition.x - selfPosition.x, 0, 0) : Vector3.back;
			_animator.SetMining(true);
			_animator.SetSpeed(_type.digSpeed);
			var abort = false;
			while (!abort && block.health > 0 && !IsFalling()) {
				if (World.TryGetConstruction(block.coordinates, out var construction) && construction.type.preventsFromMining) abort = true;
				yield return null;
			}
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
			_animator.SetSpeed(_type.movementSpeed * 1.5f);
			var position = transform.position;
			transform.forward = new Vector3(destination.x - position.x, 0, 0);
			var checkTreadWater = 0;
			var previousPosition = position;
			while (checkTreadWater < 10 && transform.position.x != destination.x && !IsFalling()) {
				transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed * Time.deltaTime);
				if (transform.position != previousPosition) {
					checkTreadWater = 0;
					previousPosition = transform.position;
				}
				else checkTreadWater++;
				yield return null;
			}
			_animator.SetMoving(false);
			_animator.ResetSpeed();
		}

		private IEnumerator Use(RestorationPlace place) {
			place.AddPersonInside();
			_visualsObject.gameObject.SetActive(false);
			_needs.declineFrozen = true;
			_rigidbody.isKinematic = true;
			AudioManager.Sfx.PlayRandom("door");
			AudioManager.Sfx.PlayRandom($"dwarf.{place.need}");
			while (place.Restore(_needs)) yield return null;
			_visualsObject.gameObject.SetActive(true);
			_needs.declineFrozen = false;
			_rigidbody.isKinematic = false;
			AudioManager.Sfx.PlayRandom("door");
			AudioManager.Sfx.PlayRandom("dwarf.restored");
			place.RemovePersonInside();
		}

		private IEnumerator Climb(Vector3 destination) {
			var direction = destination.y < transform.position.y ? Direction.Down : Direction.Up;
			_animator.SetMoving(true);
			_animator.SetSpeed(_type.movementSpeed * 1.5f);
			transform.forward = Vector3.forward;
			_rigidbody.isKinematic = true;
			var continueClimb = true;
			while (!IsFalling() && continueClimb) {
				while (transform.position != destination && !IsFalling()) {
					transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed * Time.deltaTime);
					yield return null;
				}
				if (World.TryGetConstruction(World.WorldPointToCoordinates(transform.position - Vector3.up * .5f), out var construction) && construction.type.canStandOver) continueClimb = false;
				else {
					surroundingData[direction].Refresh(transform.position, _needs, positionBeforeLastDecision);
					SaveCurrentPositionAsBeforeLastDecision();
					continueClimb = surroundingData[direction].preferredAction == DwarfDirectionStrategy.Action.Climb && surroundingData[direction].motivation >= 0;
					destination = surroundingData[direction].destination;
				}
			}

			_rigidbody.isKinematic = false;
			_animator.SetMoving(false);
			_animator.ResetSpeed();
		}

		private bool IsFalling() => _rigidbody.velocity.y < -3f;
		public float GetNeedValue(DwarfNeed need) => _needs[need];
#if UNITY_EDITOR
		private void OnDrawGizmos() {
			Handles.color = Color.white;
			surroundingData.Values.ForEach(t => Handles.Label(World.CoordinatesToWorldPoint(t.destinationCoordinates), $"{t.motivation}"));
		}
#endif
	}
}