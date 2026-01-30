using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour {
    public Animator animator;
    public bool isPlayer;

    void OnEnable() {
        RhythmEvents.OnNoteHit += OnHit;
        RhythmEvents.OnNoteMiss += OnMiss;
    }

    void OnDisable() {
        RhythmEvents.OnNoteHit -= OnHit;
        RhythmEvents.OnNoteMiss -= OnMiss;
    }

    private void OnHit() {
        if (isPlayer) animator.SetTrigger("Attack"); // Player ตี
        else animator.SetTrigger("Hurt");             // Enemy โดนตี
    }

    private void OnMiss() {
        if (isPlayer) animator.SetTrigger("Hurt");   // Player โดนตี
        else animator.SetTrigger("Attack");           // Enemy ตีสวน
    }
}