using UnityEngine;
using System.Collections.Generic;

public class NoteMapGenerator : MonoBehaviour
{
    public SongData targetSong; 
    public float totalDurationInSeconds; // How long is the song?
    public int patternType = 0; // 0: Simple Pulse, 1: Alternating, 2: Random Beat

    [ContextMenu("Generate Note Map")] // Right click on the component to run this!
    public void GenerateMap()
    {
        if (targetSong == null) return;

        targetSong.notes.Clear(); // Clear old notes
        float secPerBeat = 60f / targetSong.bpm;
        float totalBeats = totalDurationInSeconds / secPerBeat;

        for (int b = 0; b < totalBeats; b++)
        {
            // Logic: Create patterns every beat
            // You can change this logic to be more complex
            NoteInfo newNote = new NoteInfo();
            newNote.beat = b + 4; // Start at beat 4 to give player prep time
            
            // ARCHITECT LOGIC: Assign lanes based on patterns
            switch (patternType)
            {
                case 0: // Constant lane
                    newNote.laneIndex = 0; 
                    break;
                case 1: // Alternating 0, 1, 2, 3
                    newNote.laneIndex = b % 4; 
                    break;
                case 2: // Random lane but stays on beat
                    newNote.laneIndex = Random.Range(0, 4);
                    break;
            }

            targetSong.notes.Add(newNote); // Add note to the list
        }

        Debug.Log($"Generated {targetSong.notes.Count} notes for {targetSong.name}"); // Log the result
    }
}