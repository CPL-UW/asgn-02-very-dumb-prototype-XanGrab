/*
 * Copyright (c) 2017 Razeware LLC - Jeff Fisher
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	public Animator animator;
	public int color;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		render = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
    }

	public void OnTouch(InputAction.CallbackContext ctx){
		if(ctx.started){
			Debug.Log("I've been touched @:" + gameObject.transform.position);
			/*if(render.sprite == null || BoardManager.instance.IsShifting){
				return;
			}

			if(isSelected){
				Deselect();
			}else{
				if(previousSelected == null){
					Select();
				}else{
					previousSelected.Deselect();
				}
			}*/
		}
	}

	private void Select() {
		isSelected = true;
		Debug.Log("I should get darker!");
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

}