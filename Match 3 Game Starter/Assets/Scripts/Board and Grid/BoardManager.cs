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
				newTile.name = "Tile"+"("+x+","+y+")";

				prevLeft[y] = tileColor;
				prevBelow = tileColor;
			}
		}
		FindObjectOfType<AudioManager>().Play("Tetris");
    }
	
	public IEnumerator FindNullTiles() {
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				if (!tiles[x, y].GetComponent<SpriteRenderer>().enabled) {
					//Debug.Log("Start a shift in col " + x );
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}
		
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}
	}
	
	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = 0.25f) {
		IsShifting = true;
		List<GameObject> shiftTiles = new List<GameObject>();
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
				Debug.Log("Trigger top row case");
				for (int k = 0; k < shiftTiles.Count; k++) {
					GetNewTile(shiftTiles[k], x, ySize - 1);
				}
				break;
			}
			for (int k = 0; k < shiftTiles.Count - (1 + i); k++) {
				shiftTiles[k].GetComponent<Tile>().SwapTile(shiftTiles[k + 1]);
				GetNewTile(shiftTiles[k + 1], x, ySize - 1);
				
				//Debug Actions
				if(shiftTiles[k].GetComponent<SpriteRenderer>().enabled)
					StartCoroutine(ScaleMe(shiftTiles[k].transform));
				if(shiftTiles[k + 1].GetComponent<SpriteRenderer>().enabled)
					StartCoroutine(ScaleMe(shiftTiles[k + 1].transform));
				//yield return new WaitForSeconds(shiftDelay);
			}
		}
		IsShifting = false;
		//Debug.Log("Shift ended.");
	}
	private int GetNewTile(GameObject tile, int x, int y) {
		List<int> possibleColors = new List<int>(){0, 1, 2, 3, 4, 5};

		if (x > 0) {
			possibleColors.Remove(tiles[x - 1, y].GetComponent<Tile>().color);
		}
		if (x < xSize - 1) {
			possibleColors.Remove(tiles[x + 1, y].GetComponent<Tile>().color);
		}
		if (y > 0) {
			possibleColors.Remove(tiles[x, y - 1].GetComponent<Tile>().color);
		}

		tile.GetComponent<Tile>().color = possibleColors[Random.Range(0, possibleColors.Count)];
		tile.GetComponent<Animator>().SetFloat("Color", tile.GetComponent<Tile>().color);
		tile.GetComponent<SpriteRenderer>().enabled = true;
		
		return possibleColors[Random.Range(0, possibleColors.Count)];
	}

}