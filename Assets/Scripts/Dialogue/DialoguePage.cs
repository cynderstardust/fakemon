
[System.Serializable]
public class DialoguePage
{
    public string text;
    public DialogueChoice[] choices;
    public DialogueAction action;
}

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public DialogueAction action;
}

[System.Serializable]
public class DialogueAction
{
    public DialogueActionType type;
    public int goToPage;
}

[System.Serializable]
public enum DialogueActionType
{
    GoToNextDialoguePage,
    CloseDialogue,
    GetItem,
    ChangeScene,
    StartBattle,
    GoToDialoguePage,

    
}
