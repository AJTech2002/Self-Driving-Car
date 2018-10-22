using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.IO;
[CreateAssetMenu(fileName = "ChatData#", menuName = "Game Systems/NPC Chat", order = 1)]
[System.Serializable]
public class NPC_Chat {
	[SerializeField]
	public SDictionary dict = new SDictionary ();
	//public Dictionary<string, float> values = new Dictionary<string,float> ();
	[SerializeField]
	public CNodeList nodes = new CNodeList ();



	public void BeginPath() {
		start = null;
		FindStart ();
	}

	public void RemoveNode (CNode n) {
		if (nodes.nodes.Contains (n)) {
			nodes.Remove (n);
		}

	}

	private CNode start = null;

	public List<CNode> FindStart() {
		List<CNode> possibleStarts = new List<CNode> ();
		for (int i = 0; i < nodes.nodes.Count; i++) {
			if (nodes.nodes [i].startNode == true && nodes.nodes[i].hidden == false) {
				possibleStarts.Add (nodes.nodes [i]);
				continue;
			}
		}

		List<CNode> stillPossible = new List<CNode> ();
		for (int i = 0; i < possibleStarts.Count; i++) {
			if (possibleStarts [i].checkers.Count > 0) {
				List<CChecker> checks = new List<CChecker> ();
				checks.AddRange (possibleStarts [i].checkers);
				bool canAdd = true;
				for (int x = 0; x < checks.Count; x++) {
					if (checks [x].passCheck (dict) && possibleStarts[i].hidden == false) {
						canAdd = true;
					} else {
						canAdd = false;
						break;
					}
				}
				if (canAdd)
				stillPossible.Add (possibleStarts[i]);
			} else
				stillPossible.Add (possibleStarts [i]);
		}
		if (stillPossible.Count > 0)
			return stillPossible;
		else {
			Debug.LogError ("YOU NEED A START NODE!");
		}
		return null;
	}

	public List<CNode> Branch (CNode n) {
		Debug.Log (n.name);
		List<CNode> retN = new List<CNode> ();
		if (n.connected.Count > 0) {
			List<int> connections = n.connected;
			List<CNode> nr = new List<CNode> ();
			for (int i = 0; i < connections.Count; i++) {
				nr.Add (nodes.nodes [connections [i]]);
			}
			for (int i = 0; i < nr.Count; i++) {
				if (nr [i].type == "B") {
					if (nr [i].checkers.Count > 0) {
						bool shouldAdd = true;
						for (int x = 0; x < nr [i].checkers.Count; x++) {
							if (nr [i].checkers [x].passCheck (dict) == false)
								shouldAdd = false;
						}
						if (shouldAdd)
						retN.Add (nr [i]);
					} else
						retN.Add (nr [i]);
				} 
				else {
				retN.Add (nr [i]);
				}
			}

			if (retN.Count > 0) {
				return retN;
			}
			else
				return null;
		}
		return null;
	}

	public List<CNode> Selection (CNode n) {
		//if (GateCheck (n) == false)
		//	return null;
		if (n.effectors.Count > 0) {
			for (int i = 0; i < n.effectors.Count; i++) {
				Debug.Log ("WORKING?");
				n.effectors [i].ActEffect (dict);
			}
		}
		return Branch (n);
	}

	public bool GateCheck (CNode n) {
		if (n.type == "B") {
			if (n.checkers.Count > 0) {
				bool pass = true;
				for (int i = 0; i < n.checkers.Count; i++) {
					if (n.checkers [i].passCheck (dict) == false)
						return false;
				}
				return true;
			}
		} else
			return true;
		return true;
	}

}
[System.Serializable]
public class ChatReturn {
	public string type="P";
	public List<string> responses = new List<string>();
}

[System.Serializable]
public class CNPCProp {
	[SerializeField]
	public string response;
	[SerializeField]
	public CEffectorList effectors = new CEffectorList();
}

[System.Serializable]
public class CEffectorList {
	[SerializeField]
	public List<CEffector> effectors = new List<CEffector>();
	public void Add(CEffector c) {
		effectors.Add (c);
	}
	public void Remove (CEffector c) {
		if (effectors.Contains (c)) {
			effectors.Remove (c);
		}
	}
}

