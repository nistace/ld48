using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LD48.Constants;
using LD48.Game.Data.Blocks;
using UnityEngine;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	public class Dwarf : MonoBehaviour {
		private static Collider[] nonAllocSingleCollider { get; } = new Collider[1];

		private static IEnumerable<SurroundingData> surroundingData { get; } = new[] {
			new SurroundingData(Vector3.zero), new SurroundingData(new Vector3(.75f, .5f, 0)), new SurroundingData(new Vector3(-.75f, .5f, 0))
		};

		[SerializeField] protected DwarfType _type;
		[SerializeField] protected Rigidbody _rigidbody;
		[SerializeField] protected Renderer  _renderer;

		public void Init(DwarfType type) {
			_type = type;
			_renderer.material = _type.randomMaterial;
		}

		private void Start() {
			Init(LdMemory.dwarfTypes.Values.Random());
			StartCoroutine(PlayAi());
		}

		private IEnumerator PlayAi() {
			while (enabled) {
				surroundingData.ForEach(t => t.Refresh(transform.position));
				if (surroundingData.Where(t => t.canMine || t.canMove).TryRandom(out var decision)) {
					if (decision.canMine) yield return StartCoroutine(Mine(decision.block));
					else if (decision.canMove) yield return StartCoroutine(Move(decision.destination));
				}
				yield return new WaitForSeconds(1);
			}
		}

		private IEnumerator Mine(Block block) {
			while (block.health > 0) {
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
			while (transform.position != destination && _rigidbody.velocity.sqrMagnitude < .5f) {
				transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed);
				yield return null;
			}
		}

		private class SurroundingData {
			public Vector3 relativePosition { get; }
			public bool    somethingThere   { get; private set; }
			public Block   block            { get; private set; }
			public Vector3 destination      { get; private set; }
			public bool    canMove          => !somethingThere;
			public bool    canMine          => block;

			public SurroundingData(Vector3 relativePosition) => this.relativePosition = relativePosition;

			public void Refresh(Vector3 currentPosition) {
				somethingThere = Physics.OverlapSphereNonAlloc(currentPosition + relativePosition, .25f, nonAllocSingleCollider, LayerMask.GetMask("Block", "Border")) > 0;
				block = somethingThere ? nonAllocSingleCollider[0].gameObject.GetComponentInParent<Block>() : null;
				if (canMove) destination = currentPosition.With(t => (t + relativePosition.x).Floor() + .5f);
			}
		}
	}
}