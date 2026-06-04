using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(menuName = "Custom Assets/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public Dialogue Dialogue;
    public bool hasChoices = false;

    [ConditionalHide("hasChoices")] public List<Choice> Choices;

    public bool hasSceneChange = false;

    [ConditionalHide("hasSceneChange")] public String targetSceneName;


    public void ResetParts()
    {
        Dialogue.DialogueParts.Clear();
        Dialogue.DialogueCSV = null;
    }
    
    /// <summary>
    /// Ensures that data in the Dialogue object remains consistent and initializes necessary fields.
    /// </summary>
    private void OnValidate()
    {
        // Ensure the Dialogue contains parts; initialize if empty.
        if (Dialogue.DialogueParts.Count == 0) TextElaboration();

        // Sync choices with the Dialogue object.
        Dialogue.HasChoices = hasChoices;
        Dialogue.DialogueChoices = Choices;

        // Sync the sceneChange with the Dialogue object
        Dialogue.HasSceneChange = hasSceneChange;
        Dialogue.TargetSceneName = targetSceneName;

        // Ensure that each Sentence's `_sImage` array has exactly 6 elements.
        foreach (var part in Dialogue.DialogueParts)
        {
            foreach (var sentence in part.Sentences)
            {
                if (sentence.SImage.Length != 6)
                {
                    Debug.LogWarning("Don't change the 'ints' field's array size!");
                    Array.Resize(ref sentence._sImage, 6);
                }
            }
        }
    }

    /// <summary>
    /// Converts text from a CSV file into Dialogue data.
    /// </summary>
    private void TextElaboration()
    {
        if (Dialogue.DialogueCSV == null) return;

        TextAsset asset = Dialogue.DialogueCSV;
        string[] lines = asset.text.Split('\n');

        List<Line> ListLines;

        // Parse the CSV file into a list of lines.
        PrepareListLine(out ListLines, lines);

        // Create Dialogue objects from the parsed lines.
        CreateDialogue(ListLines);
    }

    /// <summary>
    /// Parses lines from the CSV file into structured data.
    /// </summary>
    /// <param name="ListLines">Output list of parsed lines.</param>
    /// <param name="lines">Array of CSV file lines.</param>
    private void PrepareListLine(out List<Line> ListLines, string[] lines)
    {
        ListLines = new List<Line>();

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split('_');

            if (parts.Length != 11)
            {
                Debug.LogError("Number of fields incorrect at line: " + i);
                return;
            }

            Line line = new Line();

            line.PG = new string[6];

            line.OSTName = parts[(int)Field.OSTName];
            line.OSTSecondStartingLoop = parts[(int)Field.OSTStartLoop];
            line.Audio = parts[(int)Field.SFX];

            for (int x = 0; x < 6; x++)
            {
                line.PG[x] = parts[3 + x];
            }

            line.PGName = parts[(int)Field.PGName];
            line.Sentence = parts[(int)Field.Sentence];
            ListLines.Add(line);
        }
    }

    /// <summary>
    /// Converts parsed lines into Dialogue objects.
    /// </summary>
    /// <param name="ListLines">List of parsed lines.</param>
    private void CreateDialogue(List<Line> ListLines)
    {
        bool musicFound = false;
        for (int i = 0; i < ListLines.Count; i++)
        {
            Line line = ListLines[i];
            string name = line.PGName;
            int j = i + 1;
            List<Sentence> strSentences = new List<Sentence>();
            Sentence strSentence = new Sentence(line.Sentence, TextToSprite(line.PG), TextToAudioClip(line.Audio, false));
            strSentences.Add(strSentence);
            if (!musicFound)
            {
                String OSTName = line.OSTName;
                if (StringCsvValorized(OSTName)){
                    float seconds = float.Parse(line.OSTSecondStartingLoop, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"));
                    Dialogue.DialogueMusicBackground = TextToAudioClip(OSTName, true);
                    Dialogue.StartingLoopPoint = seconds;
                    musicFound = true;
                }
            }
            while (j < ListLines.Count)
            {
                if (StringCsvValorized(ListLines[j].PGName )) break;
                strSentence = new Sentence(ListLines[j].Sentence, TextToSprite(ListLines[j].PG), TextToAudioClip(line.Audio,false));
                strSentences.Add(strSentence);
                j++;
            }

            if (j - 1 != i) i = --j;

            Monologue monologue = new Monologue(name, strSentences);
            Dialogue.DialogueParts.Add(monologue);
        }
    }

    /// <summary>
    /// Converts sprite references in text form to Sprite objects.
    /// </summary>
    /// <param name="strSprite">Array of sprite paths as strings.</param>
    /// <returns>Array of Sprite objects.</returns>
    private Sprite[] TextToSprite(string[] strSprite)
    {
        Sprite[] xSprite = new Sprite[6];

        for (int i = 0; i < strSprite.Length; i++)
        {
            if (!StringCsvValorized(strSprite[i])) continue;

            string strSpritePath = "2D/Character Sprites/";

            string[] strSpriteParts = strSprite[i].Split('-'); // Split sprite string into name and emotion.
            strSpritePath += strSpriteParts[0]; // Get name only.

            strSpritePath += "/S_"; // Prepare file name.
            strSpritePath += strSprite[i];

            // Load sprite from the Resources folder.
            Sprite sprite = Resources.Load<Sprite>(strSpritePath);

            xSprite[i] = sprite;
        }
        return xSprite;
    }

    // Supporting structs and enums

    /// <summary>
    /// Represents a line parsed from the CSV file.
    /// </summary>
    private struct Line
    {
        public string OSTName;
        public string OSTSecondStartingLoop;
        public string Audio;
        public string[] PG;
        public string PGName;
        public string Sentence;
    }

    /// <summary>
    /// Enum for indexing fields in the CSV file.
    /// </summary>
    private enum Field
    {
        OSTName,
        OSTStartLoop,
        SFX,
        PG1,
        PG2,
        PG3,
        PG4,
        PG5,
        PG6,
        PGName,
        Sentence
    }

    private bool StringCsvValorized(string str)
    {
        return !String.IsNullOrEmpty(str) && !"aaa".Equals(str);
    }

    /**
     * when isOST = false the sound will be searched in the SFX folder
     */
    private AudioClip TextToAudioClip(string fileName, bool isOST)
    {
        if (StringCsvValorized(fileName))
        {
            string strAudioPath = "Audio/";
            strAudioPath += isOST ? "OST/OST_" : "SFX/SFX_";
            strAudioPath += fileName;

            return Resources.Load(strAudioPath) as AudioClip;
        }
        return null;
    }

   
}
