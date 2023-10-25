using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField]
    public Dialogue dialogue = new Dialogue();
    readonly int MAXSTRINGLENGTH = 190;

    // Start is called before the first frame update
    void Start()
    {
        foreach(string page in dialogue.dialogues)
        {
            if (page.Length > MAXSTRINGLENGTH) Debug.LogWarning($"A dialogue on {gameObject.name} is too long at {page.Length} characters - {page}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (DialogueManager.instance.dialogueShowing) return;
        DialogueManager.instance.ShowDialogue(dialogue);
    }
}
