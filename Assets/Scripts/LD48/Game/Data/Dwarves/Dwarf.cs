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
			new SurroundingData(new Vector3(0, -.5f, 0)), new SurroundingData(new Vector3(.75f, .5f, 0)), new SurroundingData(new Vector3(-.75f, .5f, 0))
		};

		[SerializeField] protected DwarfType _type;
		[SerializeField] protected Rigidbody _rigidbody;
		[SerializeField] protected Renderer  _renderer;

		private     Transform sTransform { get; set; }
		private new Transform transform  => sTransform ? sTransform : sTransform = base.transform;

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
				transform.forward = Vector3.back;
				yield return new WaitForSeconds(1);
			}
		}

		private IEnumerator Mine(Block block) {
			if (block.transform.position.y == transform.position.y) transform.forward = block.transform.position - transform.position;
			else transform.forward = Vector3.back;
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
			while (transform.position != destination && !IsFalling()) {
				transform.position = Vector3.MoveTowards(transform.position, destination, _type.movementSpeed);
				yield return null;
			}
		}

		private bool IsFalling() => _rigidbody.velocity.y < -.8f;

		private class SurroundingData {
			private Vector3 relativePosition { get; }
			private bool    inGrid           { get; set; }
			private bool    somethingThere   { get; set; }
			public  Block   block            { get; private set; }
			public  Vector3 destination      { get; private set; }
			public  bool    canMove          => inGrid && !somethingThere;
			public  bool    canMine          => block;

			public SurroundingData(Vector3 relativePosition) => this.relativePosition = relativePosition;

			public void Refresh(Vector3 currentPosition) {
				inGrid = World.InGrid(currentPosition + relativePosition);
				somethingThere = inGrid && Physics.OverlapSphereNonAlloc(currentPosition + relativePosition, .25f, nonAllocSingleCollider, LayerMask.GetMask("Block")) > 0;
				block = somethingThere ? nonAllocSingleCollider[0].gameObject.GetComponentInParent<Block>() : null;
				if (canMove) destination = currentPosition.With(t => (t + relativePosition.x).Floor() + .5f);
			}
		}
	}
}