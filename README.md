# ElgatoWaveLink-cSharp
[ ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/elgatowavesdk)](https://www.nuget.org/packages/ElgatoWaveSDK/ " ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/elgatowavesdk)")

SDK For Elgato Wave Link Software, based on the StreamDeck plugin for the WaveLink

Working:
- Unit Tests
- Event handlers
- Get methods
- Set Commands
  - setMonitorMixOutput
  - switchMonitoring
  - setMicrophoneSettings
  - setOutputMixer
  - switchMonitorMix
  - setInputMix 
    - Necessary to pass in which mix you are setting (local vs stream), Looking to see how to change both at the same time as the JS library does have an "all" option
    - Setting filters is possible, but not tested as I'm not sure how to get a list of available filters

Left To Do:
- Support more than just DotNetCore 6