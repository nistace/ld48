using LD48.Game.Data.Constructions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace LD48.Game.Ui {
	public class ConstructionButtonUi : MonoBehaviour {
		[SerializeField] protected Button           _button;
		[SerializeField] protected Image            _constructionImage;
		[SerializeField] protected GameObject       _unlockedGameObject;
		[SerializeField] protected TMP_Text         _constructionName;
		[SerializeField] protected TMP_Text         _costText;
		[SerializeField] protected GameObject       _lockedGameObject;
		[SerializeField] protected TMP_Text         _unlockConditionText;
		[SerializeField] protected ConstructionType _constructionType;

		public static ConstructionType.Event onConstructionClicked { get; } = new ConstructionType.Event();

		private void Awake() {
			_button.onClick.AddListenerOnce(() => onConstructionClicked.Invoke(_constructionType));
			LdGame.onBlocksClearedChanged.AddListenerOnce(HandleBlocksClearedChanged);
		}

		public void Set(ConstructionType constructionType) {
			_constructionType = constructionType;
			_constructionImage.sprite = constructionType.sprite;
			_constructionName.text = constructionType.displayName;
			_costText.text = $"{constructionType.cost}";
			Refresh();
		}

		private void HandleBlocksClearedChanged(int blocksCleared) => Refresh();

		private void Refresh() {
			var isUnlocked = _constructionType.unlockedAfterCount <= LdGame.blocksCleared;
			_constructionImage.color = Color.white.With(a: isUnlocked ? 1 : .3f);
			_unlockedGameObject.SetActive(isUnlocked);
			_lockedGameObject.SetActive(!isUnlocked);
			_unlockConditionText.SetText($"Dig {_constructionType.unlockedAfterCount - LdGame.blocksCleared} more blocks to unlock");
			_button.interactable = isUnlocked;
		}
	}
}