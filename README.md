# ElgatoWaveLink-cSharp
[![Nuget](https://img.shields.io/nuget/v/elgatowavesdk)](https://www.nuget.org/packages/ElgatoWaveSDK/ "![Nuget](https://img.shields.io/nuget/v/elgatowavesdk)")[ ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/elgatowavesdk)](https://www.nuget.org/packages/ElgatoWaveSDK/ " ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/elgatowavesdk)")

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
