using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMsg : MonoBehaviour {

	private Text debugTxt;
	private CanvasGroup debugCanvas;
	public float tempoLimite = 4;
	public CanvasGroup debugAlertCanvas;
	public CanvasGroup debugEstadoRampaCanvas;
	public Text estadoRampaTxt;

	float tempoRestante;
	static DebugMsg debugMsg;
	public static DebugMsg Instance {get{return debugMsg;}}

	bool isDebugging;

	// Use this for initialization
	void Awake () {
		debugMsg = this;
		debugTxt = GetComponent<Text> ();
		debugCanvas = debugTxt.GetComponent<CanvasGroup> ();

		debugTxt.text = "";
	}
	
	public void Log (string content){
		debugTxt.text = content;
		tempoRestante = tempoLimite;
	}

	public void Update(){

		if (Input.GetKeyDown (KeyCode.D)) {
			isDebugging = !isDebugging;
		}

		if (isDebugging) {

			estadoRampaTxt.text = GameManager.instance.GetEstadoRampa.ToString ();
		
			if (debugEstadoRampaCanvas.alpha < 1) 
				debugEstadoRampaCanvas.alpha = 1;
			
			if (debugAlertCanvas.alpha < 1) 
				debugAlertCanvas.alpha = 1;
			
		} else {
			if (debugAlertCanvas.alpha > 0)
				debugAlertCanvas.alpha = 0;
			if (debugEstadoRampaCanvas.alpha > 0)
				debugEstadoRampaCanvas.alpha = 0;
			if (debugCanvas.alpha > 0)
				debugCanvas.alpha = 0;
			
		}

		if (tempoRestante > 0 && isDebugging) {
			tempoRestante -= Time.deltaTime;
			debugCanvas.alpha = 1;
		} else
			if (debugCanvas.alpha > 0)
				debugCanvas.alpha -= Time.deltaTime;
	}

}
