using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO;

public class TransicaoFase : MonoBehaviour {

	private CanvasGroup canvasGroupPrincipal;

    [SerializeField]
	private CanvasGroup[] canvasTransicaoMensagens;

    [SerializeField]
    private Text[] linhasMensagens;

	private bool isInTransition;

	public bool IsInTransition {get{
		return isInTransition;
	}}

	[SerializeField]
	private string[] videoFilesFase;

	[SerializeField]
	private RectTransform countDownRect;

	private Text countDownTxt;

	private CanvasGroup countDownCanvas;

	[SerializeField]
	private VideoPlayer videoPlayer;

    public delegate void MyCustomEventHandler();

	public event MyCustomEventHandler OnTransicaoFaseFinished;
	

	void Awake (){
		canvasGroupPrincipal = GetComponent<CanvasGroup>();
		canvasGroupPrincipal.alpha = 0;
		foreach (var item in canvasTransicaoMensagens)
		{
			item.alpha = 0;
		}

		string path = Application.streamingAssetsPath + "/Fase";

        videoFilesFase = Directory.GetFiles(path, "*.mp4");

		countDownRect.localScale = Vector3.zero;

		countDownTxt = countDownRect.GetComponent<Text>();

		countDownCanvas = countDownRect.GetComponent<CanvasGroup>();


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

	public void IniciaTransicaoFase(bool faseFinal){
        isInTransition = true;
		StartCoroutine("StartTransicaoCo", faseFinal);
	}

	private IEnumerator StartTransicaoCo(bool faseFinal){

		//Toca o video da fase
		if(!faseFinal)
			videoPlayer.url = videoFilesFase[GameManager.instance.faseAtual];
		else
            videoPlayer.url = GameManager.instance.GetVideoFimJogo();

		videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        canvasGroupPrincipal.alpha = 1;

		videoPlayer.Play();

        // Espera o vídeo começar
        //yield return new WaitUntil(() => videoPlayer.isPlaying);

        // Aguarda terminar
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        //Habilita as mensagens
		if(faseFinal) {
            linhasMensagens[0].text = "Parabéns!";
            linhasMensagens[1].text = "Chegou ao Fim do Jogo!";
            linhasMensagens[2].text = "Até a Próxima Partida!";
		}
		else {
            linhasMensagens[0].text = "Parabéns!";
            linhasMensagens[1].text = "Prepare-se";
            linhasMensagens[2].text = "Para a Próxima Fase!";
		}

		foreach (var item in canvasTransicaoMensagens)
		{
            LeanTween.alphaCanvas(item, 1, 1);

			yield return new WaitUntil(() => item.alpha >= 1);
		}

		yield return new WaitForSeconds(1);

        foreach (var item in canvasTransicaoMensagens)
        {
            item.alpha = 0;
        }

		if(!faseFinal){


			int countDown = GameManager.instance.GameSettings.countdownEntreFases;
            //CountDown
            for (int i = countDown; i >= 0; i--)
            {
                countDownTxt.text = i > 0 ? i.ToString() : "Vai!";

                countDownRect.localScale = Vector3.zero;

                countDownCanvas.alpha = 1;

                LeanTween.scale(countDownRect, Vector3.one * 4f, 1f);

                LeanTween.alphaCanvas(countDownCanvas, 0, 1f);

                yield return new WaitForSeconds(1);

            }

		}

		

        countDownRect.localScale = Vector3.zero;
		
		isInTransition = false;

        canvasGroupPrincipal.alpha = 0;

		if(OnTransicaoFaseFinished != null)
    		OnTransicaoFaseFinished();

    	yield return null;
	}
}
