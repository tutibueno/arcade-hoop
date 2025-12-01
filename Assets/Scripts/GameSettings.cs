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
	public Fase[] fases;
}



