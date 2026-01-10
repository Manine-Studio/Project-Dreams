using System;
using System.Collections.Generic;
using Misc;
using Progress_System;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    //Conditions needed to show this dialogue
    [SerializeField] private Condition _PreConditions;

    [SerializeField] private TextAsset _DialogueCSV;
    //List of monologues
    [SerializeField] private List<Monologue> _DialogueParts;

    //Conditions unlocked from this dialogue
    [SerializeField] private Condition _PostConditions;

    //Boolean needed to show multiple choices after the dialogue end
    private bool _HasChoices;
    private List<Choice> _DialogueChoices = new List<Choice>();

    //Boolean needed to show and set the name of the scene in which the dialogue will change at the end
    private bool _HasSceneChange;
    private String _TargetSceneName;

    public List<Monologue> DialogueParts { get => _DialogueParts; } 
    public bool HasChoices { get => _HasChoices;  set => _HasChoices = value; }
    public List<Choice> DialogueChoices { get => _DialogueChoices;  set => _DialogueChoices = value; }
    public Condition PreConditions { get => _PreConditions; }
    public Condition PostConditions { get => _PostConditions; }
    public TextAsset DialogueCSV { get => _DialogueCSV; set => _DialogueCSV = value; }
    public bool HasSceneChange { get => _HasSceneChange; set => _HasSceneChange = value; }
    public String TargetSceneName { get => _TargetSceneName; set => _TargetSceneName = value; }
}

[System.Serializable]
public class Monologue
{
    [SerializeField] private string _sName;
    [SerializeField] private List<Sentence> _Sentences;

    public string SName { get => _sName; }
    public List<Sentence> Sentences { get => _Sentences; }

    public Monologue(string sName, List<Sentence> sentences)
    {
        _sName = sName;
        _Sentences = sentences;
    }
}


[System.Serializable]
public class Sentence
{
    [SerializeField] private string _sSentence;
    [SerializeField] private AudioClip _sAudio;
    [SerializeField] public Sprite[] _sImage = new Sprite[6];

    public string SSentence { get => _sSentence;}
    public AudioClip SAudio { get => _sAudio;}
    public Sprite[] SImage { get => _sImage;}

    public Sentence(string sSentence, Sprite[] sImage)
    {
        _sSentence = sSentence;
        _sImage = sImage;
    }
}

[System.Serializable]
public class Choice
{
    [SerializeField] private string _sLabel;
    [SerializeField] private DialogueSO _xDialogueSO;

    public string ChoiceLabel { get => _sLabel; set => _sLabel = value; }
    public DialogueSO ChoiceDialogueSO { get => _xDialogueSO; set => _xDialogueSO = value; }

}
