using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class ChatSystem : EditorWindow {
	public Rect loc = new Rect(new Vector2(0,0), new Vector2(0,0));
	#region Main
	private NPC_Chat c=new NPC_Chat();
	private List<CNode> deleted = new List<CNode>();
	[MenuItem("Game Systems/NPC-Chat")]
	private static void Init() {
		EditorWindow w = (ChatSystem)EditorWindow.GetWindow (typeof(ChatSystem));
		w.Show ();
	}
	/// <summary>
	/// CHANGE TO USE
	/// </summary>
	private Rect w;
	private string name="";
	private void OnGUI() {
		if (c == null) {
			selectedNode = null;
			connectionDragging = null;
			dragNode = null;
			if (n != null)
				n.nodes.Clear ();
			if (deleted != null)
				deleted.Clear ();
			c = new NPC_Chat ();
		}
		SaveLoad ();
		Repaint ();
		w = position;
		DrawMain ();
		if (!Application.isPlaying && c != null) {
			UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty ();
		}

	}

	private string path;
	private void CheckFileSystem() {
		if (!Directory.Exists (Application.persistentDataPath + "/NodeData/")) {
			Directory.CreateDirectory (Application.persistentDataPath + "/NodeData/");
		}
		if (!Directory.Exists (Application.dataPath + "/NodeData/" + name + "/")) {
			Directory.CreateDirectory (Application.dataPath + "/NodeData/"+name+"/");
			path = Application.dataPath + "/NodeData/" + name + "/";
		}
	}
		
		

	private void SaveLoad() {
		name = GUI.TextField (r (10, 10, 50, 20), name);
		if (GUI.Button (r (60, 10, 70, 20), "Save")) {
			CheckFileSystem ();
			if (Selection.activeGameObject == null) {
				string json = (string)JsonUtility.ToJson (c);
				string path = Application.dataPath + "/NodeData/" + name + "/";
				File.WriteAllText (Path.Combine (path, name + ".json"), json);
				UnityEditor.AssetDatabase.Refresh ();
			} else {
				if (Selection.activeGameObject.GetComponent<NPC> () != null) {
					NPC_Chat r = Selection.activeGameObject.GetComponent<NPC> ().chat;
					Selection.activeGameObject.GetComponent<NPC> ().chat = c;
					Selection.activeGameObject.GetComponent<NPC> ().loadChat = c;
					c = new NPC_Chat ();
					c.dict = r.dict;
					c.nodes = r.nodes;
					EditorUtility.SetDirty (Selection.activeGameObject);
				}

			}

		}
		if (GUI.Button (r (140, 10, 70, 20), "Load")) {
			CheckFileSystem ();
			if (Selection.activeGameObject == null) {
				string path = Application.dataPath + "/NodeData/" + name + "/";
				string s = File.ReadAllText (Path.Combine (path, name + ".json"));
				NPC_Chat json = (NPC_Chat)JsonUtility.FromJson<NPC_Chat> (s);
				Reset (json);
			}
			else {
				if (Selection.activeGameObject.GetComponent<NPC> () != null) {
					NPC_Chat cr = Selection.activeGameObject.GetComponent<NPC> ().loadChat;
					NPC_Chat cd = new NPC_Chat ();
					cd = new NPC_Chat ();
					cd.dict = cr.dict;
					cd.nodes = cr.nodes;
					Reset (cd);
				}

			}
		}

		if (GUI.Button (r(220, 10, 70, 20), "Run")) {
			if (c != null) {
				pathB.Clear ();
				List<CNode> final = new List<CNode> ();
				List<CNode> starts = new List<CNode> ();
				List<CNode> curBatch = new List<CNode> ();
				starts = c.FindStart ();
				int r = Random.Range (0, starts.Count);
				final.Add(starts[r]);
				curBatch = c.Branch (starts [r]);
				while (curBatch != null) {
					int sel = Random.Range (0, curBatch.Count);
					final.Add (curBatch[sel]);
					curBatch = c.Selection (curBatch [sel]);
						
				}
				pathB = final;
			}
		}

		if (GUI.Button (r(300, 10, 70, 20), "Clear Run")) {
				pathB.Clear ();
		}

	}

	List<CNode> pathB = new List<CNode>();
	private void DrawPath() {
		if (pathB != null && pathB.Count > 0) {
			for (int i = 0; i < pathB.Count; i++) {
				if (i == 0)
					continue;
				int p = i - 1;
				Handles.BeginGUI ();
				Handles.color = Color.green;
				float nC = pathB [i].pos.height;
				float hC = pathB [i].pos.height;
				Handles.DrawBezier (pathB [p].pos.center, pathB [i].pos.center, pathB [p].pos.center + Vector2.up * hC, pathB [i].pos.center+ Vector2.down * hC, Color.green, null, 8);
//				Handles.DrawBezier (pathB [i].pos.center, pathB [p].pos.center);
				Handles.EndGUI ();

			}
		}
	}

	private void Reset (NPC_Chat chat) {
		selectedNode = null;
		connectionDragging = null;
		dragNode = null;
		n.nodes.Clear ();
		deleted.Clear ();
		c = chat;
		for (int i = 0; i<c.nodes.nodes.Count; i++) {
			//c.nodes.nodes [i].chat = c;
			c.nodes.nodes [i].ChangeColor ();
		}
	}

	#endregion

	#region MainUI
	private Rect main;
	private void DrawMain() {
		main = r (0, 0, w.size.x / 2 + w.size.x/4.7f, w.size.y);
		MainWindowLoop ();
		DrawPath ();
		NodeAdderPanel ();
		DrawInspector ();
		Enumming ();
	}
	private Vector2 enummingPos;
	private enum SelectionType {Gate,Player,NPC};
	private SelectionType t;
	private void Enumming() {
		Event e = Event.current;
		if (e != null && e.keyCode != KeyCode.None  && e.type == EventType.KeyUp && e.alt==true) {
			if (e.keyCode == KeyCode.A) {
				if (c != null) {
					BNode n = new BNode ();
					c.nodes.Add ((CNode)n);
					CNode newN = c.nodes.nodes [c.nodes.nodes.Count - 1];
					//c.nodes.nodes [c.nodes.nodes.Count - 1].chat = c;
					c.nodes.nodes [c.nodes.nodes.Count - 1].name = "Gate";
					newN.pos.position = e.mousePosition;
					for (int i = 0; i < c.dict.keys.Count; i++) {
						CChecker checkNew = new CChecker ();
						checkNew.checkType = CChecker.propertyCheck.NA;
						checkNew.propertyName = c.dict.keys [i];
						checkNew.value = c.dict.value (c.dict.keys [i]);
						newN.checkers.Add (checkNew);
					}

				}
			}

			if (e.keyCode == KeyCode.S) {
				if (c != null) {
					//				ScriptableObject n = ScriptableObject.CreateInstance<CPlayerWindow> ();
					c.nodes.Add ((CNode)new CPlayerWindow());
					CNode newN = c.nodes.nodes [c.nodes.nodes.Count - 1];
					//c.nodes.nodes [c.nodes.nodes.Count - 1].chat = c;
					c.nodes.nodes [c.nodes.nodes.Count - 1].name = "Player Node";
					newN.pos.position = e.mousePosition;
					for (int i = 0; i < c.dict.keys.Count; i++) {
						CEffector ef = new CEffector ();
						ef.s = c.dict.keys [i];
						ef.f = c.dict.value (c.dict.keys [i]);
						ef.type = CEffector.operationType.NA;
						newN.effectors.Add (ef);
					}

				}
			}

			if (e.keyCode == KeyCode.D) {
				if (c != null) {
					c.nodes.Add ((CNode)new CNPCWindow());
					CNode newN = c.nodes.nodes [c.nodes.nodes.Count - 1];
					//c.nodes.nodes [c.nodes.nodes.Count - 1].chat = c;
					c.nodes.nodes [c.nodes.nodes.Count - 1].name = "NPC";
					newN.pos.position = e.mousePosition;
					for (int i = 0; i < c.dict.keys.Count; i++) {
						CEffector ef = new CEffector ();
						ef.s = c.dict.keys [i];
						ef.f = c.dict.value (c.dict.keys [i]);
						ef.type = CEffector.operationType.NA;
						newN.effectors.Add (ef);
					}

				}
			}

		}

	}

	CNode ni (int i) {
		if (c != null) {
			return c.nodes.nodes [i];
		}
		else
			return new CNode ();
	}
	private void ConnectionDrawing(CNode n) {

		Event e = Event.current;
		if (connectionDragging != null) {
			Vector2 nC = connectionDragging.connecterBox.center;
			float hC = connectionDragging.pos.height;
			Handles.DrawBezier (nC, e.mousePosition, nC + Vector2.up * hC, e.mousePosition + Vector2.down * hC, connectionDragging.tag, null, 5);
		}
		for (int i = 0; i < deleted.Count; i++) {
			if (n.connected.Contains (i))
				n.connected.Remove (i);
		}
		for (int i = 0; i < n.connected.Count; i++) {
			if (n.connected[i] == null || n.hidden || n.childrenHidden || ni(n.connected[i]).hidden) {
				continue;
			}
			Handles.BeginGUI ();
			Vector2 nC = n.connecterBox.center;
			float hC = n.pos.height/2;
			Vector2 nR = ni(n.connected[i]).recieverBox.center;
			float hR = n.pos.height/2;
			Handles.DrawBezier (nC, nR, nC+Vector2.up*hC,nR+Vector2.down*hR, n.tag, null, 5);
			Handles.EndGUI ();
		}
	}

	Rect nAdder;
	private void NodeAdderPanel() {
		nAdder = o (main, 0, w.height - 30, main.width, 30);
		GUI.Box (nAdder, "");
		Rect bNode = o (nAdder, 20, 5, 100, 20);
		if (GUI.Button (bNode, "Begin/Gate")) {
			if (c != null) {
				BNode n = new BNode ();
				c.nodes.Add ((CNode)n);
				CNode newN = c.nodes.nodes [c.nodes.nodes.Count - 1];
				//c.nodes.nodes [c.nodes.nodes.Count - 1].chat = c;
				c.nodes.nodes [c.nodes.nodes.Count - 1].name = "Gate";
				for (int i = 0; i < c.dict.keys.Count; i++) {
					CChecker checkNew = new CChecker ();
					checkNew.checkType = CChecker.propertyCheck.NA;
					checkNew.propertyName = c.dict.keys [i];
					checkNew.value = c.dict.value (c.dict.keys [i]);
					newN.checkers.Add (checkNew);
				}

			}
		}
		Rect playerNode = o (nAdder, 140, 5, 100, 20);
		Rect npcNode = duplicate (playerNode, 120, 0);
		if (GUI.Button (playerNode, "Player Response")) {
			if (c != null) {
//				ScriptableObject n = ScriptableObject.CreateInstance<CPlayerWindow> ();
				c.nodes.Add ((CNode)new CPlayerWindow());
				CNode newN = c.nodes.nodes [c.nodes.nodes.Count - 1];
				//c.nodes.nodes [c.nodes.nodes.Count - 1].chat = c;
				c.nodes.nodes [c.nodes.nodes.Count - 1].name = "Player Node";

				for (int i = 0; i < c.dict.keys.Count; i++) {
					CEffector ef = new CEffector ();
					ef.s = c.dict.keys [i];
					ef.f = c.dict.value (c.dict.keys [i]);
					ef.type = CEffector.operationType.NA;
					newN.effectors.Add (ef);
				}

			}
		}
		if (GUI.Button (npcNode, "NPC Response")) {
			if (c != null) {
				c.nodes.Add ((CNode)new CNPCWindow());
				CNode newN = c.nodes.nodes [c.nodes.nodes.Count - 1];
				//c.nodes.nodes [c.nodes.nodes.Count - 1].chat = c;
				c.nodes.nodes [c.nodes.nodes.Count - 1].name = "NPC";

				for (int i = 0; i < c.dict.keys.Count; i++) {
					CEffector ef = new CEffector ();
					ef.s = c.dict.keys [i];
					ef.f = c.dict.value (c.dict.keys [i]);
					ef.type = CEffector.operationType.NA;
					newN.effectors.Add (ef);
				}

			}

		}
	//	Rect sel = duplicate (npcNode, 120, 0);
	//	c = (NPC_Chat)EditorGUI.ObjectField (sel, c, typeof(NPC_Chat));
		Rect clear = duplicate (npcNode, 120, 0);
		if (GUI.Button (clear, "Clear")) {
			if (c != null)
				c.nodes.nodes.Clear ();
			deleted.Clear ();
		}
		Rect showAll = duplicate (clear, 120, 0);
		if (GUI.Button (showAll, "Show All")) {
			if (c != null) {
				for (int i = 0; i < c.nodes.nodes.Count; i++) {
					c.nodes.nodes [i].hidden = false;
				}
			}
		}
	}
		


	private Rect inspector;
	private string addName="";
	private void DrawInspector() {
		inspector = o (main, main.size.x, 0, w.size.x-main.size.x, w.size.y);
		GUI.Box (inspector, "");

		if (selectedNode != null) {

			Rect nameBox = o (inspector, 20, 30, 80, 20);
			Rect nText = duplicate (nameBox, 0, -20);
			selectedNode.name = GUI.TextField (nameBox, selectedNode.name);
			GUI.Label (nText, "Node Name :");
			Rect cText = duplicate (nameBox, 0, 30);
			GUI.Label (cText, "Tag Color :");
			Rect cPicker = duplicate (cText, 0, 20);
			selectedNode.tag = EditorGUI.ColorField (cPicker, selectedNode.tag);
			Rect cBut = duplicate (cPicker, 0, 20);
			if (GUI.Button (cBut, "Change")) {
				selectedNode.ChangeColor ();
			}

			if (selectedNode.type != "B") {
				Rect effectT = duplicate (cBut, 0, 30);
				Rect response = o (nameBox, 90, 0, 100, 140);
				selectedNode.response = GUI.TextArea (response, selectedNode.response);
				GUI.Label (effectT, "Effectors");
				Rect effectAdd = duplicate (effectT, 0, 20);
				if (GUI.Button (effectAdd, "+")) {
					CEffector ef = new CEffector ();
//					ef.dict = c.dict;
					ef.type = CEffector.operationType.Add;
					selectedNode.effectors.Add (ef);
					Debug.Log ("0:"+selectedNode.effectors[0].s);
				}
				if (selectedNode.effectors != null) {
					List<CEffector> e = selectedNode.effectors;
					Rect currentE = o (effectAdd, 0, 20, 80, 20);
					GUI.Label (currentE, "Effectors :");
					for (int i = 0; i < e.Count; i++) {
						//e [i].chat = c;
						Rect pT = o (effectAdd, 0, 34 * (i + 1) + 30, 70, 20);
						Rect bT = o (pT, -10, -5, 250, 30);
						GUI.Box (bT, "");
						e [i].s = GUI.TextField (pT, e [i].s);
						Rect pF = o (pT, 75, 0, 30, 20);
						e [i].f = EditorGUI.FloatField (pF, e [i].f);
						Rect pE = o (pF, 35, 0, 60, 40);
						e [i].type = (CEffector.operationType)EditorGUI.EnumPopup (pE, e [i].type);
						Rect pM = o (pE, 65, 0, 20, 20);
						if (GUI.Button (pM, "-"))
							selectedNode.effectors.Remove (selectedNode.effectors [i]);
					}
				}

			} else {
				Rect checkT = duplicate (cBut, 0, 30);
				GUI.Label (checkT, "Conditions");
				Rect effectR = duplicate (checkT, 0, 20);
				if (GUI.Button (effectR, "+")) {
					CChecker cr = new CChecker ();
//					cr.dict = c.dict;
					selectedNode.checkers.Add (cr);
				}
				Rect effectAdd = duplicate (effectR, 0, 20);
				if (GUI.Button (effectAdd, "Start")) {
					selectedNode.startNode = !selectedNode.startNode;
					if (selectedNode.startNode) {
						selectedNode.tag = Color.green;
						selectedNode.ChangeColor ();
						selectedNode.name = "START";
					}
				}
				if (selectedNode.checkers != null) { 
					List<CChecker> cc = selectedNode.checkers;
					Rect currentC = o (effectAdd, 0, 20, 80, 20);
					GUI.Label (currentC, "Checkers : ");
					for (int i = 0; i < cc.Count; i++) {
						Rect pT = o (effectAdd, 0, 34 * (i + 1) + 30, 70, 20);
						Rect bT = o (pT, -10, -5, 270, 30);
						GUI.Box (bT, "");
						cc [i].propertyName = GUI.TextField (pT, cc [i].propertyName);
						Rect pF = o (pT, 75, 0, 70, 40);
						cc [i].checkType = (CChecker.propertyCheck)EditorGUI.EnumPopup (pF, cc [i].checkType);
						Rect pV = o (pF, 75, 0, 70, 20);
						cc [i].value = EditorGUI.FloatField (pV, cc [i].value);
						Rect pM = o (pV, 75, 0, 20, 20);
						if (GUI.Button (pM, "-"))
							selectedNode.checkers.Remove(selectedNode.checkers [i]);
					}
				}
			}


		} else {
			Rect AddBox = o (inspector, 20, 30, 130, 20);
			Rect nText = duplicate (AddBox, 0, -20);
			if (GUI.Button (AddBox, "Add Property")) {
					c.dict.Add (addName, 0);
			}
			GUI.Label (nText, "Properties");
			Rect addNameR = duplicate (AddBox, 0, 20);
			addName = GUI.TextField (addNameR, addName);
			Rect clearProps = duplicate (addNameR, 0, 20);
			if (GUI.Button (clearProps, "Clear Properties")) {
				c.dict.Clear ();
			}
			Rect pT = duplicate (clearProps, 0, 30);
			GUI.Label (pT, "Properties :");
			if (c != null)
			if (c.dict != null && c.dict.keys.Count > 0) {
				List<string> keys = c.dict.keys;
				for (int i = 0; i < keys.Count; i++) {
					Rect pTT = o (pT, 0, 34 * (i + 1) + 10, 70, 20);
					Rect bT = o (pTT, 0, -5, 170, 30);
					GUI.Box (bT, "");
					GUI.Label (pTT, keys[i]);
					Rect vT = o (pTT, 75, 0, 30, 20);
					Rect bTT = o (vT, 35, 0, 20, 20);
					Rect bTTT = o (bTT, 25, 0, 20, 20);
					if (!c.dict.setInStone)
						c.dict.SetValue (keys [i], EditorGUI.FloatField (vT, c.dict.value (keys [i])));
					else
						GUI.Label (vT, c.dict.value (keys [i]).ToString());
					if (GUI.Button (bTT, "-")) {
						c.dict.Remove (keys [i]);
					}
					if (GUI.Button (bTTT, "[]")) {
						c.dict.setInStone = !c.dict.setInStone;
					}
						continue;
					}
				
				}
			}
		}
		
	#endregion

	#region WindowRendering
	private CNode dragNode;
	private CNodeList n = new CNodeList();
	private void MainWindowLoop() {
		//Loop
		if (c != null) {
			if (c.nodes != null) {
				for (int i = 0; i < c.nodes.nodes.Count; i++) {
					if (!c.nodes.nodes [i].permaDel) {
						c.nodes.nodes [i].ShowWindow ();
						ConnectionDrawing (c.nodes.nodes [i]);
						OutOfScreenCheck (c.nodes.nodes [i]);
						c.nodes.nodes [i].loc = loc;
						if (!c.nodes.nodes [i].hidden) {
							DragWindowLoop (c.nodes.nodes [i], i);
						}
					}
				}
			}
		}



	}

	private void OutOfScreenCheck (CNode w) {
		Rect s = position;
		if (w.pos.y > nAdder.yMin || w.pos.x > inspector.xMin || w.pos.x < 0 || w.pos.y < 0) {
			w.tagOverride = true;
			Vector2 pos = w.tagR.position;
			if (w.pos.y < 0)
				pos.y = 0;
			else if (w.pos.y > nAdder.yMin)
				pos.y = nAdder.yMin-10;
			if (w.pos.x < 0)
				pos.x = 0;
			else if (w.pos.x > inspector.xMin)
				pos.x = inspector.xMin-10;

			w.oTag = new Rect (pos, new Vector2 (10, 10));
		} else {
			w.tagOverride = false;
		}
	}

	private CNode selectedNode;
	private CNode connectionDragging;
	private Vector2 lastPos;
	bool enumming=false;
	private void DragWindowLoop(CNode w, int ind) {
		Event e = Event.current;
		if (e.button == 0 && e.type == EventType.MouseDown && e.control == false) {

			if (w.pos.Contains (e.mousePosition)) {
				if (dragNode == null && connectionDragging == null) {
					selectedNode = w;
				}
			}

		}

		if (e.button == 0 && e.type == EventType.MouseDown && e.shift == true) {
			if (dragNode == null && connectionDragging == null) {
				if (!w.pos.Contains (e.mousePosition)) {
					enumming = true;
					enummingPos = e.mousePosition;
				}
			}
		}

		if (e.button == 0 && e.type == EventType.MouseUp && e.alt == true) {
			if (w.recieverBox.Contains (e.mousePosition)) {
				for (int i = 0; i < c.nodes.nodes.Count; i++) {
					if (c.nodes.nodes [i].connected.Contains (ind))
						c.nodes.nodes [i].connected.Remove (ind);
				}
				w.recieved.Clear ();
			}
			if (w.connecterBox.Contains (e.mousePosition)) {
				w.connected.Clear ();
			}
		}
			

		if (e.button == 0 && e.type == EventType.MouseDrag) {
			if (dragNode == w) {
				dragNode.pos = new Rect (new Vector2 (e.mousePosition.x - w.pos.width / 2, e.mousePosition.y - w.pos.height / 2), w.pos.size);
				return;
			}
			if (w.pos.Contains (e.mousePosition)) {
				if (dragNode == null) {
					dragNode = w;
					dragNode.pos = new Rect (new Vector2 (e.mousePosition.x - w.pos.width / 2, e.mousePosition.y - w.pos.height / 2), w.pos.size);
				}
			}
			if (w.connecterBox.Contains (e.mousePosition) && connectionDragging == null) {
				connectionDragging = w;
			}
		}
		if (e.button == 1 && e.type == EventType.MouseDown) {
			if (w.hideBox.Contains (e.mousePosition)) {
				w.HideChildren (c);
				return;
			} else {
				selectedNode = null;
				connectionDragging = null;
			}

		}
		if (e.button == 0 && e.type == EventType.MouseUp) {
			dragNode = null;
			if (w.hideBox.Contains (e.mousePosition)) {
				w.hidden = !w.hidden;
				return;
			}
			if (w.showBox.Contains (e.mousePosition)) {
				w.ShowChildren (c);
			}
			if (w.recieverBox.Contains (e.mousePosition) && connectionDragging != null && !connectionDragging.connected.Contains(ind)) {
				connectionDragging.connected.Add (ind);
				w.recieved.Add (nodeToIndex (connectionDragging));
				w.tag = connectionDragging.tag;
				w.ChangeColor ();
				connectionDragging = null;
				selectedNode = w;
			}
		}
		if (e.button == 2 && e.type == EventType.MouseDown && e.alt == true) {
			if (w.pos.Contains (e.mousePosition)) {
				for (int i = 0; i < c.nodes.nodes.Count; i++) {
					if (c.nodes.nodes [i].connected.Contains (ind))
						c.nodes.nodes [i].connected.Remove (ind);
				}
					w.recieved.Clear ();
					w.connected.Clear ();
				w.permaDel = true;
				w.hidden = true;
			}
		}
		if (e.button == 2 && e.type == EventType.MouseDrag) {
			Vector2 dir = (e.mousePosition - lastPos).normalized;
			loc.position -= dir*0.3f;
			lastPos = e.mousePosition;
		}
		if (e.button == 2 && e.type == EventType.MouseUp) {
			loc.position = Vector2.zero;
		}
	}

	private int nodeToIndex (CNode n) {
		for (int i = 0; i < c.nodes.nodes.Count; i++) {
			if (c.nodes.nodes [i] == n)
				return i;
		}
		return 0;
	}

	private bool delNode;


	#endregion

	#region Helpers
	Rect r (float x, float y, float xS, float yS) {
		return new Rect (new Vector2 (x, y), new Vector2 (xS, yS));
	}

	Rect o (Rect r, float xOff, float yOff, float xS, float yS) {
		return new Rect (new Vector2 (r.x+xOff, r.y+yOff), new Vector2 (xS, yS));
	}

	Rect duplicate(Rect r, float xOff, float yOff) {
		return new Rect (new Vector2 (r.x + xOff, r.y + yOff), r.size);
	}

	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	#endregion
}
