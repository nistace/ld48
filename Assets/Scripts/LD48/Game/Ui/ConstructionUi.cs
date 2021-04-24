using System.Linq;
using LD48.Constants;
using UnityEngine;
using Utils.Extensions;
using Utils.Ui;

namespace LD48.Game.Ui {
	public class ConstructionUi : MonoBehaviourUi {
		[SerializeField] protected Transform            _constructionsContainer;
		[SerializeField] protected ConstructionButtonUi _constructionButtonPrefab;

		private void Start() => LdMemory.constructionTypes.Values.OrderBy(t => t.unlockedAfterCount).ForEach(t => Instantiate(_constructionButtonPrefab, _constructionsContainer).Set(t));
	}
}