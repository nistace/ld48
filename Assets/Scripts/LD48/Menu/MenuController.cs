using LD48.Menu.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Extensions;

namespace LD48.Menu {
	public class MenuController : MonoBehaviour {
		public void Start() {
			MenuUi.onStartButtonClicked.AddListenerOnce(HandleStartButtonClicked);
		}

		private static void HandleStartButtonClicked() => SceneManager.LoadSceneAsync("Game");
	}
}