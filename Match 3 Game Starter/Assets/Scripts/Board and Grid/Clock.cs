/*
 * Copyright (c) 2017 Razeware LLC - Jeff Fisher
 */

//using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Clock : MonoBehaviour {
	private static Color selectedColor = new Color(.3f, .3f, .3f, 1.0f);
	private static Clock previousSelected;

	//[SerializeField]
	private SpriteRenderer render;
	public Sprite minNums;
	public Sprite hrNums;
	public Animator animator;

	private GameObject MinHand;
	private GameObject HourHand;
	private GameObject Gear;
	private GameObject Face;

	public int hour = 0;
	public int min = -1;
	public bool gear = false;

	private bool isSelected;
	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	private bool matchFound;

	void Awake() {
		render = GetComponent<SpriteRenderer>();
		animator = GetComponentInChildren<Animator>();
	}

	public void Start()
	{
		MinHand = gameObject.transform.GetChild(0).gameObject;
		HourHand = gameObject.transform.GetChild(1).gameObject;
		Gear = gameObject.transform.GetChild(2).gameObject;
		Face = gameObject.transform.GetChild(3).gameObject;

		int start = Random.Range(0, 3);

		switch(start){
			case 0:
			hour = Random.Range(1, 13);
			break;
			case 1:
			min = Random.Range(0, 12) * 5;
			break;
			case 2:
			gear = true;
			break;
		}
		UpdateState();
		//animator.SetFloat("Color", color);
	}

	public void Update() {
	}

	public void UpdateState(){
		if(min < 0 && hour < 1){
			Face.SetActive(false);
		}

		if(min < 0){
			MinHand.SetActive(false);
		}else{
			MinHand.SetActive(true);
			MinHand.transform.Rotate(0.0f, 0.0f, min * 30.0f, Space.Self);
			Face.GetComponent<SpriteRenderer>().sprite = minNums;
		}

		if(hour < 1){
			HourHand.SetActive(false);
		}else{
			HourHand.SetActive(true);
			HourHand.transform.Rotate(0.0f, 0.0f, hour * 30.0f, Space.Self);
			Face.GetComponent<SpriteRenderer>().sprite = hrNums;
		}

		if(!gear){
			Gear.SetActive(false);
		}else{
			Gear.SetActive(true);
		}
	}

	public void Touch()
	{ 
		Debug.Log("Tile(" + gameObject.transform.position + ") touched!");
		if(HandyBoardManager.instance.IsShifting){
			return;
		}

		if(isSelected){
			Deselect();
		}else{
			if(previousSelected == null){ 
				Select();
			}else{
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject)) {
					SwapTile(previousSelected.gameObject);
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
				} else {
					previousSelected.GetComponent<Clock>().Deselect();
					Select();
				}
			}
		}
	}
	
	/**
	 * This method is utilized when swapping tiles to ensure the swapping tiles are adjacent
	 */
	public GameObject GetAdjacent(Vector2 castDir) {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		if (hit.collider != null) {
			return hit.collider.gameObject;
		}
		return null;
	}

	private List<GameObject> GetAllAdjacentTiles() {
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++) {
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return adjacentTiles;
	}
	
	private List<GameObject> FindMatch(Vector2 castDir) {
		List<GameObject> matchingTiles = new List<GameObject>(); 
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		//TODO: Matching
		//while (hit.collider != null && (int)hit.collider.GetComponent<Tile>().color == color) {
			//matchingTiles.Add(hit.collider.gameObject);
			//hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		//}
		return matchingTiles;
	}

	private void ClearMatch(Vector2[] paths) {
		List<GameObject> matchingTiles = new List<GameObject>();
		for (int i = 0; i < paths.Length; i++) {
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2) {
			for (int i = 0; i < matchingTiles.Count; i++) {
				//Debug.Log(name+"("+color+") popped.");
				//matchingTiles[i].GetComponent<Animator>().SetBool("Pop", true);
				matchingTiles[i].GetComponent<SpriteRenderer>().enabled = false;
			}
			matchFound = true;
		}
	}
	
	public void ClearAllMatches() {
		if (!render.enabled)
			return;
		
		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound) {
			render.enabled = false;
			//Debug.Log(name +"("+color+") popped.");
			matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			//TODO: add SFX
		}
	}
	
	public void SwapTile(GameObject other) { // 1
		Debug.Log("Swap ME!");
		Vector3 tempPos = other.transform.position;
		other.transform.position = gameObject.transform.position;
		gameObject.transform.position = tempPos;
		//SpriteRenderer renderer1 = GetComponent<SpriteRenderer>();
		//SpriteRenderer renderer2 = other.GetComponent<SpriteRenderer>();
		//if (!renderer1.enabled && !renderer2.enabled)
			//return;
		//if (renderer1.enabled != renderer2.enabled) {
			//bool temp = renderer1.enabled;
			//renderer1.enabled = renderer2.enabled;
			//renderer2.enabled = temp;
			////Debug.Log("Swapping a null and enabled");
		//}
		

		// Swap the names
		string tempName = gameObject.name;
		gameObject.name = other.name;
		other.name = tempName;
		//if (color == other.GetComponent<Clock>().color)
			//return;
		
		//int otherColor = other.GetComponent<Tile>().color;
		//other.GetComponent<Clock>().color = GetComponent<Clock>().color;
		//other.GetComponent<Animator>().SetFloat("Color", color);
		//GetComponent<Clock>().color = otherColor;
		//animator.SetFloat("Color", color);

		//TODO: Add SFX
	}

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Clock>();
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}
}