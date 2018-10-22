using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

	//CHAT SYSTEM
	public float hate;
	public int stage;
	public float intelligence;
	public NPC_Chat chat;
	[HideInInspector]
	public NPC_Chat loadChat;

	//Trigger System
	public Transform player;


	public void Trigger() {

	}

}
