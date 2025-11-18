using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class GameManager : MonoBehaviour {

	public enum gameState {
		apresentacao,
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
	private Text highScoreTxt;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	public VideoPlayer videoPlayer;

	[SerializeField]
	private CanvasGroup videoCanvasGroup;

	[SerializeField]
	private string[] videoFiles;

	private float tempoRestante;

	[SerializeField]
	public Text timerText;

	public float tempoLimite = 90f; //Vai ser por fase

	public int pontos;

	public int highScore;

	public int faseAtual;

	public gameState state;

	Color tempoOriginalColor;

	// Use this for initialization
	void Start () {

        tempoOriginalColor = timerText.color;

		ResetGame();

		// Caminho completo da pasta StreamingAssets
		string path = Application.streamingAssetsPath;

		// Pega todos os arquivos .mp4 (ou troque para .mov / .avi)
		videoFiles = Directory.GetFiles(path, "*.mp4");

		if (videoFiles.Length == 0)
		{
			Debug.LogError("Nenhum vídeo encontrado em StreamingAssets!");
		}

		if (videoFiles.Length == 0)
		{
			Debug.LogError("Nenhum vídeo encontrado em StreamingAssets!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		// Contagem regressiva
		if (tempoRestante > 0)
		{
			tempoRestante -= Time.deltaTime;
			if (tempoRestante < 0) tempoRestante = 0;
			UpdateUI();
		}

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
		RemoveCreditos();
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
		pontos += 2;
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
		tempoRestante = tempoLimite;
	}

	void StartGame()
	{
		//verifica se não existe um jogo em andamento
		if(state != gameState.ingame && creditos > 0){
			RemoveCreditos();
			ResetGame();
			state = gameState.ingame;
		}
	}
}
