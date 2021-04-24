using System.Collections;
using System.Collections.Generic;
using LD48.Game.Data.Blocks;
using UnityEngine;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	public class Dwarf : MonoBehaviour {
		private IEnumerable<Vector3> surroundingPositionsToCheck { get; } = new[] {Vector3.zero, new Vector3(.5f, .5f, 0), new Vector3(-.5f, .5f, 0)};

		[SerializeField] protected DwarfType _type;

		private static Collider[] nonAllocSingleCollider { get; } = new Collider[1];

		private void Start() {
			StartCoroutine(PlayAi());
		}

		private IEnumerator PlayAi() {
			while (enabled) {
				if (Physics.OverlapSphereNonAlloc(transform.position + surroundingPositionsToCheck.Random(), .3f, nonAllocSingleCollider) == 1) {
					if (nonAllocSingleCollider[0].gameObject.TryGetComponentInParent<Block>(out var block)) {
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
				}
				yield return new WaitForSeconds(1);
			}
		}
	}
}