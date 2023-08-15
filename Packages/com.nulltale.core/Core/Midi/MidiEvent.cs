using System;

namespace CoreLib.Midi
{
    // MIDI event raw data struct
    [Serializable]
    public struct MidiEvent
    {
        public uint time;
        public byte status;
        public byte data1;
        public byte data2;

        public bool  IsCC      => (status & 0xb0) == 0xb0;
        public bool  IsNote    => (status & 0xe0) == 0x80;
        public bool  IsNoteOn  => (status & 0xf0) == 0x90;
        public bool  IsNoteOff => (status & 0xf0) == 0x80;
        public int   Id        => data1;
        public float Velocity  => data2 / 127f;

        public override string ToString()
        {
            return string.Format("[{0}: {1:X}, {2}, {3}]", time, status, data1, data2);
        }
    }
}
