using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BNode : CNode {

	public override void ShowWindow ()
	{
		if (type != "B")
			type = "B";
		base.ShowWindow ();

	}

	public override void AddCheck (CChecker c)
	{
		base.checkers.Add (c);
	}

	public override List<CChecker> GetCheckers ()
	{
		return base.checkers;
	}

	public override void RemoveCheck (CChecker c)
	{
			base.checkers.Remove (c);
	}

}

[System.Serializable]
public class CheckersList {
	[SerializeField]
	public List<CChecker> checkers = new List<CChecker>();
	public void Add(CChecker c) {
		checkers.Add (c);
	}
	public void Remove (CChecker c) {
		if (checkers.Contains (c)) {
			checkers.Remove (c);
		}
	}
}