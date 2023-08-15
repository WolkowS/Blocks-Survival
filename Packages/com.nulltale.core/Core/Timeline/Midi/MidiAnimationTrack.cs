using CoreLib.Midi;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    // Track asset class that contains a MIDI animation track (clips and its assigned controls)
    [TrackColor(0.4f, 0.4f, 0.4f)]
    [TrackClipType(typeof(MidiAnimationAsset))]
    [TrackBindingType(typeof(MidiOutput))]
    public sealed class MidiAnimationTrack : TrackAsset
    {
    }
}