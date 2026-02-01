using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntroStep 
{
    public CanvasGroup maskCanvasGroup; 
    [TextArea(3, 10)] public string dialogueText; 
}