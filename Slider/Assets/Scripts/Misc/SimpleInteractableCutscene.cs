using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SimpleInteractableCutscene : MonoBehaviour, IInteractable
{
    [Tooltip("Optional Flag to set when cutscene starts")]
    [SerializeField] private string cutsceneStartedFlag;
    [Tooltip("Optional Flag to set when cutscene finishes")]
    [SerializeField] private string cutsceneFinishedFlag;
    [Tooltip("Array of all the characters in the cutscene")]
    [SerializeField] protected NPC[] cutsceneCharacters;
    [SerializeField] protected bool stillResolveCutsceneIfNotFinished = true;

    private const float DEFAULT_TIME_BETWEEN_DIALOGUE_LINES = 1.3f;

    private NPC currentlyTalkingCharacter;
    private bool currentlyTyping = false;
    private bool currentlyWaitingAfterTyped = false;
    private bool skipWaitingAfterTyped = false;
    private bool currentDialogueAllowedToSkip = false;
    private bool playerInTrigger = false;

    public bool cutsceneStarted { get; private set; } = false;
    public bool cutsceneFinished { get; private set; } = false;

    protected virtual void Start()
    {
        if (!string.IsNullOrEmpty(cutsceneFinishedFlag) && SaveSystem.Current.GetBool(cutsceneFinishedFlag))
        {
            OnCutSceneFinish();
        }
        else
        {
            if (ShouldCutsceneBeSkipped())
            {
                OnCutSceneFinish();
                return;
            }
        }
    }

    private void OnDisable()
    {
        if (stillResolveCutsceneIfNotFinished && cutsceneStarted && !cutsceneFinished)
        {
            Debug.Log($"Cutscene didn't finish, trying to resolve...");
            OnCutsceneNotFinished();
        }
    }

    protected virtual void OnCutsceneNotFinished()
    {
        if (!string.IsNullOrEmpty(cutsceneFinishedFlag))
        {
            SaveSystem.Current.SetBool(cutsceneFinishedFlag, true);
        }
    }

    protected virtual bool ShouldCutsceneBeSkipped()
    {
        // For example, if the cutscene is on Slider 1 and you get Slider 2, skip it.
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        if (ShouldCutsceneBeSkipped())
        {
            OnCutSceneFinish();
            return;
        }

        playerInTrigger = true;

        if (!cutsceneFinished)
        {
            AllowPlayerInteraction(true);
        }

        if (!cutsceneStarted)
        {
            StartCutScene();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        playerInTrigger = false;

        if (!cutsceneFinished)
        {
            AllowPlayerInteraction(false);
        }
    }

    private void AllowPlayerInteraction(bool allowed)
    {
        if (allowed)
        {
            Player.GetPlayerAction().AddInteractable(this);
        }
        else
        {
            Player.GetPlayerAction().RemoveInteractable(this);
        }
    }

    public bool Interact()
    {
        TrySkipCurrentDialogue();

        return true;
    }

    private void TrySkipCurrentDialogue()
    {
        if (currentlyTalkingCharacter != null && currentDialogueAllowedToSkip)
        {
            if (currentlyTyping)
            {
                currentlyTalkingCharacter.SkipText();
            }
            else if (currentlyWaitingAfterTyped)
            {
                skipWaitingAfterTyped = true;
            }
        }
    }

    private void StartCutScene()
    {
        StartCoroutine(StartCutSceneCoroutine());
    }

    private IEnumerator StartCutSceneCoroutine()
    {
        EnableAllNormalCharacterDialogueTriggers(false);

        cutsceneStarted = true;

        if (cutsceneStartedFlag != null)
        {
            SaveSystem.Current.SetBool(cutsceneStartedFlag, true);
        }

        foreach (NPC character in cutsceneCharacters)
        {
            if (character.IsDialogueBoxActive())
            {
                character.DeactivateDialogueBox();
            }
        }

        yield return CutScene();

        if (playerInTrigger)
        {
            AllowPlayerInteraction(false);
        }

        cutsceneFinished = true;

        if (cutsceneFinishedFlag != null)
        {
            SaveSystem.Current.SetBool(cutsceneFinishedFlag, true);
        }
        //we want a bit of a gap after cutscene is done before you can start talking to them like normal, otherwise it's weird
        yield return new WaitForSeconds(1.5f);

        EnableAllNormalCharacterDialogueTriggers(true);
    }

    private void EnableAllNormalCharacterDialogueTriggers(bool enable)
    {
        foreach (NPC character in cutsceneCharacters)
        {
            EnableNormalCharacterDialogueTrigger(character, enable);
        }
    }

    private void EnableNormalCharacterDialogueTrigger(NPC character, bool enable)
    {
        OnTriggerEnter triggerEnterScript = character.GetComponent<OnTriggerEnter>();
        if (triggerEnterScript != null)
        {
            triggerEnterScript.SetOnPlayerEnterActive(enable);
        }
    }

    protected virtual IEnumerator CutScene()
    {
        yield return null;
        OnCutSceneFinish();
    }

    // Enable/disable gameobjects, colliders, etc.
    // This way we can call this when you load in from a save.
    protected virtual void OnCutSceneFinish()
    {

    }
   
    protected IEnumerator SayNextDialogue(NPC character, bool skippable = true, float timeWaitAfterFinishedTyping = DEFAULT_TIME_BETWEEN_DIALOGUE_LINES)
    {
        character.TypeCurrentDialogue();
        currentlyTalkingCharacter = character;
        currentlyTyping = true;
        currentDialogueAllowedToSkip = skippable;

        yield return new WaitWhile(() => character.IsTypingDialogue());

        currentlyTyping = false;
        currentlyWaitingAfterTyped = true;

        float startTime = Time.time;
        float currentTime = Time.time;
        while ((currentTime - startTime < timeWaitAfterFinishedTyping) && !skipWaitingAfterTyped)
        {
            yield return null;
            currentTime = Time.time;
        }

        currentlyWaitingAfterTyped = false;
        currentDialogueAllowedToSkip = false;
        //yield return new WaitForSeconds(timeWaitAfterFinishedTyping);

        character.AdvanceDialogueChain(); //moves dialogue to next part in chain, considering this line to be said
        character.DeactivateDialogueBox(); //makes the current dialogue disappear from screen

        currentlyTalkingCharacter = null;
        skipWaitingAfterTyped = false;
    }
}
