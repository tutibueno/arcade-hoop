using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSettings{

	public int countdownParaComecar;
	public int countdownEntreFases;
	public int tempoHabilitarBonus;
	public string mensagemCountDown = "Vai!"; //3, 2, 1, Vai!
	public int tempoSolicitarLevantarBaixarRampaNoCountDown = 2;
	public KeyCode teclaAcertarCesta = KeyCode.Alpha2;
	public KeyCode teclaAdicionarCredito = KeyCode.Alpha5;
	public KeyCode teclaStartGame = KeyCode.Alpha1;
	public KeyCode teclaRampa = KeyCode.R;
	public bool ligaNumLock = true;
	public bool ligaCapsLock = true;
	public Fase[] fases;
}



