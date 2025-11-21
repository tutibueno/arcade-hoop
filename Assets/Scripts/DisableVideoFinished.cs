using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class DisableVideoFinished : MonoBehaviour {

	private VideoPlayer videoPlayer;

	// Use this for initialization
	void Awake () {

		videoPlayer = GetComponent<VideoPlayer>();

		if(!videoPlayer)
			Debug.LogError("Componente de video nao encontrado!");

		videoPlayer.loopPointReached += OnReachEnd;

		gameObject.SetActive(false);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable()
	{
		videoPlayer.Play();
	}

	

	void OnReachEnd(VideoPlayer vp){
		videoPlayer.Stop();
		gameObject.SetActive(false);
	}
}
