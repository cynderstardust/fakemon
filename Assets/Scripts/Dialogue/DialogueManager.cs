using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public bool dialogueShowing = false;
    Dialogue currentDialogue;
    int dialoguePosition = 0;

    public GameObject dialoguePanel, caret;
    public TextMeshProUGUI dialogueText;

    public float caretFlashInterval = 1f;
    float caretFlashTimer = 0;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) AdvanceDialogue();
        UpdateCaret();        
    }

    void UpdateCaret()
    {
        caretFlashTimer += Time.deltaTime;
        if(caretFlashTimer > caretFlashInterval)
        {
            caretFlashTimer = 0;
            caret.SetActive(!caret.activeSelf);
        }
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        if (dialogue.dialogues.Length == 0) return;

        currentDialogue = dialogue;
        dialogueShowing = true;
        dialoguePanel.SetActive(true);
        dialogueText.text = currentDialogue.dialogues[0];
        dialoguePosition = 0;
    }

    void AdvanceDialogue()
    {
        if(currentDialogue.dialogues.Length > dialoguePosition + 1)
        {
            dialoguePosition++;
            dialogueText.text = currentDialogue.dialogues[dialoguePosition];
        } else
        {
            HideDialogue();
        }
    }

    void HideDialogue()
    {
        dialogueShowing = false;
        dialoguePanel.SetActive(false);
    }
}
