/*
 * Copyright (c) 2017 Razeware LLC - Jeff Fisher
 */

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected;

	//[SerializeField]
	private SpriteRenderer render;
	public Animator animator;
	public int color;

	private bool isSelected;
	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	private bool matchFound;

	void Awake() {
		render = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	public void Start()
	{
		animator.SetFloat("Color", color);
	}

	public void Update() {
	}

	public void Touch()
	{ 
		//Debug.Log("Tile(" + gameObject.transform.position + ") touched!");
		if(BoardManager.instance.IsShifting){
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
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}
		}
	}
	
	/**
	 * This method is utilized when swapping tiles to ensure the swapping tiles are adjacent
	 */
	private GameObject GetAdjacent(Vector2 castDir) {
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
		while (hit.collider != null && (int)hit.collider.GetComponent<Tile>().color == color) {
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}
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
				matchingTiles[i].GetComponent<Animator>().SetBool("Pop", true);
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
		SpriteRenderer renderer1 = GetComponent<SpriteRenderer>();
		SpriteRenderer renderer2 = other.GetComponent<SpriteRenderer>();
		if (!renderer1.enabled && !renderer2.enabled)
			return;
		if (renderer1.enabled != renderer2.enabled) {
			bool temp = renderer1.enabled;
			renderer1.enabled = renderer2.enabled;
			renderer2.enabled = temp;
			//Debug.Log("Swapping a null and enabled");
		}
		
		if (color == other.GetComponent<Tile>().color)
			return;
		
		int otherColor = other.GetComponent<Tile>().color;
		other.GetComponent<Tile>().color = GetComponent<Tile>().color;
		other.GetComponent<Animator>().SetFloat("Color", color);
		GetComponent<Tile>().color = otherColor;
		animator.SetFloat("Color", color);

		//TODO: Add SFX
	}

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}
}