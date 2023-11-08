using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

        CheckOptionSelectionKeys();
    }

    void CheckOptionSelectionKeys()
    {
        if (currentPage == null) return;

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (selectedOption < currentPage.choices.Length - 1)
            {
                selectedOption++;
                return;
            } else if (selectedOption == currentPage.choices.Length - 1)
            {
                selectedOption = 0;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (selectedOption > 0)
            {
                selectedOption--;
                return;
            }
            else if (selectedOption == 0)
            {
                selectedOption = currentPage.choices.Length - 1;
                return;
            }
        }
    }

    
    
    void UpdateCaret()
    {
        //Turn off the blinking 'next page' carat if we have options to choose from
        if(currentPage == null || currentPage.choices.Length > 0)
        {
            caret.SetActive(false);

            switch (selectedOption)
            {
                case 0:
                    option1Carat.gameObject.SetActive(true);
                    option2Carat.gameObject.SetActive(false);
                    option3Carat.gameObject.SetActive(false);
                    break;

                case 1:
                    option1Carat.gameObject.SetActive(false);
                    option2Carat.gameObject.SetActive(true);
                    option3Carat.gameObject.SetActive(false);
                    break;

                case 2:
                    option1Carat.gameObject.SetActive(false);
                    option2Carat.gameObject.SetActive(false);
                    option3Carat.gameObject.SetActive(true);
                    break;
            }

            return; //Don't let the rest of the flashing caret code run
        }

        caretFlashTimer += Time.deltaTime;
        if(caretFlashTimer > caretFlashInterval)
        {
            caretFlashTimer = 0;
            caret.SetActive(!caret.activeSelf);
        }
    }

    // Called by the object collision detection function
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

        Debug.Log($"Current page has {currentPage.choices.Length} choices");

       //Turn all the options off first, then only turn them on if needed
        option3Text.gameObject.SetActive(false);
        option2Text.gameObject.SetActive(false);
        option1Text.gameObject.SetActive(false);

        option1Carat.gameObject.SetActive(false);
        option2Carat.gameObject.SetActive(false);
        option3Carat.gameObject.SetActive(false);


        if (currentPage.choices.Length > 2)
        {
            option3Text.gameObject.SetActive(true);
            option3Text.text = currentPage.choices[2].text;
        }

        if (currentPage.choices.Length > 1)
        {
            option2Text.gameObject.SetActive(true);
            option2Text.text = currentPage.choices[1].text;
        }

        if (currentPage.choices.Length  > 0)
        {
            option1Text.gameObject.SetActive(true);
            option1Text.text = currentPage.choices[0].text;
        }

        //This will also set the choice to -1 if there are no pages
        SetSelectedOption(currentPage.choices.Length - 1);
    }

    void SetSelectedOption(int option)
    {
        selectedOption = option;
    }

    void ProcessAction()
    {
        DialogueAction selectedAction;
        //If the page has options, process the currently selected option
        if(selectedOption >= 0)
        {
            selectedAction = currentPage.choices[selectedOption].action;
        } else
        {
            //Otherwise, process the page's option
            selectedAction = currentPage.action;
        }

        switch (selectedAction.type)
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

            case DialogueActionType.ChangeScene:
                SceneManager.LoadScene(selectedAction.goToScene);
                break;
        }
    }

    void HideDialogue()
    {
        dialogueShowing = false;
        dialoguePanel.SetActive(false);
    }
}
