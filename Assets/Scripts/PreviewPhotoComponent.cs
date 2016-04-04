using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreviewPhotoComponent : MonoBehaviour {
	
	public GameObject contents;
	public GameObject closeBtnPrefab;
	
	int currentPhotoIndex = 0;
	int photosCount = 0;
	bool isMove = false;
	
	readonly float photosMargin = 10;
	
	public float animTime;

	/* Default Methods */
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	/* Member Methods */
	
	
	public void Init (Sprite[] sprites) {
		// init slideview.
		foreach (Transform child in contents.transform) {
			Destroy (child.gameObject);
		}
		contents.transform.DetachChildren ();
		contents.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
		
		// init variables.
		currentPhotoIndex = 0;
		
		// Instantiate and reduction.
		foreach (var sprite in sprites) {
			var go = new GameObject("Photo");
			var image = go.AddComponent<Image> ();
			
			image.transform.SetParent (contents.transform);
			image.rectTransform.anchoredPosition = Vector3.zero;
			
			image.sprite = sprite;
			
			ReductionImage (image);
			AddCloseBtnToImage (image);
		}
		photosCount = sprites.Length;
		
		SetSlideView ();
	}
	
	
	// Delete image.
	
	void DeleteImage (Image image) {
		var go = image.gameObject;
		
		for (int i = 0; i < photosCount; i++) {
			if (go == contents.transform.GetChild (i).gameObject) {
				
				// 1枚しか画像が無い時、ただ削除する.
				if (photosCount == 1) {
					Destroy (go);
					
					photosCount--;
					currentPhotoIndex--;
					
					return;
				}
				
				// 選択中でない右端だった時、詰めずにただ削除する.
				if (i == photosCount - 1 && currentPhotoIndex != i) {
					Destroy (go);
					photosCount--;
					
					return;
				}
				
				// 選択中、もしくはそれより右の画像で、右端でない時、右の画像を寄せる.
				if (i >= currentPhotoIndex && i != photosCount - 1) {
					PullRightPhotos (i);
					return;
				// 選択中より左の画像、もしくは右端の時、左の画像を寄せる.
				} else {
					PullLeftPhotos (i);
					return;
				}
			}
		}
	}
	
	void PullRightPhotos (int index) {
		if (index + 1 <= photosCount) {
			float nextImagePosX = contents.transform.GetChild (index + 1).GetComponent<Image> ().rectTransform.anchoredPosition.x;
			float currentImagePosX = contents.transform.GetChild (index).GetComponent<Image> ().rectTransform.anchoredPosition.x;
			
			float moveX = nextImagePosX - currentImagePosX;
			
			for (int i = index + 1; i < photosCount; i++) {
				var image = contents.transform.GetChild (i).GetComponent<Image> ();
				float imagePosX = image.rectTransform.anchoredPosition.x;
				
				image.rectTransform.anchoredPosition = new Vector2 (imagePosX - moveX, 0);
			}
		}
		
		Destroy(contents.transform.GetChild (index).gameObject);
		
		photosCount--;
	}
	
	void PullLeftPhotos (int index) {
		if (index - 1 >= 0) {
			float currentImagePosX = contents.transform.GetChild (index).GetComponent<Image> ().rectTransform.anchoredPosition.x;
			float prevImagePosX = contents.transform.GetChild (index - 1).GetComponent<Image> ().rectTransform.anchoredPosition.x;
			
			float moveX = currentImagePosX - prevImagePosX;
			
			for (int i = index - 1; i >= 0; i--) {
				var image = contents.transform.GetChild (i).GetComponent<Image> ();
				float imagePosX = image.rectTransform.anchoredPosition.x;
				
				image.rectTransform.anchoredPosition = new Vector2 (imagePosX + moveX, 0);
			}
		}
		
		Destroy(contents.transform.GetChild (index).gameObject);
		
		photosCount--;
		currentPhotoIndex--;
	}
	
	
	// Reduction.
	
	void ReductionImage (Image image) {
		image.SetNativeSize ();
		
		float width;
		float height = 200;
		
		int[] aspectRatio = GetAspectRatio (image.rectTransform.sizeDelta.x, image.rectTransform.sizeDelta.y);
		
		float ratio = height / aspectRatio [1];
		
		width = ratio * aspectRatio [0];
		
		image.rectTransform.sizeDelta = new Vector2 (width, height);
	}

	int GCD (int a, int b) {
		int remainder;

		while(b != 0){
			remainder = a % b;
			a = b;
			b = remainder;
		}

		return a;
	}

	int[] GetAspectRatio (float _x, float _y) {
		int x = (int)_x;
		int y = (int)_y;

		return new int [] { x / GCD (x, y), y / GCD (x, y) };
	}
	
	// Add close btn.
	
	void AddCloseBtnToImage (Image image) {		
		float harfWidth = image.rectTransform.sizeDelta.x / 2;
		float harfHeight = image.rectTransform.sizeDelta.y / 2;
		
		var go = Instantiate(closeBtnPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		
		var btn = go.GetComponent<Image> ();
		btn.transform.SetParent (image.transform);
		
		float btnHarfWidth = btn.rectTransform.sizeDelta.x / 2;
		float btnHarfHeight = btn.rectTransform.sizeDelta.y / 2;
		
		btn.rectTransform.anchoredPosition = new Vector2 (harfWidth - btnHarfWidth, harfHeight - btnHarfHeight);
		
		go.GetComponent<Button> ().onClick.AddListener (() => DeleteImage(image));
	}
	
	
	// SlideShow.
	
	void SetSlideView () {
		float lastMovedImgPosX = 0;
		
		for (int i = 1; i < photosCount; i++) {
			var image = contents.transform.GetChild (i).GetComponent<Image> ();
			var prevImage = contents.transform.GetChild (i - 1).GetComponent<Image> ();
			
			float imgHarfWidth = image.rectTransform.sizeDelta.x / 2;
			float prevImgHarfWidth = prevImage.rectTransform.sizeDelta.x / 2;
			
			image.rectTransform.anchoredPosition = new Vector2 (imgHarfWidth + prevImgHarfWidth + photosMargin + lastMovedImgPosX, 0);
			
			lastMovedImgPosX = image.rectTransform.anchoredPosition.x;
		}
	}
	
	public void SlideToPrev () {
		if (currentPhotoIndex == 0 || isMove) {
			return;
		}
		
		var currentPhoto = contents.transform.GetChild (currentPhotoIndex).GetComponent<Image> ();
		var prevPhoto = contents.transform.GetChild (--currentPhotoIndex).GetComponent<Image> ();
		
		float moveX = prevPhoto.rectTransform.anchoredPosition.x - currentPhoto.rectTransform.anchoredPosition.x;
		
		iTween.ValueTo (gameObject, iTween.Hash ("from", contents.GetComponent<RectTransform>().anchoredPosition.x,
											     "to",  contents.GetComponent<RectTransform>().anchoredPosition.x - moveX,
											     "time", animTime,
											     "onupdate", "OnUpdateSlide",
											     "oncomplete", "OnCompleteSlide"
											    ));
		isMove = true;
	}
	
	public void SlideToNext () {
		if (currentPhotoIndex == photosCount - 1 || isMove) {
			return;
		}
		
		var currentPhoto = contents.transform.GetChild (currentPhotoIndex).GetComponent<Image> ();
		var nextPhoto = contents.transform.GetChild (++currentPhotoIndex).GetComponent<Image> ();
		
		float moveX = nextPhoto.rectTransform.anchoredPosition.x - currentPhoto.rectTransform.anchoredPosition.x;
		
		iTween.ValueTo (gameObject, iTween.Hash ("from", contents.GetComponent<RectTransform>().anchoredPosition.x,
											     "to",  contents.GetComponent<RectTransform>().anchoredPosition.x + moveX * -1,
											     "time", animTime,
											     "onupdate", "OnUpdateSlide",
											     "oncomplete", "OnCompleteSlide"
											    ));
		isMove = true;
	}
	
	void OnUpdateSlide (float value) {
		contents.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (value, 0);
	}
	
	void OnCompleteSlide () {
		isMove = false;
	}
}