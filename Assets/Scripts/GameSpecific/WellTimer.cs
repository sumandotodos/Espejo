using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WellTimer : MonoBehaviour {

	public ScrambledWordGame.SWGGameController timerListener_N;
	public ScreenEController timerListenerE_N;

	public Texture[] images;

	public RawImage imageBottom;
	public RawImage imageTop;

	bool even;

	bool gameOver = false;

	float blend;
	float targetBlend;

	public float blendSpeed;

	public float frameTime;

	public float elapsedTime;
	public bool stopped;

	public int nextImage;

	float opacity;

	public bool autostart;

	float elapsedFrameTime;

	bool started = false;


	// Use this for initialization
	public void Start () {
	
		if (started)
			return;
		started = true;

		stopped = !autostart;

		reset ();

	}
	
	// Update is called once per frame
	void Update () {

		if (stopped)
			return;

		elapsedFrameTime += Time.deltaTime;
		opacity = 1.0f - (elapsedFrameTime / frameTime);
		imageTop.color = new Color (1, 1, 1, opacity);
		if (elapsedFrameTime > frameTime) {

			if (nextImage < images.Length) {

				elapsedFrameTime -= frameTime;
				imageTop.texture = images [nextImage - 1];
				imageBottom.texture = images [nextImage];
				++nextImage;

			} else {
				stopped = true;
				if (timerListener_N != null) {
					ScrambledWordGame.SWGGameController tl = timerListener_N;
					if (tl != null)
						tl.timeup ();
				}
				if (timerListenerE_N != null) {
					timerListenerE_N.timeup ();
				}
			}

		}


	}

	public void stop() {

		stopped = true;

	}

	public void go() {

		stopped = false;

	}

	public void reset() {

		even = true;
		blend = 0.0f;
		elapsedTime = 0.0f;

		elapsedFrameTime = 0.0f;

		imageTop.texture =  images [0];
		imageBottom.texture = images [1];

		opacity = 1.0f;

		nextImage = 2;

	}
}