[System.Serializable]
public class CNPCNode {
	public List<CNPCProp> opts = new List<CNPCProp>();
	public Color tag;
}

[System.Serializable]
public class CChecker {
	public string propertyName="";
	public float value=0;
	public enum propertyCheck {LowerThan, GreaterThan, EqualTo, GreaterOrEqual, LowerOrEqual, NA};
	public propertyCheck checkType=propertyCheck.EqualTo;
	private SDictionary dict = new SDictionary ();

	public bool available (string s) {
		if (dict.keys.Contains (s)) {
			return true;
		} else
			return false;
	}

	public bool passCheck(SDictionary newD) {
		dict = newD;
		if (available(propertyName)) {
			if (checkType == propertyCheck.GreaterThan) {
				if (dict.value (propertyName) > value)
					return true;
			} else if (checkType == propertyCheck.EqualTo) {
				if (dict.value (propertyName) == value)
					return true;

			} else if (checkType == propertyCheck.GreaterOrEqual) {
				if (dict.value (propertyName) >= value)
					return true;

			} else if (checkType == propertyCheck.LowerOrEqual) {
				if (dict.value (propertyName) <= value)
					return true;

			} else if (checkType == propertyCheck.LowerThan) {
				if (dict.value (propertyName) < value)
					return true;

			} else if (checkType == propertyCheck.NA) {
				return true;
			}
			return false;
		}
		return false;
	}

}

[System.Serializable]
public class CPlayerNode {
	public List<CPlayerProp> opts = new List<CPlayerProp> ();
	public Color tag;
}

[System.Serializable]
public class CPlayerProp {
	[SerializeField]
	public string response;
	[SerializeField]
	public CEffectorList effects = new CEffectorList();
}

[System.Serializable]
public class CEffector{
	public string s="";
	public float f=0;
	//public SDictionary dict = new SDictionary();
	public enum operationType {Multiply,Divide,Add,Minus,Equal,NA};
	public operationType type=operationType.Add;

	public void ActEffect (SDictionary dict) {
		if (dict.keys.Contains (s)) {
			if (type == operationType.Add)
				dict.SetValue (s, dict.value (s) + f);
			if (type == operationType.Divide)
				dict.SetValue (s, dict.value (s) / f);
			if (type == operationType.Minus)
				dict.SetValue (s, dict.value (s) - f);
			if (type == operationType.Multiply)
				dict.SetValue (s,dict.value (s) * f);
			if (type == operationType.Equal)
				dict.SetValue (s, f);
		} 
	}


}


[System.Serializable]
public class SDictionary {
	public bool setInStone;
	public List<string> keys = new List<string>();
	public List<float> values = new List<float>();

	public void Add (string k, float v) {
		keys.Add (k);
		values.Add (v);
	}

	public void Remove (string k) {
		int r = 0;
		for (int i = 0; i < keys.Count; i++) {
			if (keys [i] == k) {
				r = i;
				break;
			}
		}
		keys.RemoveAt (r);
		values.RemoveAt (r);
	}

	public void Clear() {
		keys.Clear ();
		values.Clear ();
	}

	public float value(string s) {
		if (keys.Contains (s)) {
			int r = 0;
			for (int i = 0; i < keys.Count; i++) {
				if (keys [i] == s) {
					r = i;
					break;
				}
			}
			return values [r];
		} else
			return float.NaN;
	}

	public void SetValue (string s, float v) {
		if (keys.Contains(s)) {
			int r = 0;
			for (int i = 0; i < keys.Count; i++) {
				if (keys [i] == s) {
					r = i;
					break;
				}
			}
			values [r] = v;
		}
	}

}

[System.Serializable]
public class CNodeList {
	public List<CNode> nodes = new List<CNode>();
	public void Add(CNode c) {
		nodes.Add (c);
	}
	public void Remove (CNode c) {
		if (nodes.Contains (c)) {
			nodes.Remove (c);
		}
	}
}