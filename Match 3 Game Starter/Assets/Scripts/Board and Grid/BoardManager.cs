/*
 * Copyright (c) 2017 Razeware LLC - Jeff Fisher
 */

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Random = UnityEngine.Random;

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
			
			Collider2D[] hits = Physics2D.OverlapPointAll(pointerPos);
			
			//Debug.Log("Raycast @ " + pointerPos);
			foreach (var hit in hits)
			{
				// check if hit is a tile
				if (hit.gameObject.CompareTag("Tile")){
					hit.GetComponent<Tile>().Touch();
				}
				//Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
				StartCoroutine(ScaleMe(hit.transform));
			}
		}
	}

	IEnumerator ScaleMe(Transform objTr) {
		objTr.localScale *= 1.2f;
		yield return new WaitForSeconds(0.1f);
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
				newTile.GetComponent<Animator>().Play("Grey");
				newTile.GetComponent<Animator>().SetFloat("Color", tileColor);
				
				tiles[x, y] = newTile;
				newTile.transform.parent = transform;
				newTile.name = "Tile" + (Vector2)newTile.gameObject.transform.position;

				prevLeft[y] = tileColor;
				prevBelow = tileColor;
			}
		}
    }
	
	public IEnumerator FindNullTiles() {
		for (int x = 0; x < xSize; x++) {
			Debug.Log("Scanning ");
			for (int y = 0; y < ySize; y++) {
				Debug.Log("Scanning " + tiles[x,y].name);
				if (!tiles[x, y].GetComponent<SpriteRenderer>().enabled) {
					Debug.Log("Start a shift in col " + x );
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}
		
		/*
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}*/
	}
	
	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = 0.25f) {
		IsShifting = true;
		List<GameObject> shiftTiles = new List<GameObject>();
		if (shiftTiles.Count != 0) {
			Debug.LogError("Unexpected");
		}
		int nullCount = 0;

		for (int y = yStart; y < ySize; y++) {
			SpriteRenderer renderer
				= tiles[x, y].GetComponent<SpriteRenderer>();
			if (!renderer.enabled) {
				nullCount++;
			}
			shiftTiles.Add(tiles[x,y]);
		}

		for (int i = 0; i < nullCount; i++) {
			yield return new WaitForSeconds(shiftDelay);
			if (nullCount == shiftTiles.Count) {
				break;
			}
			Debug.Log("NullC " + i);
			for (int k = 0; k < shiftTiles.Count - 1; k++) {
				shiftTiles[k].GetComponent<Tile>().SwapTile(shiftTiles[k + 1]);
				Debug.Log("Tiles to shift: " + shiftTiles.Count);
				//shiftTiles[k + 1].GetComponent<SpriteRenderer>().enabled = false;
				
				//Debug Actions
				if(shiftTiles[k].GetComponent<SpriteRenderer>().enabled)
					StartCoroutine(ScaleMe(shiftTiles[k].transform));
				if(shiftTiles[k + 1].GetComponent<SpriteRenderer>().enabled)
					StartCoroutine(ScaleMe(shiftTiles[k + 1].transform));
				yield return new WaitForSeconds(shiftDelay);
			}
		}
		IsShifting = false;
		Debug.Log("Shift ended.");
	}
}