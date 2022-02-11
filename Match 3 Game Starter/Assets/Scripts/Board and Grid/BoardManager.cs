/*
 * Copyright (c) 2017 Razeware LLC - Jeff Fisher
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
	public GameObject tile;
	public int xSize, ySize;

	private GameObject[,] tiles;

	public bool IsShifting { get; set; }

	void Start () {
		instance = GetComponent<BoardManager>();

		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;
		
		int[] prevLeft = new int[ySize];
		int prevBelow = -1;

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
				tiles[x, y] = newTile;
				newTile.transform.parent = transform;
				
				// Set the tile's color and ensure no starting march-3
				Animator tileAnim = newTile.GetComponent<Tile>().animator;
				int tileColor = newTile.GetComponent<Tile>().color;
				
				List<int> possibleColors = new List<int>(){0, 1, 2, 3, 4, 5};
				
				possibleColors.Remove(prevLeft[y]);
				possibleColors.Remove(prevBelow);

				tileColor = possibleColors[Random.Range(0, possibleColors.Count)];
				switch(tileColor){
					case 0:
						tileAnim.Play("Grey");
						break;
					case 1:
						tileAnim.Play("Blue");
						break;
					case 2:
						tileAnim.Play("Green");
						break;
					case 3:
						tileAnim.Play("Lime");
						break;
					case 4:
						tileAnim.Play("Orange");
						break;
					case 5:
						tileAnim.Play("Red");
						break;
				}
				prevLeft[y] = tileColor;
				prevBelow = tileColor;
			}
		}
    } // endof CreateBoard

}
