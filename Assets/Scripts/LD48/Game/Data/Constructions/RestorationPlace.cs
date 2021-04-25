using LD48.Game.Data.Dwarfs;
using UnityEngine;

namespace LD48.Game.Data.Constructions {
	[RequireComponent(typeof(Construction))]
	public class RestorationPlace : MonoBehaviour {
		[SerializeField] protected DwarfNeed  _need;
		[SerializeField] protected int        _capacity;
		[SerializeField] protected GameObject _fullPlaceObject;
		[SerializeField] protected float      _restorationPerSecond;

		private Construction sConstruction        { get; set; }
		public  Construction construction         => sConstruction ? sConstruction : sConstruction = GetComponent<Construction>();
		public  DwarfNeed    need                 => _need;
		public  bool         isFull               => countInside >= _capacity;
		public  float        restorationPerSecond => _restorationPerSecond;
		public  int          countInside          { get; set; }

		public void AddPersonInside() => SetCountInside(countInside++);

		public void RemovePersonInside() => SetCountInside(countInside--);

		private void SetCountInside(int count) {
			countInside = count;
			if (_fullPlaceObject) _fullPlaceObject.SetActive(isFull);
		}

		public bool Restore(DwarfNeeds needs) {
			needs.Restore(need, restorationPerSecond * Time.deltaTime);
			return needs[need] < 1;
		}
	}
}