/*
 * Copyright (c) 2017 Razeware LLC - Jeff Fisher
 */

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
	private SpriteRenderer bg;

	public GameObject tile;
	public int xSize, ySize;

	private GameObject[,] tiles;

	public bool IsShifting { get; set; }

	void Start () {
		instance = GetComponent<BoardManager>();
		bg = GetComponent<SpriteRenderer>();

		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }
	
	public void OnTouch(InputAction.CallbackContext ctx){
		if(ctx.started)
		{
			Camera mainCam = Camera.main;
			Vector2 pointerPos = Mouse.current.position.ReadValue();
			pointerPos = mainCam.ScreenToWorldPoint(pointerPos);
			RaycastHit2D hit = Physics2D.Raycast(pointerPos, Vector2.zero);
			Debug.Log("Raycast @ " + pointerPos);
			if (hit.collider != null) {
				Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
				StartCoroutine(ScaleMe(hit.transform));
			}
		}
	}
	IEnumerator ScaleMe(Transform objTr) {
		objTr.localScale *= 1.2f;
		yield return new WaitForSeconds(0.5f);
		objTr.localScale /= 1.2f;
	}
	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x - (bg.bounds.extents.x * 0.8f);
		float startY = transform.position.y - (bg.bounds.extents.y * 0.8f);
		
		int[] prevLeft = new int[ySize];
		int prevBelow = -1;

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				int tileColor = 0;
				
				List<int> possibleColors = new List<int>(){0, 1, 2, 3, 4, 5};
				
				possibleColors.Remove(prevLeft[y]);
				possibleColors.Remove(prevBelow);

				tileColor = possibleColors[Random.Range(0, possibleColors.Count)];
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
				newTile.GetComponent<Tile>().color = tileColor;
				Animator tileAnim = newTile.GetComponent<Tile>().animator;
				tiles[x, y] = newTile;
				newTile.transform.parent = transform;
				
				prevLeft[y] = tileColor;
				prevBelow = tileColor;
			}
		}
    } // endof CreateBoard

}
