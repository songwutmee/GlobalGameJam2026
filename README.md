<div align="center">

# VeilTempo

<a href="https://www.youtube.com/watch?v=FWYNIrOunEM">
  <img src="https://img.youtube.com/vi/FWYNIrOunEM/maxresdefault.jpg" width="100%" alt="VeilTempo Gameplay">
</a>

<a href="https://www.youtube.com/watch?v=FWYNIrOunEM">
  <img src="https://img.shields.io/badge/▶%20CLICK%20TO%20WATCH%20DEMO-FF0000?style=for-the-badge&logo=youtube&logoColor=white" />
</a>

</div>

###  Game Concept
VeilTempo is a fast-paced rhythm game developed in 48 hours for **Global Game Jam 2026** at Game Dev Hub. VeilTempo is rhythm game where got stole identity by the mask.As the protagonist,they must regain themselves by rhythm and beat.

### Technical Work
As the  Programmer, I focused on building a solid foundation that could handle high-speed rhythm mechanics and make it easy for the team to create content:

- **Procedural Note Generation:** I built a system to read MIDI files and convert them into JSON data. This allowed us to generate note charts automatically, saving a massive amount of time that would have been spent manually placing notes.
- **Precision Audio Sync:** Fixed the common issue of audio-visual desync by using **AudioSettings.dspTime** for the game's Conductor. This ensured that note timing remained frame-perfect regardless of framerate fluctuations.
- **Modular Input System:** Used Unity's New Input System and C# Events to keep the gameplay logic separate from the UI and VFX. This made it much easier to debug and allowed the artists to add polish without breaking anything.
- **Scoring & Hit Detection:** Developed the logic for real-time accuracy calculation (Perfect, Great, Miss) and combo tracking, optimized to handle dense note patterns without performance drops.

### Team & Event
This project was a collaborative effort by a team of 7 passionate creators during a 48-hour jam session at Game Dev hub:
- **Programmer (Me):** Responsible for the technical architecture, MIDI-to-JSON pipeline, and core rhythm systems.
- **Artists & Designers:** Handled the 3D assets, environmental storytelling, and mask designs. We worked closely onsite to make sure the visual feedback matched the music perfectly.

## Play the Game
[Play VeilTempo on Itch.io](https://songwutlyhnn.itch.io/veiltempo)
