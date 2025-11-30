using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfigLoader
{
    private static string filePath = Application.streamingAssetsPath +  "/game.ini";

    public static GameSettings Load()
    {
        if (!File.Exists(filePath))
        {
            CreateDefaultFile();
            Console.WriteLine("Arquivo game.ini criado automaticamente.");
        }

        return ParseIni(File.ReadAllLines(filePath));
    }

    private static GameSettings ParseIni(string[] lines)
    {
        GameSettings settings = new GameSettings();
        List<Fase> fasesList = new List<Fase>();

        Fase faseAtual = null;
        string currentSection = "";

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrEmpty(line) || line.StartsWith(";"))
                continue;

            // Detecta seção: [Geral], [Fase0], [Fase1], etc.
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentSection = line.Substring(1, line.Length - 2);

                if (currentSection.StartsWith("Fase"))
                {
                    // Criar nova fase AO ENTRAR NA SEÇÃO
                    faseAtual = new Fase();
					fasesList.Add (faseAtual);
                }

                continue;
            }

            // Parse key=value
            string[] parts = line.Split('=');
            if (parts.Length != 2) continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            // Seção Geral -----------------------------
            if (currentSection == "Geral")
            {
                switch (key)
                {
                    case "countdownParaComecar":
                        settings.countdownParaComecar = int.Parse(value);
                        break;
                    case "countdownEntreFases":
                        settings.countdownEntreFases = int.Parse(value);
                        break;
                    case "tempoHabilitarBonus":
                        settings.tempoHabilitarBonus = int.Parse(value);
                        break;
                }
            }

            // Seções de fase --------------------------
            else if (currentSection.StartsWith("Fase") && faseAtual != null)
            {
                switch (key)
                {
                    case "pontos":
                        faseAtual.pontos = int.Parse(value);
                        break;
                    case "tempo":
                        faseAtual.tempo = int.Parse(value);
                        break;
                }
					
            }
        }

        settings.fases = fasesList.ToArray();
        return settings;
    }

    private static void CreateDefaultFile()
    {
        string example = @";
; CONFIGURAÇÃO DO JOGO
; Edite livremente!

[Geral]
countdownParaComecar = 3
countdownEntreFases = 3
tempoHabilitarBonus = 15

[Fase0]
pontos = 100
tempo = 30

[Fase1]
pontos = 200
tempo = 25

[Fase2]
pontos = 350
tempo = 20
";

        File.WriteAllText(filePath, example);
    }
}
