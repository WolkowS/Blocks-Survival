using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using CoreLib.Fx;
using CoreLib.Sound;
using CoreLib.States;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Image = UnityEngine.UI.Image;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(FxTools), menuName = Core.k_CoreModuleMenu + nameof(FxTools))]
    public class FxTools : Core.Module<FxTools>
    {
        public static List<INoisehandle>                             s_NoiseHandles  = new List<INoisehandle>();
        public static Dictionary<VolumeProfile, PostProcessInstance> s_PostProcesses = new Dictionary<VolumeProfile, PostProcessInstance>();

        public Color             m_FadeColor = Color.black;
        public AnimationCurve    m_FadeIn = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
        public AnimationCurve    m_FadeOut = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        public Color             m_FlashColor = Color.white;
        public AnimationCurve    m_Flash = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(0.2f, 0.0f));

        [Tooltip("Main Camera must contain CinemachineImpulseSource Component")]
        public float             m_ImpulseForce;
        public NoiseSettings     m_ShakeNoise;
        
        public LeanTweenType     m_TextEase = LeanTweenType.easeInOutCirc;
        
        public Sprite m_Circle;
        public Sprite m_Square;
        
        public float _tickSlow = 0.1f;
        public float _tickTime = 0.04f;
        
        [Foldout("Audio")]
        public AudioClip    _coin;
        [Foldout("Audio")]
        public AudioClip    _gem;
        [Foldout("Audio")]
        public AudioClip    _kill;
        [Foldout("Audio")]
        public AudioClip    _hit;
        [Foldout("Audio")]
        public AudioClip    _up;
        [Foldout("Audio")]
        public AudioClip    _shoot;
        [Foldout("Audio")]
        public AudioClip    _fail;

        // =======================================================================
        public interface IScreenOverlay
        {
            Color   Color { get; set; }
            Sprite  Sprite { get; set; }
            Vector2 Scale { get; set; }
        }

        public class ScreenOverlayHandle : IScreenOverlay, IDisposable
        {
            protected Image         m_Image;
            protected RectTransform m_Transform;
            protected Canvas        m_Canvas;

            public Color   Color
            {
                get => m_Image.color;
                set
                {
                    if (m_Image.color != value)
                        m_Image.color = value;
                }
            }

            public Sprite  Sprite
            {
                get => m_Image.sprite;
                set
                {
                    if (m_Image.sprite != value)
                        m_Image.sprite = value;
                }
            }

            public Vector2 Scale
            {
                get => m_Transform.localScale.To2DXY();
                set
                {
                    if (m_Transform.localScale.To2DXY() != value)
                        m_Transform.localScale = value.To3DXY(1.0f);
                }
            }

            public Image  Image  => m_Image;
            public Canvas Canvas => m_Canvas;

            // =======================================================================
            public ScreenOverlayHandle(int sortingOrder = 10000)
            {
                var go = new GameObject("SO", typeof(Canvas));
                go.hideFlags = HideFlags.DontSave;

                // instantiate canvas & image
                m_Canvas = go.GetComponent<Canvas>();
                m_Canvas.renderMode   = RenderMode.ScreenSpaceCamera;
                m_Canvas.worldCamera  = Core.Camera;
                m_Canvas.sortingOrder = sortingOrder;
                // 
                m_Canvas.transform.SetParent(Core.Instance.transform);

                var imgGO = new GameObject("Image", typeof(CanvasRenderer), typeof(Image));
                imgGO.transform.SetParent(m_Canvas.transform);
                
                m_Image = imgGO.GetComponent<Image>();
                m_Image.raycastTarget = false;
                m_Image.maskable = false;

                m_Transform = m_Image.GetComponent<RectTransform>();
                m_Transform.localPosition = m_Transform.localPosition.WithZ(0f);
                m_Transform.anchorMax = new Vector2(1.0f, 1.0f);
                m_Transform.anchorMin = new Vector2(0.0f, 0.0f);

                m_Transform.offsetMin = new Vector2(0.0f, 0.0f);
                m_Transform.offsetMax = new Vector2(0.0f, 0.0f);

                go.transform.SetParent(Core.Instance.transform);
                go.SetActive(false);
            }

            public void Open()
            {
#if UNITY_EDITOR
                if (Application.isEditor)
                {
                    if (m_Canvas == null)
                        return;
                }
#endif
                m_Canvas.gameObject.SetActive(true);
            }

            public void Close()
            {
#if UNITY_EDITOR
                if (Application.isEditor)
                {
                    if (m_Canvas == null)
                        return;
                }
#endif
                m_Canvas.gameObject.SetActive(false);
            }

            public void Dispose()
            {
                if (m_Canvas != null && m_Canvas.gameObject != null)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                        DestroyImmediate(m_Canvas.gameObject);
                    else
                        Destroy(m_Canvas.gameObject);
#else
                    Destroy(m_Canvas.gameObject);
#endif
                }
            }
        }

        [ExecuteAlways]
        public class PostProcessInstance : MonoBehaviour
        {
            private Volume       m_Volume;
            private List<Handle> m_Handles = new List<Handle>();

            // =======================================================================
            public class Handle : IDisposable
            {
                public PostProcessInstance Owner;
                public float       Weight;

                // =======================================================================
                public void Dispose()
                {
                    Owner.Close(this);
                }
            }

            // =======================================================================
            public void Init(VolumeProfile postProcess)
            {
                m_Volume = gameObject.AddComponent<Volume>();
                m_Volume.isGlobal = true;
                m_Volume.profile  = postProcess;

                gameObject.hideFlags = HideFlags.DontSave;
            }

            public void Update()
            {
                m_Volume.weight = m_Handles.Max(n => n.Weight);
            }

            internal void Close(Handle handle)
            {
                if (m_Handles.Remove(handle) == false)
                    return;
                
                // destroy if no more handles
                if (m_Handles.IsEmpty() == false)
                    return;

                s_PostProcesses.Remove(m_Volume.profile);

#if UNITY_EDITOR
                if (gameObject == null)
                    return;
#endif
                
                if (Application.isPlaying)
                    gameObject.Destroy();
                else
                    gameObject.DestroyImmediate();
            }

            internal Handle Open()
            {
                var handle = new Handle() { Owner = this };
                m_Handles.Add(handle);

                return handle;
            }
        }

        public interface INoisehandle
        {
            float Amplitude { get; }
            float Frequency { get; }
            float Torque    { get; }
        }

        public class NoiseHandle : INoisehandle, IDisposable
        {
            public float             Amplitude   { get; set; }
            public float             Frequency   { get; set; }
            public float             Torque      { get; set; }

            public void Dispose()
            {
                s_NoiseHandles.Remove(this);
            }
        }

        // =======================================================================
        public override void Init()
        {
            s_NoiseHandles  = new List<INoisehandle>();
            s_PostProcesses = new Dictionary<VolumeProfile, PostProcessInstance>();
        }

        public static void CreateScreenOverlayEffect(Color color, AnimationCurve fade, Sprite sprite = null, Gs state = null, int sortingOreder = 10000, bool realTime = true)
        {
            var go = new GameObject("SOE");
            go.SetActive(false);

            var sof = go.AddComponent<ScreenOverlayEffect>();

            sof.m_OnComplete   = ScreenOverlayEffect.OnComplete.Destroy;
            sof.m_Fade         = fade;
            sof.m_Color        = color;
            sof.m_Sprite       = new Optional<Sprite>(sprite, true);
            sof.m_RealTime     = realTime;

            
            if (state != null)
            {
                state.Open();
                sof.gameObject.AddComponent<OnDestroyCallback>().Action = state.Close;
            }

            go.SetActive(true);
            sof.Canvas.sortingOrder = sortingOreder;
        }

        public static PostProcessInstance.Handle PostProcess(VolumeProfile postProcess)
        {
            if (postProcess == null)
                return null;

            if (s_PostProcesses.TryGetValue(postProcess, out var postProcessInstance) == false)
            {
                var go = new GameObject($"PP_{postProcess.name}");
                go.transform.SetParent(Core.Instance.transform);

                postProcessInstance = go.AddComponent<PostProcessInstance>();
                postProcessInstance.Init(postProcess);
                s_PostProcesses.Add(postProcess, postProcessInstance);
            }

            return postProcessInstance.Open();
        }

        public static NoiseHandle Noise()
        {
            var noiseHandle = new NoiseHandle();
            s_NoiseHandles.Add(noiseHandle);
            return noiseHandle;
        }
        
        public static void CoinSound() => _audio(Instance._coin);
        public static void GemSound() => _audio(Instance._gem);
        public static void KillSound() => _audio(Instance._kill);
        public static void HitSound() => _audio(Instance._hit);
        public static void UpSound() => _audio(Instance._up);
        public static void ShootSound() => _audio(Instance._shoot);
        public static void FailSound() => _audio(Instance._fail);
        
        public static void Points(float points, Vector3 pos, Color color = default)
        {
            if (points == 0)
                return;

            SpawnText($"{points:N1}", pos, color);
        }
        
        public static void Points(int points, Vector3 pos, Color color = default)
        {
            if (points == 0)
                return;

            SpawnText(points.ToString(), pos, color);
        }

        public static void SpawnText(string text, Vector3 pos, Color color)
        {
            if (color == default)
                color = Color.green;
            
            var go = new GameObject("Points");
            go.transform.position = pos;

            go.AddComponent<LifeTime>().TimeLeft = 0.7f;

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.alignment                               = TextAlignmentOptions.Center;
            tmp.fontSize                                = 4;
            tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 5);
            tmp.text                                    = text;
            tmp.color                                   = color;
            tmp.sortingOrder                            = 1000;

            LeanTween.move(go, pos + Vector3.up, 0.6f)
                     .setEase(Instance.m_TextEase);
        }
        
        public static void Circle(Vector3 pos, float radius, Color color = default, float duration = .7f)
        {
            if (color == default)
                color = Color.green;
            
            var go = new GameObject("Circle");
            go.transform.position = pos;
            go.transform.localScale = radius.ToVector3XY(1f);

            go.AddComponent<LifeTime>().TimeLeft = duration;

            var tmp = go.AddComponent<SpriteRenderer>();
            tmp.sprite                                  = Instance.m_Circle;
            tmp.color                                   = color;
            tmp.sortingOrder                            = 1000;
        }

        public static LTDescr Pop(GameObject go)
        {
            return global::LeanTween.scale(go, go.transform.localScale * 1.2f, 0.3f).setEase(LeanTweenType.punch);
        }

        public static void FadeIn()
        {
            CreateScreenOverlayEffect(Instance.m_FadeColor, Instance.m_FadeIn, null, null);
        }
        
        public static void FadeOut()
        {
            CreateScreenOverlayEffect(Instance.m_FadeColor, Instance.m_FadeOut, null, null);
        }

        public static void Flash()
        {
            CreateScreenOverlayEffect(Instance.m_FlashColor, Instance.m_Flash, null, null);
        }
        
        public static void Flash(Color color)
        {
            CreateScreenOverlayEffect(color, Instance.m_Flash, null, null);
        }

        public static void GenerateImpulse()
        {
            Core.Camera.GetComponent<CinemachineImpulseSource>().GenerateImpulse(Instance.m_ImpulseForce);
        }
        
        public static void TimeTick()
        {
            TimeControl.SlowDown(Instance._tickSlow, Instance._tickTime);
        }

        // =======================================================================
        private static void _audio(AudioClip audio)
        {
            if (audio == null)
                return;
            
            SoundManager.Instance.Sound.AudioSource.PlayOneShot(audio);
        }
    }

}