using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public interface IClipContainer
    {
        TimelineClip Clip { set; }
    }
    /*public class Generic : Marker, INotification, INotificationOptionProvider
    {
        [Tag]
        public string m_Tag;
        public string m_Data;
        public Object m_ObjectLink;

        public PropertyName id => name;

        public NotificationFlags    m_Flags = NotificationFlags.TriggerInEditMode | NotificationFlags.Retroactive;

        public NotificationFlags flags => m_Flags;
    }*/
}