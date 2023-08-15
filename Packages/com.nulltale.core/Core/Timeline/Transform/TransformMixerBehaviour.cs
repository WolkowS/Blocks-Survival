using UnityEngine;
using UnityEngine.Playables;


namespace CoreLib.Timeline
{
    public class TransformMixerBehaviour : PlayableBehaviour
    {
        private bool isFirstFrame = true;
        
        private Vector3    posBase;
        private Quaternion rotBase;
        private Vector3    scaleBase;
                
        // =======================================================================
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var target = playerData as Transform;

            if (target == null)
                return;
            
            if (isFirstFrame)
            {
                posBase   = target.position;
                rotBase   = target.rotation;
                scaleBase = target.localScale;
            }

            var inputCount = playable.GetInputCount();

            var posWeight = 0f;
            var scaleWeight = 0f;
            var rotWeight = 0f;

            var pos   = Vector3.zero;
            var scale = Vector3.zero;
            var rot   = new Quaternion(0f, 0f, 0f, 0f);

            for (var n = 0; n < inputCount; n++)
            {
                var input = (ScriptPlayable<TransformBehaviour>)playable.GetInput(n);
                var beh   = input.GetBehaviour();

                if (beh.endLocation == null)
                    continue;

                var inputWeight = playable.GetInputWeight(n);
                if (inputWeight == 0)
                    continue;

                var time  = (float)(input.GetTime() / beh.clip.duration).Clamp01();
                var curveTime = beh.EvaluateCurrentCurve(time);

                if (beh.position)
                {
                    posWeight += inputWeight;
                    pos     += Vector3.LerpUnclamped(beh.startLocation.position, beh.endLocation.position, curveTime) * inputWeight;
                }

                if (beh.scale)
                {
                    scaleWeight += inputWeight;
                    scale += Vector3.LerpUnclamped(beh.startLocation.localScale, beh.endLocation.localScale, curveTime) * inputWeight;
                }
                
                if (beh.rotation)
                {
                    rotWeight += inputWeight;

                    var goalRot = Quaternion.LerpUnclamped(beh.startLocation.rotation, beh.endLocation.rotation, curveTime);
                    goalRot = NormalizeQuaternion(goalRot);

                    if (Quaternion.Dot(rot, goalRot) < 0f)
                    {
                        goalRot = ScaleQuaternion(goalRot, -1f);
                    }

                    goalRot = ScaleQuaternion(goalRot, inputWeight);
                    rot = AddQuaternions(rot, goalRot);
                }
            }

            pos   += posBase * (1f - posWeight);
            scale += scaleBase * (1f - scaleWeight);
            rot   =  AddQuaternions(rot, ScaleQuaternion(rotBase, 1f - rotWeight));

            target.position   = pos;
            target.rotation   = rot;
            target.localScale = scale;

            isFirstFrame = false;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            isFirstFrame = true;
        }

        private static Quaternion AddQuaternions(Quaternion first, Quaternion second)
        {
            first.w += second.w;
            first.x += second.x;
            first.y += second.y;
            first.z += second.z;
            return first;
        }

        private static Quaternion ScaleQuaternion(Quaternion rotation, float multiplier)
        {
            rotation.w *= multiplier;
            rotation.x *= multiplier;
            rotation.y *= multiplier;
            rotation.z *= multiplier;
            return rotation;
        }

        private static float QuaternionMagnitude(Quaternion rotation)
        {
            return Mathf.Sqrt((Quaternion.Dot(rotation, rotation)));
        }

        private static Quaternion NormalizeQuaternion(Quaternion rotation)
        {
            var magnitude = QuaternionMagnitude(rotation);

            if (magnitude > 0f)
                return ScaleQuaternion(rotation, 1f / magnitude);

            Debug.LogWarning("Cannot normalize a quaternion with zero magnitude.");
            return Quaternion.identity;
        }
    }
}