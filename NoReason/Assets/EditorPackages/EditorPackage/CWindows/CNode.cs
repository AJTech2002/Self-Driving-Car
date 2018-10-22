using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CNode  {
	public Rect loc = new Rect (new Vector2 (0, 0), new Vector2 (0, 0));
	public Rect pos = new Rect (new Vector2 (100, 100), new Vector2 (80, 80));
	[NonSerialized]
	public Rect connecterBox;
	[NonSerialized]
	public Rect recieverBox;
	[NonSerialized]
	public Rect hideBox;
	[NonSerialized]
	public Rect showBox;
	public string name;
	public bool permaDel;
	public Color tag=Color.red;
	public Rect oTag;
	[NonSerialized]
	public Rect tagR;
	//public NPC_Chat chat=null;
	public string response="";
	private GUIStyle s = null;
	public bool tagOverride = false;
	public string type="";
	public bool hidden=false;
	public bool childrenHidden=false;
	public bool startNode=false;
	//public CNodeList connected = new CNodeList();
	public List<int> connected = new List<int>();
	public List<int> recieved = new List<int>();
	public List<CEffector> effectors = new List<CEffector>();
	public List<CChecker> checkers = new List<CChecker>();

	public virtual void ShowWindow () {
		if (s == null)
			s = MakeTex (200, 200, tag);

		if (!hidden) {
			pos = o (loc, pos.x, pos.y, pos.width, pos.height);
			hideBox = o (pos, 80, 0, 20, 20);
			showBox = o (pos, 80, 30, 20, 20);
			recieverBox = o (pos, -20, 50, 20, 20);
			connecterBox = o (pos, 80, 50, 20, 20);
			if (!startNode)
			GUI.Box (recieverBox, "R");
			GUI.Box (hideBox, "H");
			GUI.Box (showBox, "S");
			GUI.Box (connecterBox, "C");
			GUI.Box (pos, name+":"+response);
			tagR = o (pos, 0, -10, 10, 10);
			if (!tagOverride)
				GUI.Box (tagR, "", s);
			else
				GUI.Box (oTag, "", s);
		}
	}



	public CNode ni (int i, NPC_Chat chat) {
		return chat.nodes.nodes [i];
	}
	public void HideChildren(NPC_Chat c) {
		childrenHidden = !childrenHidden;
		bool val = childrenHidden;
		for (int i = 0; i < connected.Count; i++) {
			ni(connected[i],c).hidden = val;
			ni(connected[i],c).HideChildren (c);
		}
	}


	public void ShowChildren(NPC_Chat c) {
		for (int i = 0; i < connected.Count; i++) {
			ni(connected[i],c).hidden = false;
		}
	}

	public void Hide() {
		hidden = true;
	}

	public void UnHide() {
		hidden = false;
	}

	public virtual string reply() {
		return "";
	}

	public virtual string setReply(string r) {
		return "";
	}

	public void ChangeColor () {
		s = MakeTex (200, 200, tag);
	}
		
	public virtual void AddEffector (CEffector e) {
		return;
	}

	public virtual List<CEffector> GetEffectors() {
		return null;
	}

	public virtual void RemoveEffector (CEffector e) {
		return;
	}

	public virtual void AddCheck (CChecker c) {
		return;
	}

	public virtual void RemoveCheck (CChecker c) {
		return;
	}

	public virtual List<CChecker> GetCheckers() {
		return null;
	}

	Rect r (float x, float y, float xS, float yS) {
		return new Rect (new Vector2 (x, y), new Vector2 (xS, yS));
	}

	Rect o (Rect r, float xOff, float yOff, float xS, float yS) {
		return new Rect (new Vector2 (r.x+xOff, r.y+yOff), new Vector2 (xS, yS));
	}

	private GUIStyle MakeTex( int width, int height, Color col )
	{
		GUIStyle s = new GUIStyle ();
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; i++ )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		s.normal.background = result;
		return s;
	}

}
