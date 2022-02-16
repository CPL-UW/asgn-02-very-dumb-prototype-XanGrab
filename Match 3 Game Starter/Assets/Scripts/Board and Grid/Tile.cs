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

	void Awake() {
		render = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	public void Start()
	{
		animator.SetFloat("Color", color);
	}

	public void OnTouch(InputAction.CallbackContext ctx){
		if(ctx.started)
		{
			//Debug.Log("Touch logged @ " + );
		}
	}

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	private void Touch()
	{ 
		Debug.Log("Tile(" + gameObject.transform.position + ") touched!");
		if(BoardManager.instance.IsShifting){
			return;
		}

		if(isSelected){
			Deselect();
		}else{
			if(previousSelected == null){ Select();
			}else{ previousSelected.Deselect(); }
		}
	}
	private void Select() {
		isSelected = true;
		Debug.Log("I should be selected");
		GetComponent<SpriteRenderer>().sharedMaterial.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
	}

	private void Deselect() {
		isSelected = false;
		Debug.Log("I should be deselected");
		GetComponent<SpriteRenderer>().sharedMaterial.color = Color.white;
		previousSelected = null;
	}
}