namespace ElgatoWaveSDK
{
    internal static class ElgatoConstants
    {
        internal class Events
        {
            // Microphone
            public const string MicrophoneConfigChanged = "microphoneConfigChanged";

            // Output
            public const string OutputSwitched = "outputSwitched";
            public const string SelectedOutputChanged = "selectedOutputChanged";
            public const string OutputMuteChanged = "outputMuteChanged";
            public const string OutputVolumeChanged = "outputVolumeChanged";

            // Input
            public const string InputsChanged = "inputsChanged";
            public const string InputMuteChanged = "inputMuteChanged";
            public const string InputVolumeChanged = "inputVolumeChanged";
            public const string InputNameChanged = "inputNameChanged";
            public const string InputEnabled = "inputEnabled";
            public const string InputDisabled = "inputDisabled";

            // Filter
            public const string FilterBypassStateChanged = "filterBypassStateChanged";
            public const string FilterAdded = "filterAdded";
            public const string FilterChanged = "filterChanged";
            public const string FilterRemoved = "filterRemoved";

            //Unused
            public const string OutputChanged = "outputChanged";
            public const string UpdatePI = "updatePI";
        }

        internal class Commands
        {
            // Request
            // Common
            public const string GetApplicationInfo = "getApplicationInfo";

            // Microphone
            public const string GetMicrophoneConfig = "getMicrophoneConfig";
            public const string SetMicrophoneConfig = "setMicrophoneConfig";

            // Output
            public const string GetSwitchState = "getSwitchState";
            public const string SwitchOutput = "switchOutput";
            public const string GetOutputConfig = "getOutputConfig";
            public const string SetOutputConfig = "setOutputConfig";
            public const string GetOutputs = "getOutputs";
            public const string SetSelectedOutput = "setSelectedOutput";

            // Input
            public const string GetInputConfigs = "getInputConfigs";
            public const string SetInputConfig = "setInputConfig";
            public const string SetFilterBypass = "setFilterBypass";
            public const string SetFilter = "setFilter";
        }

        internal class Properties
        {
            internal static class Microphone
            {
                public const string IsWaveLink = "isWaveLink";
                public const string IsWaveXLR = "isWaveXLR";
                public const string Gain = "gain";
                public const string OutputVolume = "outputVolume";
                public const string Balance = "balance";
                public const string LowCut = "lowCut";
                public const string LowCutType = "lowCutType";
                public const string ClipGuard = "clipGuard";
            }

            internal static class ApplicationInfo
            {
                public const string AppID = "appID";
                public const string AppName = "appName";
                public const string InterfaceRevision = "interfaceRevision";
            }

            internal static class Common
            {
                public const string Identifier = "identifier";
                public const string Name = "name";
                public const string Property = "property";
                public const string Value = "value";
                public const string IsAdjustVolume = "isAdjustVolume";
                public const string IsActive = "isActive";
                public const string MixerID = "mixerID";
                public const string FilterID = "filterID";
                public const string PluginID = "pluginID";
                public const string LocalMixer = "localMixer";
                public const string StreamMixer = "streamMixer";
            }

            internal static class Others
            {
                // Wave Link / Plugin
                public const string Volume = "Volume";
                public const string Mute = "Mute";
                public const string OutputLevel = "Output Level";
                public const string OutputMute = "Output Mute";

                public const string ForceLink = "forceLink";
            }
        }

        internal class Values
        {
            internal class ApplicationInfo
            {
                public const string AppID = "egwl";
                public const string AppName = "Elgato Wave Link";
                public const string InterfaceRevision = "3";
            }

            internal class Mixers
            {
                public const string MixerIDLocal = "com.elgato.mix.local";
                public const string MixerIDStream = "com.elgato.mix.stream";
                public const string MixerIDAll = "com.elgato.mix.all";
            }
        }

        //public enum ActionType
        //{
        //    Mute = 0,
        //    SetVolume = 1,
        //    AdjustVolume = 2,
        //    SetEffect = 3,
        //    SetEffectChain = 4,
        //    SetDeviceSettings = 5,
        //    SetOutput = 6,
        //    ToggleOutput = 7,
        //    SwitchOutput = 8
        //}
    }
}