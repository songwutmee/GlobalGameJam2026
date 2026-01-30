using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private int perfectComboCount = 0;
    private int normalComboCount = 0;
    private bool isNoteMissed = false;
    private void OnEnable()
    {
        NoteObject.OnNoteHit += HandleNoteHit;
        NoteObject.OnNoteHitPerfect += HandleNoteHitPerfect;
        NoteObject.OnNoteMiss += HandleNoteMiss;
    }

    private void OnDisable()
    {
        NoteObject.OnNoteHit -= HandleNoteHit;
        NoteObject.OnNoteHitPerfect -= HandleNoteHitPerfect;
        NoteObject.OnNoteMiss -= HandleNoteMiss;
    }

    private void HandleNoteHit(int lane)
    {
        if(isNoteMissed)
        {
            isNoteMissed = false;
            return;
        }
        normalComboCount++;
        perfectComboCount = 0;
    }

    private void HandleNoteHitPerfect(int lane)
    {
        if(isNoteMissed)
        {
            isNoteMissed = false;
            return;
        }
        perfectComboCount++;
        normalComboCount = 0;
    }

    private void HandleNoteMiss(int lane)
    {
        perfectComboCount = 0;
        normalComboCount = 0;
        isNoteMissed = true;
    }
}
