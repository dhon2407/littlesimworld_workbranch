using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CustomOutlineShader : MonoBehaviour {

	[SerializeField] bool ShowCachedSettings;
	[ShowIf("ShowCachedSettings"), SerializeField] Sprite spr;
	[ShowIf("ShowCachedSettings"), SerializeField] SpriteRenderer outlineRenderer;
	[ShowIf("ShowCachedSettings"), SerializeField] Material outlineMaterial;

	public bool AutoSolveWidth = true;

	[HideIf("AutoSolveWidth"), Range(0, 20)] public int Width;
	[HideIf("AutoSolveWidth"), Range(10, 100)] public float Speed;

	public Color clr = Color.green;
	public Color clr2 = Color.yellow;



	void Awake() {
		if (AutoSolveWidth) { AutoSolveWidth = false; }
	}
	void OnMouseOver() {
		isMouseOver = true;
	}

	void OnMouseExit() {
		isMouseOver = false;
	}

	bool isMouseOver;
	void Update()
	{
		if (!spr) { return; }
		var cur = outlineMaterial.GetFloat("_Width");

		float tar = 0;
		if (!isMouseOver) { tar = Mathf.MoveTowards(cur, 0, 2 * Speed * Time.deltaTime); }
		else { tar = Mathf.MoveTowards(cur, Width, Speed * Time.deltaTime); }

		Color tarClr = Color.Lerp(clr2, clr, cur / Width);

		outlineMaterial.SetFloat("_Width", tar);
		outlineMaterial.SetColor("_Color", tarClr);
	}

	void InitializeRenderer(GameObject objectPrefab) {
		spr = GetComponent<SpriteRenderer>().sprite;

		if (!spr.texture.isReadable) {
			Debug.Log($"Texture of {this.gameObject.name} isn't readable.");
			this.enabled = false;
			return;
		}

		var sprGO = GameObject.Instantiate(objectPrefab);
		sprGO.transform.parent = transform;
		sprGO.transform.localPosition = Vector3.zero - Vector3.up * 0.0001f;
		sprGO.transform.localScale = Vector3.one;

		outlineRenderer = sprGO.GetComponent<SpriteRenderer>();
		outlineRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
		outlineRenderer.spriteSortPoint = SpriteSortPoint.Pivot;

		outlineMaterial = outlineRenderer.material;

		int offset = 50;

		var sprTexture = spr.texture;
		var sprRect = spr.textureRect;

		int sprWidth = (int) sprRect.size.x;
		int sprHeight = (int) sprRect.size.y;

		if (AutoSolveWidth) {

			Width = (int) (10 * (Mathf.Min(sprWidth, sprHeight) / 440f));
			Speed = 6 * Width;

			offset = 4 * Width;
		}

		var textureHeight = sprHeight + offset;
		var textureWidth = sprWidth + offset;

		var clearColor = Color.clear;

		var texture = InitializeSprite();
		ImportSprite(spr, 0, offset / 2);

		var rect = new Rect(0, 0, textureWidth, textureHeight);

		var newPivot = new Vector2();
		newPivot.x = spr.pivot.x / sprRect.size.x;
		newPivot.y = spr.pivot.y / sprRect.size.y;
		if (spr.pivot.y < 5) { newPivot.y = (offset / 2f) / textureHeight; }

		//float scaleSize = 1 / 20f;
		//texture.Resize(texture.width * scaleSize, texture.height * scaleSize, TextureFormat.Alpha8);

		outlineRenderer.sprite = Sprite.Create(texture, rect, newPivot, spr.pixelsPerUnit, 1, SpriteMeshType.FullRect);

		outlineMaterial.SetTexture("_MainTex", texture);

		Texture2D InitializeSprite() {

			Texture2D texture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.Alpha8, false);

			// Initialize Texture
			for (int x = 0; x < textureWidth; x++) {
				for (int y = 0; y < textureHeight; y++) {
					texture2D.SetPixel(x, y, clearColor);
				}
			}

			return texture2D;
		}


		void ImportSprite(Sprite spr, int offsetX, int offsetY) {

			int ReadStartX = (int) sprRect.position.x;
			int ReadEndX = ReadStartX + sprWidth;

			int ReadStartY = (int) sprRect.position.y;
			int ReadEndY = ReadStartY + sprHeight;

			int WriteEndY = textureHeight - offsetY;
			int WriteStartY = WriteEndY - sprHeight;

			int WriteStartX = ((textureWidth - sprWidth) - offsetX) / 2;
			int WriteEndX = textureWidth - WriteStartX - offsetX;

			int ReadPixelsX = ReadStartX;
			int ReadPixelsY = ReadStartY;

			for (int x = WriteStartX; x <= WriteEndX; x++) {

				ReadPixelsY = ReadStartY;

				for (int y = WriteStartY; y <= WriteEndY; y++) {

					// safety otherwise unity crashes
					if (ReadPixelsX <= sprTexture.width && ReadPixelsY <= sprTexture.height) {
						var color = sprTexture.GetPixel(ReadPixelsX, ReadPixelsY);
						if (color.a >= 0.8f) { texture.SetPixel(x, y, Color.white); }
					}

					ReadPixelsY++;
					if (ReadPixelsY > ReadEndY) { break; }
				}

				ReadPixelsX++;
				if (ReadPixelsX > ReadEndX) { break; }
			}

			texture.Apply();
		}
	}
}
