using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class Adsr
    {
        [SerializeField] private float attack = .2f;
        [SerializeField] private float decay = .1f;
        [SerializeField] private float sustain = .5f;
        [SerializeField] private float release = .3f;
        [SerializeField] private float attackEase;
        [SerializeField] private float decayEase;
        [SerializeField] private float releaseEase;
        [SerializeField] private Optional<float> output;

        [Tooltip("If disabled, no matter how short the signal is, it will be played until at least the end of decay time. " +
            "\n\nIf enabled, the end of signal will \"interrupt\" the attack or decay and immediately skip to release.")]
        [SerializeField] private bool interrupt;

        private float time;
        private bool  lastPressed;
        private float lastOnValue;
        private bool  isComplete;

        public float Time => time;
        public bool  IsComplete => isComplete;

        // =======================================================================
        public Adsr() { }
        
        public Adsr(float attack, float decay, float sustain, float release, float attackEase, float decayEase, float releaseEase, bool interrupt)
        {
            this.attack      = attack;
            this.decay       = decay;
            this.sustain     = sustain;
            this.release     = release;
            this.attackEase  = attackEase;
            this.decayEase   = decayEase;
            this.releaseEase = releaseEase;
            this.interrupt   = interrupt;
        }
        
        public float EvaluateIn(float time)
        {
            isComplete = false;
                
            if (time < attack)
                return 1 - _ease(1 - time / attack, attackEase);

            if (time < attack + decay)
                return Mathf.Lerp(sustain, 1, _ease(1 - ((time - attack) / decay), decayEase));

            return sustain;
        }

        public float EvaluateOut(float time, float from = 0)
        {
            var _from = from == 0 ? sustain : from;

            if (time < 0)
                return _from;

            if (time < release)
                return _ease(1 - time / release, releaseEase) * _from;
            
            isComplete = true;
            return 0;
        }

        public float Update(bool pressed, float deltaTime)
        {
            // If interrupt is not set, this makes the value "sticky",
            // so it keeps being on until the end of decay
            if (lastPressed && interrupt == false && time < attack + decay)
                pressed = true;

            // Reset time on key change
            if (pressed != lastPressed)
                time = 0;

            time += deltaTime;

            var f = 0f;
            if (pressed)
            {
                f = EvaluateIn(time);
                lastOnValue = f;
            }
            else
            {
                f = EvaluateOut(time, lastOnValue);
            }

            lastPressed = pressed;

            return f;
        }
        public float Update(bool pressed, ref float time, float deltaTime)
        {
            // If interrupt is not set, this makes the value "sticky",
            // so it keeps being on until the end of decay
            if (lastPressed && interrupt == false && time < attack + decay)
                pressed = true;

            // Reset time on key change
            if (pressed != lastPressed)
                time = 0;

            time += deltaTime;

            var f = 0f;
            if (pressed)
            {
                f = EvaluateIn(time);
                lastOnValue = f;
            }
            else
            {
                f = EvaluateOut(time, lastOnValue);
            }

            lastPressed = pressed;
            
            return output.Enabled ? (f / sustain) * output.Value : f;
        }

        // =======================================================================
        private static float _ease(float p_x, float p_c)
        {
            if (p_c == 0)
            {
                return p_x;
            }
            else if (p_c < 0)
            {
                return 1.0f - Mathf.Pow(1.0f - p_x, -p_c + 1);
            }
            else
            {
                return Mathf.Pow(p_x, p_c + 1);
            }
        }
    }

}