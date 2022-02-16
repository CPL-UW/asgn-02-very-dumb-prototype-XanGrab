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
	private static Tile previousSelected = null;

	//[SerializeField]
	private SpriteRenderer render;
	public Animator animator;
	public int color;

	private bool isSelected = false;
	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		render = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	public void Start()
	{
		animator.SetFloat("Color", color);
	}
	public void Touch()
	{ 
		Debug.Log("Tile(" + gameObject.transform.position + ") touched!");
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
					SwapColor(previousSelected.animator);
					previousSelected.Deselect();
				} else {
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}
		}
	}
	
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
	
	public void SwapColor(Animator other) { // 1
		if (color == (int) other.GetFloat("Color")) { // 2
			return;
		}

		int tempColor = (int) other.GetFloat("Color"); // 3
		other.SetFloat("Color", this.color);
		animator.SetFloat("Color", tempColor);
		//TODO: Add SFX
	}

	private void Select() {
		isSelected = true;
		//Debug.Log("I should be selected");
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
	}

	private void Deselect() {
		isSelected = false;
		//Debug.Log("I should be deselected");
		render.color = Color.white;
		previousSelected = null;
	}
}