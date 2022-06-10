# ElgatoWaveLink-cSharp
SDK For Elgato Wave Link Software, based off of the StreamDeck plugin for the wavelink

Working:
- Event handlers
- Get methods
- Set Commands
  - setMonitorMixOutput
  - switchMonitoring !! Only switchin to StreamMix works !!
  - setMicrophoneSettings
  - setOutputMixer

Left to do:
- Unit Test
- switchMonitorMix won't allow us to change to LocalMix, only to StreamMix
- setInputMix isn't implemented yet
- Add support for new filter feature
- Add client settings
