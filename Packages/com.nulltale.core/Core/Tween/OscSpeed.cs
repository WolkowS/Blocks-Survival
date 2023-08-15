namespace CoreLib.Tween
{
    public class OscSpeed : TweenOscillator
    {
        public float m_Value;
        public float m_Speed = 1;
        
        public override float Value => m_Value += m_Speed * DeltaTime;

        public float Speed
        {
            get => m_Speed;
            set => m_Speed = value;
        }

        public void SetValue(float value)
        {
            m_Value = value;
        }
    }
}