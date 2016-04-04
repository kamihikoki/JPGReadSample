using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MainComponentHandler : MonoBehaviour {

	public Image image;
	
	public PreviewPhotoComponent previewPhotoComponent;

	//string photoName = "kasumi_chan.png";
	
	string[] photoNames = new string[] {"yayoi.png", "mikakunin.png", "imas.png", "kokosake.png"};

	/* Default Methods */


	// Use this for initialization
	void Start () {
		//photoName = photoNames[3];

		CreateSpritesFromPhoto ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	/* Member Methods */


	void CreateSpritesFromPhoto () {
		var sprites = new List<Sprite> ();
		
		foreach (string photoName in photoNames) {
			string path = Application.dataPath + "/" + photoName;
			
			if (File.Exists (path)) {
				var sprite = ReadPng (path);
				sprites.Add (sprite);
			}
		}
		
		previewPhotoComponent.Init (sprites.ToArray ());
	}

	byte[] ReadPngFile(string path){
		var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

		var bin = new BinaryReader(fileStream);
		byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

		bin.Close();

		return values;
	}

	Sprite ReadPng (string path) {
		byte[] readBinary = ReadPngFile (path);

		int pos = 16;

		int width = 0;
		for (int i = 0; i < 4; i++) {
			width = width * 256 + readBinary [pos++];
		}

		int height = 0;
		for (int i = 0; i < 4; i++) {
			height = height * 256 + readBinary [pos++];
		}

		print (width);
		print (height);

		var tex = new Texture2D (width, height);
		tex.LoadImage (readBinary);

		var sprite = Sprite.Create (tex, new Rect (0, 0, width, height), Vector2.zero);

		return sprite;
	}
	
	
	/* OnClick Events */
	
	
	public void OnClickInitButton () {
		photoNames = new string[] {"yayoi.png", "mikakunin.png", "kokosake.png"};
		
		CreateSpritesFromPhoto ();
	}
}