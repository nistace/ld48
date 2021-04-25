using System;
using LD48.Game.Data.Dwarfs;
using UnityEngine;

namespace LD48.Game.Data.Constructions {
	[Serializable]
	public class RestorationOnGround {
		[SerializeField] protected bool             _appeared;
		[SerializeField] protected RestorationPlace _restorationPlace;
		[SerializeField] protected int              _appearsAfterBlocks;

		public bool appeared           => _appeared;
		public int  appearsAfterBlocks => _appearsAfterBlocks;

		public void Appear() {
			DwarfNeeds.SetNeedActive(_restorationPlace.need, true);
			_appeared = true;
			_restorationPlace.gameObject.SetActive(true);
			World.AddConstruction(_restorationPlace.construction, World.WorldPointToCoordinates(_restorationPlace.transform.position + Vector3.up / 2));
		}

		public void Hide() {
			_appeared = false;
			_restorationPlace.gameObject.SetActive(false);
		}
	}
}