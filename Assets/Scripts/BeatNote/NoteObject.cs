using System;
using UnityEngine;

public class NoteObject : Notebase
{
    protected override void CheckHit()
    {
        float offset = Conductor.Instance.songPositionSeconds - hitTime;
        float absOffset = Mathf.Abs(offset);

        if (absOffset < 0.25f)
        {
            isHandled = true;
            if (absOffset < 0.12f)
                NoteEvents.TriggerPerfectHit(laneIndex); 
            else if (offset < 0) 
            {
                NoteEvents.TriggerEarlyHit(laneIndex);
            }
            else 
            {
                NoteEvents.TriggerNormalHit(laneIndex); 
            }

            Destroy(gameObject);
        }
    }
}
