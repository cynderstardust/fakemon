using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public bool dialogueShowing = false;
    Dialogue currentDialogue;
    DialoguePage currentPage;
    int dialoguePosition = 0;
    int selectedOption;

    public GameObject dialoguePanel, caret;
    public TextMeshProUGUI dialogueText, option1Text, option2Text, option3Text, option1Carat, option2Carat, option3Carat;

    public float caretFlashInterval = 1f;
    float caretFlashTimer = 0;

    void Start()
    {
        instance = this;
        Debug.Log($"Dialogue text lengths: {dialogueText.text.Length}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ProcessAction();
        UpdateCaret();        
    }

    void UpdateCaret()
    {
        //Turn off the blinking 'next page' carat if we have options to choose from
        if(currentPage == null ||currentPage.choices.Length > 0)
        {
            caret.SetActive(false);
            return;
        }

        caretFlashTimer += Time.deltaTime;
        if(caretFlashTimer > caretFlashInterval)
        {
            caretFlashTimer = 0;
            caret.SetActive(!caret.activeSelf);
        }
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        if (dialogue.pages.Length == 0) return;

        currentDialogue = dialogue;
        dialogueShowing = true;
        dialoguePanel.SetActive(true);
        ShowDialoguePage(0);        
    }

    void ShowDialoguePage(int pageNumber)
    {
        dialoguePosition = pageNumber;
        currentPage = currentDialogue.pages[pageNumber];

        dialogueText.text = currentPage.text;

        if(currentPage.choices == null)
        {
            option3Text.gameObject.SetActive(false);
            option2Text.gameObject.SetActive(false);
            option1Text.gameObject.SetActive(false);
        } else
        {
            option3Text.gameObject.SetActive(currentPage.choices.Length > 2);
            option2Text.gameObject.SetActive(currentPage.choices.Length > 1);
            option1Text.gameObject.SetActive(currentPage.choices.Length > 0);
        }

        //TODO fix this first next time - the null check evaluating to false doesn't stop the || being evaluated
       
        
        SetSelectedOption(currentPage.choices.Length);
    }

    void SetSelectedOption(int option)
    {
        selectedOption = option;


    }

    void ProcessAction()
    {
        //If the page has options, process the currently selected option


        //Otherwise, process the page's option

        DialogueActionType selectedType = currentPage.action.type;

        switch (selectedType)
        {
            case DialogueActionType.GoToNextDialoguePage:
                //Make sure there is a next page, if not, close
                dialoguePosition++;
                if (currentDialogue.pages.Length <= dialoguePosition)
                {
                    HideDialogue();
                    break;
                }

                ShowDialoguePage(dialoguePosition);
                break;

            case DialogueActionType.CloseDialogue:
                HideDialogue();
                break;
        }
    }

    void HideDialogue()
    {
        dialogueShowing = false;
        dialoguePanel.SetActive(false);
    }
}
