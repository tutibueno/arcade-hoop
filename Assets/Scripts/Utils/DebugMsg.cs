using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMsg : MonoBehaviour {

	public Text debugTxt;
	private CanvasGroup debugCanvas;
	public static float tempoLimite = 4; //tempo que a mensagem de debug fica visível
	public Text estadoRampaTxt;
	private CanvasGroup debugTxtCanvas;

	public Text capslockTxt;
	public Text numlockTxt;

	static float tempoRestante;

	bool isDebugging;

	static string _content;

	// Use this for initialization
	void Awake () {
		debugCanvas = GetComponent<CanvasGroup> ();
		debugTxtCanvas = debugTxt.GetComponent<CanvasGroup> ();

		debugTxt.text = "";
	}
	
	public static void Log (string content){
		_content = content;
		tempoRestante = tempoLimite;
	}

	public void Update(){

		if (Input.GetKeyDown (KeyCode.D)) {
			isDebugging = !isDebugging;
		}

		if (isDebugging) {

			debugTxt.text = _content;

			estadoRampaTxt.text = GameManager.instance.GetEstadoRampa.ToString ();

			debugCanvas.alpha = 1;


			if (tempoRestante > 0) {
				tempoRestante -= Time.deltaTime;
				debugTxtCanvas.alpha = 1;
			} else
				debugTxtCanvas.alpha = 0;

			numlockTxt.text = KeyboardHelper.GetNumlockState() ? "Ligado" : "Desligado";
			capslockTxt.text = KeyboardHelper.GetCapslockState() ? "Ligado" : "Desligado";

		}
		else
			if(debugCanvas.alpha > 0)
				debugCanvas.alpha = 0;


	}

}
