using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [CustomTimelineEditor(typeof(StageAsset)), UsedImplicitly]
    public class StageAssetEditor : ClipEditor
    {
        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            if (clip == null)
                return;
            
            var rect = region.position;
            if (rect.width <= 0)
                return;

            var stageClip = clip.asset as StageAsset;
            
            if (stageClip == null || stageClip._track == null)
                return;
            
            if (stageClip._track._showUI == false)
                return;
            
            Handles.color = new Color32(215, 217, 222, 255);
            switch (stageClip._mode)
            {
                case StageAsset.Mode.Relative:
                {
                    var time = stageClip._offset;
                    for (var n = 0; n < stageClip._blocks.Length; n++)
                    {
                        var block = stageClip._blocks[n];

                        var realWidth = (rect.width / (float)((region.endTime - region.startTime) / clip.duration));
                        var timePos   = rect.x + time / clip.duration.ToFloat() * realWidth;
                        if (region.startTime > 0d)
                            timePos -= (float)(region.startTime / clip.duration) * realWidth;

                        GUI.Label(new Rect(new Vector2(timePos, rect.yMin), new Vector2(330f, rect.height)), n.ToString());
                        if (time != 0)
                            Handles.DrawAAPolyLine(3.3f, new Vector3(timePos, rect.yMin, 0f), new Vector3(timePos, rect.yMax, 0f));
                        time += block;
                    }
                } break;
                case StageAsset.Mode.Absolute:
                {
                    for (var n = 0; n < stageClip._blocks.Length; n++)
                    {
                        var realWidth = (rect.width / (float)((region.endTime - region.startTime) / clip.duration));
                        var timePos   = rect.x + stageClip._blocks[n] / clip.duration.ToFloat() * realWidth;
                        if (region.startTime > 0d)
                            timePos -= (float)(region.startTime / clip.duration) * realWidth;

                        GUI.Label(new Rect(new Vector2(timePos, rect.yMin), new Vector2(330f, rect.height)), n.ToString());
                        if (stageClip._blocks[n] != 0)
                            Handles.DrawAAPolyLine(3.3f, new Vector3(timePos, rect.yMin, 0f), new Vector3(timePos, rect.yMax, 0f));
                    }
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}