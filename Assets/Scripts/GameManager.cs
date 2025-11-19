using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class GameManager : MonoBehaviour {

	public enum gameState {
		attract,
		countdown,
		ingame,
		gameOver
	}

	[SerializeField]
	public Fase[] fases;

	[SerializeField]
	protected int creditos;

	[SerializeField]
	private Text creditosTxt;

    [SerializeField]
    private Text attractTxt;

	[SerializeField]
	private Text highScoreTxt;

	[SerializeField]
	private Text scoreText;
    
	[SerializeField]
    private Text proximaFasePtsTxt;

    [SerializeField]
    private Text faseAtualTxt;

    [SerializeField]
	private Slider faseSlider;

	[SerializeField]
	public VideoPlayer videoPlayer;

	[SerializeField]
	private CanvasGroup videoCanvasGroup;
    [SerializeField]
	CanvasGroup inGameCanvas;
    [SerializeField]
	CanvasGroup gameOverCanvas;
    [SerializeField]
    CanvasGroup attractCanvas;

	[SerializeField]
	private string[] videoFiles;

    [SerializeField]
    private string[] videoFilesAttract;

	private float tempoRestante;

	[SerializeField]
	public Text timerText;

	private float tempoLimite = 0; //Vai ser por fase

	public int pontos;

	public int highScore;

	public int faseAtual;

	public gameState stateAtual;

	int pointsPerShooting = 2;
	int pointsPerShootingBonus = 3;

	Color tempoOriginalColor;

	// Use this for initialization
	void Start () {

        tempoOriginalColor = timerText.color;

		stateAtual = gameState.attract;

		// Caminho completo da pasta StreamingAssets
		string path = Application.streamingAssetsPath + "/Pontuacao";

		// Pega todos os arquivos .mp4 (ou troque para .mov / .avi)
		videoFiles = Directory.GetFiles(path, "*.mp4");

		Debug.Log("Videos de Pontuação encontrados: " + videoFiles.Length);
		
		path = Application.streamingAssetsPath + "/Attract";
		videoFilesAttract = Directory.GetFiles(path, "*.mp4");

        Debug.Log("Videos de Attract encontrados: " + videoFilesAttract.Length);

	}
	
	// Update is called once per frame
	void Update () {
		
		switch (stateAtual){
			case gameState.attract:
                UpdateAttract();
				break;
			case gameState.countdown:
                UpdateCountDown();
				break;
			case gameState.ingame:
				UpdateInGame();
				break;
			case gameState.gameOver:
				UpdateGameOver();
				break;

		}

        UpdateUI();

		GetInputs();

		if (videoPlayer.isPlaying)
			videoCanvasGroup.alpha = 1; 
		else
			videoCanvasGroup.alpha = 0;

	}

	void GetInputs()
	{
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			AddCreditos();
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			AddPontos();
		}

		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			ResetGame();
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			StartGame();
		}
	}

	public void GameOver(){
		videoPlayer.Stop();
		stateAtual = gameState.attract;
		inGameCanvas.alpha = 0;
		gameOverCanvas.alpha = 1;
	}

	public void AddCreditos(){
		creditos++;
		if (creditos > 99)
			creditos = 99;
	}

	public void RemoveCreditos(){
		creditos--;
		if (creditos < 0)
			creditos = 0;
	}

	void PlayRandomVideo()
	{
		if (videoFiles.Length == 0) return;

		string randomVideo = videoFiles[Random.Range(0, videoFiles.Length)];

		// Configura o VideoPlayer
		videoPlayer.source = VideoSource.Url;
		videoPlayer.url = randomVideo;
		videoPlayer.isLooping = false;
		videoPlayer.Play();

		Debug.Log("Reproduzindo vídeo: " + randomVideo);

	}

	void AddPontos()
	{
		if(stateAtual != gameState.ingame)
			return;
		
		pontos += tempoRestante > 15 ? pointsPerShooting : pointsPerShootingBonus;
		
		if (pontos > highScore)
			highScore = pontos;

		

		PlayRandomVideo ();
	}

	void UpdateUI()
	{
		int minutes = Mathf.FloorToInt(tempoRestante / 60f);
		int seconds = Mathf.FloorToInt(tempoRestante % 60f);

		scoreText.text = pontos.ToString();
		timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
		highScoreTxt.text = highScore.ToString ();
		creditosTxt.text = creditos.ToString ();

		faseAtualTxt.text = (faseAtual + 1).ToString();

		proximaFasePtsTxt.text = (fases[faseAtual].pontosParaProximaFase - pontos).ToString();

        if (tempoRestante <= 5f)
        {
            float scale = 1f + Mathf.PingPong(Time.time * 4f, 0.2f);
            timerText.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            timerText.transform.localScale = Vector3.one;
        }


        // Mudar cor quando faltam 5 segundos
        if (tempoRestante <= 20f)
            timerText.color = Color.red;
        else
            timerText.color = tempoOriginalColor;

    }

	void ResetGame()
	{
		pontos = 0;
		faseAtual = 0;
		tempoRestante = 0;
		videoPlayer.Stop();
		tempoRestante += fases[0].tempoAdicional;
	}

	void StartGame()
	{
		//verifica se não existe um jogo em andamento
		if(stateAtual != gameState.ingame && creditos > 0){
			RemoveCreditos();
			ResetGame();
			stateAtual = gameState.ingame;
			attractCanvas.alpha = 0;
			gameOverCanvas.alpha = 0;
			faseAtual = 0;
		}
	}

	void UpdateInGame()
	{

		inGameCanvas.alpha = Mathf.Lerp(inGameCanvas.alpha,1,Time.deltaTime*1.4f);

        // Contagem regressiva
        if (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
            if (tempoRestante < 0) {
				tempoRestante = 0;
				stateAtual = gameState.gameOver;
			}

        }

		float p = (float)pontos / (float)fases[faseAtual].pontosParaProximaFase;

        faseSlider.value = Mathf.Lerp(faseSlider.value, p, Time.deltaTime*2);


        if(pontos >= fases[faseAtual].pontosParaProximaFase){
			faseAtual++;
            faseSlider.value = 0;
		}

		
	}

	float t = 0;
	float blinkSpeed = 4;
	int currentAttractVideoIndex;
	void UpdateAttract()
	{
        t = Mathf.Sin(Time.time * blinkSpeed);
	
        attractCanvas.alpha = Mathf.Lerp(1f, 0.2f, t);

		if(creditos > 0)
            attractTxt.text = "APERTE START!!";	
		else
			attractTxt.text = "INSIRA CRÉDITOS PARA COMEÇAR!";

		
		if(!videoPlayer.isPlaying){
			if(currentAttractVideoIndex > videoFilesAttract.Length - 1)
                currentAttractVideoIndex = 0;

            videoPlayer.url = videoFilesAttract[currentAttractVideoIndex];
            videoPlayer.isLooping = false;
			videoPlayer.Play();

			currentAttractVideoIndex++;
		}
	}

	void UpdateCountDown()
	{

	}

	void UpdateGameOver()
	{

	}
}
