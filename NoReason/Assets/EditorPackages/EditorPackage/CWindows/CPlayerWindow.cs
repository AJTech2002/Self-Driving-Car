using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CPlayerWindow : CNode {

	public override void ShowWindow ()
	{
		if (type != "P")
			type = "P";
		base.ShowWindow ();
	}

	public override void AddEffector (CEffector e)
	{
		base.effectors.Add (e);
		Debug.Log (base.effectors.Count);
	}

	public override List<CEffector> GetEffectors ()
	{
		return base.effectors;
	}


	public override void RemoveEffector (CEffector e)
	{
		base.effectors.Remove (e);
	}

	Rect r (float x, float y, float xS, float yS) {
		return new Rect (new Vector2 (x, y), new Vector2 (xS, yS));
	}

	Rect o (Rect r, float xOff, float yOff, float xS, float yS) {
		return new Rect (new Vector2 (r.x+xOff, r.y+yOff), new Vector2 (xS, yS));
	}

}
