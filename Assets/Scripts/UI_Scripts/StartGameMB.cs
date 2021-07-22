using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartGameMB : MonoBehaviour
{
	public string sceneName = "game";

	public bool startInTutorial;

	void Start()
	{
		Button b = GetComponent<Button>();
		if (b != null && sceneName != "")
		{
			b.onClick.AddListener(() =>
			{
				Debug.Log(sceneName);
				PersistentObjMB.instance.SetStartInTutorial(startInTutorial);
				SceneManager.LoadScene(sceneName);
			});
		}
	}
}
