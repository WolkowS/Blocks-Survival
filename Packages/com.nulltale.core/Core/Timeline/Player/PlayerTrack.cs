using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.26f, 0.6f, 0.94f)]
    [TrackClipType(typeof(PlayerAsset))]
    [TrackBindingType(typeof(TimelinePlayer))]
    public class PlayerTrack : TrackAsset
    {
    }
}