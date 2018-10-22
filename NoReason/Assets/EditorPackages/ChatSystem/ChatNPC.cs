using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ChatNPC : MonoBehaviour {

	public NPC_Chat chat;
	public string path;



	[HideInInspector]
	private List<CNode> n = new List<CNode>();
	private void Awake() {
		/*if (path != string.Empty) {
		StreamReader reader = new StreamReader (path);
		string json = reader.ReadToEnd ();
		chat = (NPC_Chat)JsonUtility.FromJson<NPC_Chat> (json);
		n = chat.nodes.nodes;
		current = FindStart ();
		ReadNodeData ();
		}*/
	}


	public void Reset (string newPath) {
		cur.Clear ();
		StreamReader reader = new StreamReader (newPath);
		string json = reader.ReadToEnd ();
		chat = (NPC_Chat)JsonUtility.FromJson<NPC_Chat> (json);
		n = chat.nodes.nodes;
		current = FindStart ();
	}

	private List<CNode> cur = new List<CNode>();
	private void ReadNodeData() {
		CNode start = FindStart ();
		cur.Add (start);

		while (cur != null && cur.Count > 0) {
			int randInt = Random.Range (0, cur.Count);
			SelectNode (cur [randInt]);
//			print (cur [randInt].name);
			cur = Branch (cur [randInt]);
		}

	}

	private CNode current;
	public List<CNode> FindNextNodes() {
		return Branch (current);
	}

	public void SelectNode (CNode node) {
		if (node.effectors.Count > 0) {
			for (int i = 0; i < node.effectors.Count; i++) {
				node.effectors [i].ActEffect (chat.dict);
			}
		}
		current = node;
	}


	private List<CNode> Branch (CNode node) {

		List <CNode> q = new List<CNode> ();

		for (int i = 0; i < n.Count; i++) {
			if (node.connected.Contains (i))
				q.Add (n [i]);
		}

		if (q.Count > 0) {

			List <CNode> s = new List<CNode> ();

			for (int c = 0; c < q.Count; c++) {

				if (q [c].type == "P") {

					//If it is a player node just add it to s
					s.Add (q [c]);

				} else if (q [c].type == "NPC") {

					// If it is an NPC node then you have to check the conditions of this node

					s.Add (q [c]);


				} else if (!q [c].startNode && q [c].type == "B") {

					if (q [c].checkers.Count > 0) {

						bool passedAll = true;

						// If the node passes all checks then it is added to the 's' list

						for (int checks = 0; checks < q [c].checkers.Count; checks++) {
							if (!q [c].checkers [checks].passCheck (chat.dict)) {
								passedAll = false;
							}
						}

						if (passedAll)
							s.Add (q [c]);


					} else {
						s.Add (q [c]);
					}

				}

			}

			return s;
		} else {
			return null;
		}

	}


	private CNode FindStart() {

		List<CNode> q = new List<CNode> ();

		//Gets all nodes of type 'B'
		for (int i = 0; i < n.Count; i++) {
			if (n [i].type == "B" && n[i].startNode == true)
				q.Add (n [i]);
		}

		//Puts those in a 'q' list

		List<CNode> s = new List<CNode> ();
		//Go through 'q' list
		for (int r = 0; r < q.Count; r++) {

			// If the node has conditions lets check it

			if (q [r].checkers.Count > 0) {
				bool passedAll = true;

				// If the node passes all checks then it is added to the 's' list

				for (int checks = 0; checks < q [r].checkers.Count; checks++) {
					if (!q [r].checkers [checks].passCheck (chat.dict)) {
						passedAll = false;
					}
				}

				if (passedAll)
					s.Add (q [r]);

			} else {

				// If the node has no conditions it is also added to the 's' list

				s.Add (q [r]);
			}
		}

		// Now a random node from 's' is picked

		return s [Random.Range (0, s.Count)];


	}



}
