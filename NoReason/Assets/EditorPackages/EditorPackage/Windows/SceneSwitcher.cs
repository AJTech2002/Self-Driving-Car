using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneSwitcher : EditorWindow {

	[MenuItem("Scenes/Home")]
	public void GoHome() {
		Application.LoadLevel (0);
	}


}
