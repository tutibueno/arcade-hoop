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
		transicaoFase,
		gameOver,
		final
	}

	[SerializeField]
	GameSettings gameSettings;

	public GameSettings GameSettings{get{return gameSettings;}}

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
    private RectTransform countDownRect;

    private Text countDownTxt;

    private CanvasGroup countDownCanvas;

    [SerializeField]
	private Slider faseSlider;

	[SerializeField]
	public VideoPlayer videoPlayer;

	[SerializeField]
	private List<VideoPlayer> videoPlayersAttract;
    
	[SerializeField]
	private List<VideoPlayer> videoPlayersPontuacao;
	
	[SerializeField]
    private List<VideoPlayer> videoPlayersFase;

    [SerializeField]
    private List<VideoPlayer> videoPlayersFinalJogo;

	[SerializeField]
	private CanvasGroup videoCanvasGroup;
    [SerializeField]
	CanvasGroup inGameCanvas;

	[SerializeField]
	CanvasGroup bonusPointsCanvas;

    //GameOver
    [SerializeField]
	CanvasGroup gameOverCanvas;
    
	[SerializeField]
    CanvasGroup gameOverHiscoreCanvas;

    [SerializeField]
	Text hiscoreGameOverTxt;
	//----

    [SerializeField]
    CanvasGroup attractCanvas;

	[SerializeField]
	private string[] videoFiles;

    [SerializeField]
    private string[] videoFilesAttract;

    [SerializeField]
    private string[] videoFilesFinal;

	private float tempoRestante;

	[SerializeField]
	public Text timerText;

	private float tempoLimite = 0; //Vai ser por fase

	public int pontos;

	public int highScore;

	public int faseAtual;

	private TransicaoFase transicaoFase;

	public gameState stateAtual;

	int pointsPerShooting = 2;
	int pointsPerShootingBonus = 3;

	bool isHighscore;

	Color tempoOriginalColor;

	//Singleton
	static GameManager _instance;

	private Coroutine attractCoroutine;

	public static GameManager instance {get{
		return _instance;
	}}

	void Awake() {
        if (_instance == null)
        {
            _instance = this;
        }

		gameSettings = GameConfigLoader.Load ();

	}

	// Use this for initialization
	void Start () {

        tempoOriginalColor = timerText.color;

		ChangeState(gameState.attract);

		transicaoFase = GetComponentInChildren<TransicaoFase>();

        countDownTxt = countDownRect.GetComponent<Text>();

        countDownCanvas = countDownRect.GetComponent<CanvasGroup>();

		// Caminho completo da pasta StreamingAssets
		string path = Application.streamingAssetsPath + "/Pontuacao";

		// Pega todos os arquivos .mp4 (ou troque para .mov / .avi)
		videoFiles = Directory.GetFiles(path, "*.mp4");

		Debug.Log("Videos de Pontuação encontrados: " + videoFiles.Length);

		foreach (var item in videoFiles)
		{
			var go = Instantiate(videoPlayer, Vector3.zero, Quaternion.identity, transform);
			go.url = item;
			go.source = VideoSource.Url;
			go.isLooping = false;
			videoPlayersPontuacao.Add(go);

		}
		
		path = Application.streamingAssetsPath + "/Attract";
		videoFilesAttract = Directory.GetFiles(path, "*.mp4");

        foreach (var item in videoFiles)
        {
            var vid = Instantiate(videoPlayer, Vector3.zero, Quaternion.identity);
            vid.url = item;
			vid.isLooping = false;
            videoPlayersAttract.Add(vid);
			vid.loopPointReached += OnLoopReached;
        }

        path = Application.streamingAssetsPath + "/Final";

		videoFilesFinal = Directory.GetFiles(path, "*.mp4");

        Debug.Log("Videos de Attract encontrados: " + videoFilesAttract.Length);

		gameOverCanvas.alpha = 0;

		//Subscreve ao evento
		transicaoFase.OnTransicaoFaseFinished += OnTransicaoFaseFinished;

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

        videoCanvasGroup.alpha = videoPlayer.isPlaying ? 1 : 0;

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

		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}

	public void GameOver(){
		videoPlayer.Stop();
		ChangeState(gameState.attract);
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
		
		if (pontos > highScore) {
			highScore = pontos;
			isHighscore = true;
		}

		

		PlayRandomVideo ();
	}

	void UpdateUI()
	{

        highScoreTxt.text = highScore.ToString();
        creditosTxt.text = creditos.ToString();

		if(stateAtual != gameState.ingame)
			return;

		int minutes = Mathf.FloorToInt(tempoRestante / 60f);
		int seconds = Mathf.FloorToInt(tempoRestante % 60f);

		scoreText.text = pontos.ToString();
		timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
		

		faseAtualTxt.text = (faseAtual + 1).ToString();

		proximaFasePtsTxt.text = (Mathf.Clamp(gameSettings.fases[faseAtual].pontos - pontos, 0, Mathf.Infinity)).ToString();

        if (tempoRestante <= 6f)
        {
            float scale = 1f + Mathf.PingPong(Time.time * 4f, 0.2f);
            timerText.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            timerText.transform.localScale = Vector3.one;
        }

		//Cesta de 3 pontos
		if(tempoRestante <= 15){
            bonusPointsCanvas.alpha = Mathf.PingPong(Time.time * 4f,1);
		}
		else
            bonusPointsCanvas.alpha = 0;


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
        isHighscore = false;
		videoPlayer.Stop();
		faseSlider.value = 0;
		tempoRestante = gameSettings.fases[0].tempo;
	}

	void StartGame()
	{
		//verifica se não existe um jogo em andamento
		if(stateAtual == gameState.attract && creditos > 0){
			RemoveCreditos();
			ResetGame();
			ChangeState(gameState.countdown);
			attractCanvas.alpha = 0;
			gameOverCanvas.alpha = 0;
			faseAtual = 0;
			StartCoroutine(StartGameCo());
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
			}
        }

		float p = (float)pontos / (float)gameSettings.fases[faseAtual].pontos;

        faseSlider.value = Mathf.Lerp(faseSlider.value, p, Time.deltaTime*2);

		//Verifica se passou de fase
		if(tempoRestante <= 0){
			if(pontos >= gameSettings.fases[faseAtual].pontos){
				faseAtual++;

				bool faseFinal = faseAtual >= gameSettings.fases.Length;

				//verifica se é a última fase
				if(faseFinal){
                    faseAtual = gameSettings.fases.Length-1;
					ChangeState(gameState.final);
				}
				else
                    ChangeState(gameState.transicaoFase);

				faseSlider.value = 0;
				tempoRestante = gameSettings.fases[faseAtual].tempo;
				videoPlayer.Stop();
                
                LeanTween.alphaCanvas(inGameCanvas, 0, 0.5f).setOnComplete(()=> {transicaoFase.IniciaTransicaoFase(faseFinal); });
				return;
			}
			//Não conseguiu avançar de fase: Game Over
			else{
                ChangeState(gameState.gameOver);
				StartCoroutine(GameOverCo());
                return;
			}
		}

		
	}

	float t = 0;
	float blinkSpeed = 4;
	int currentAttractVideoIndex = 0;
	void UpdateAttract()
	{
        t = Mathf.Sin(Time.time * blinkSpeed);
	
        attractCanvas.alpha = Mathf.Lerp(1f, 0.2f, t);

		if(creditos > 0)
            attractTxt.text = "APERTE START!!";
		else
			attractTxt.text = "INSIRA CRÉDITOS PARA COMEÇAR!";

		
		if(!videoPlayer.isPlaying){

			if(currentAttractVideoIndex >= videoPlayersAttract.Count)
				currentAttractVideoIndex = 0;

			//Desabilita todos os outros videos
			foreach (var item in videoPlayersAttract)
			{
				item.gameObject.SetActive(false);
			}

            videoPlayersAttract[currentAttractVideoIndex].gameObject.SetActive(true);

            videoPlayer = videoPlayersAttract[currentAttractVideoIndex];

            Debug.Log("Tocando o video do indice: " + currentAttractVideoIndex);

            currentAttractVideoIndex++;

            Debug.Log("Proximo Video: " + currentAttractVideoIndex);

        }
		
	}

	void UpdateCountDown()
	{
		

	}

	void UpdateGameOver()
	{
        
	}

    void OnLoopReached(UnityEngine.Video.VideoPlayer vp)
    {
        //vp.playbackSpeed = vp.playbackSpeed / 10.0F;
		Debug.Log("Chegou ao fim do video");
    }

	private IEnumerator TransicaoFase()
	{

		yield return null;

	}

    private IEnumerator StartGameCo()
    {

        //TODO implementar a rotina de aguardar o motor da rampa



        //CountDown
        for (int i = gameSettings.countdownParaComecar; i >= 0; i--)
        {

            countDownTxt.text = i > 0 ? i.ToString() : "Vai!";

            countDownRect.localScale = Vector3.zero;

            countDownCanvas.alpha = 1;

            LeanTween.scale(countDownRect, Vector3.one * 4f, 1f);

            LeanTween.alphaCanvas(countDownCanvas, 0, 1f);

            yield return new WaitForSeconds(1);

        }

        countDownRect.localScale = Vector3.zero;

        ChangeState(gameState.ingame);

        yield return null;

    }

	private IEnumerator FinalCo(){
		
		
		yield return null;
	}

	private IEnumerator GameOverCo(){

		videoPlayer.Stop();

		hiscoreGameOverTxt.text = pontos.ToString();

		gameOverCanvas.alpha = 0;

        gameOverHiscoreCanvas.alpha = 0;

        LeanTween.alphaCanvas(inGameCanvas,0,1).setOnComplete(() => LeanTween.alphaCanvas(gameOverCanvas, 1, 1));
        
		yield return new WaitUntil(()=> !LeanTween.isTweening());

        yield return new WaitForSeconds(2);

		if(isHighscore){
			LeanTween.alphaCanvas(gameOverHiscoreCanvas,1,0.2f).setLoopPingPong();
		}

		yield return new WaitForSeconds(10);

        LeanTween.alphaCanvas(gameOverCanvas, 0, 1);

        yield return new WaitForSeconds(2);

		LeanTween.cancel(gameOverHiscoreCanvas.gameObject);

		stateAtual = gameState.attract;

        yield return null;
	}

	void ChangeState(gameState newState)
	{
		stateAtual = newState;
	}

    void OnTransicaoFaseFinished(){

		if(stateAtual == gameState.final){
            ChangeState(gameState.gameOver);
			StartCoroutine(GameOverCo());
			//
			return;
		}

        ChangeState(gameState.ingame);
	}

	public string GetVideoFimJogo(){
		int rdn = Random.Range(0, videoFilesFinal.Length);
		return videoFilesFinal[rdn];
	}
}
