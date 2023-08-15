using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CoreLib.Commands;
using CoreLib.ExposedValues;
using CoreLib.SceneManagement;
using CoreLib.Scripting;
using CoreLib.Sound;
using CsvHelper;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;
using CompressionLevel = System.IO.Compression.CompressionLevel;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


namespace CoreLib
{
    public static class Extensions
    {
        private static Dictionary<(Type, Type), MethodInfo> s_DynamicCastCache = new Dictionary<(Type, Type), MethodInfo>();
        private static Collider2D[]                         s_Collider2DBuffer = new Collider2D[64];
        
        public static Vector2Int Infinity => new Vector2Int(int.MaxValue, int.MaxValue);
        public const float PI2 = Mathf.PI + Mathf.PI;
        
        // =======================================================================
        private class CircularEnumarator<T> : IEnumerator<T>
        {
            private readonly IEnumerator _wrapedEnumerator;

            public CircularEnumarator(IEnumerator wrapedEnumerator)
            {
                _wrapedEnumerator = wrapedEnumerator;
            }

            public object Current => _wrapedEnumerator.Current;

            T IEnumerator<T>.Current =>  (T)Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (!_wrapedEnumerator.MoveNext())
                {
                    _wrapedEnumerator.Reset();
                    return _wrapedEnumerator.MoveNext();
                }
                return true;
            }

            public void Reset()
            {
                _wrapedEnumerator.Reset();
            }
        }

	    public class UnityWebRequestAwaiter : INotifyCompletion
        {
            private UnityWebRequestAsyncOperation asyncOp;
            private Action                        continuation;

            public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation async)
            {
                asyncOp           =  async;
                asyncOp.completed += OnRequestCompleted;
            }

            public bool IsCompleted => asyncOp.isDone;

            public void GetResult() { }

            public void OnCompleted(Action cont)
            {
                continuation = cont;
            }

            private void OnRequestCompleted(AsyncOperation obj)
            {
                continuation();
            }
        }
        
        public class CommandAwaiter : INotifyCompletion
        {
            private Cmd.Handle _handle;

            // calls on task to check is process exists
            public bool IsCompleted => false;
        
            // =======================================================================
            public CommandAwaiter(Cmd.Handle handle)
            {
                _handle = handle;
            }

            public void GetResult() { }

            public void OnCompleted(Action continuation)
            {
                _handle._continuation = continuation;
            }
        }
    
        // =======================================================================
        public static CommandAwaiter GetAwaiter(this Cmd.Handle handle)
        {
            return new CommandAwaiter(handle);
        }
        
        // =======================================================================
        #region Coroutines

        // =======================================================================
        public static Coroutine Delayed(this MonoBehaviour obj, Action action, float timeDelay, bool realTime = false)
        {
            return obj.StartCoroutine(DelayRun(timeDelay, action, realTime));

            static IEnumerator DelayRun(float delay, Action action, bool realTime) 
            {
                if (realTime)
                    yield return new WaitForSecondsRealtime(delay);
                else
                    yield return new WaitForSeconds(delay);

                action();
            }
        }

        public static Coroutine Delayed(this MonoBehaviour obj, Action action, int frameDelay = 1)
        {
            return obj.StartCoroutine(_run(frameDelay, action));

            // -----------------------------------------------------------------------            
            static IEnumerator _run(int frameCount, Action action) 
            {
                while (frameCount-- > 0)
                    yield return null;

                action();
            }
        }

        public static Coroutine FixedUpdate(this MonoBehaviour obj, Action action)
        {
            return obj.StartCoroutine(_run(action));

            // -----------------------------------------------------------------------
            static IEnumerator _run(Action action) 
            {
                yield return Core.k_WaitForFixedUpdate;

                action();
            }
        }
        
        public static Coroutine FixedUpdate(this MonoBehaviour obj, Action action, int count)
        {
            return obj.StartCoroutine(_run(count, action));

            // -----------------------------------------------------------------------
            static IEnumerator _run(int count, Action action) 
            {
                for (var n = 0; n < count; n++)
                    yield return new WaitForFixedUpdate();

                action();
            }
        }

        public static Coroutine LateUpdate(this MonoBehaviour obj, Action action)
        {
            return obj.StartCoroutine(_run(action));
            
            static IEnumerator _run(Action action) 
            {
                yield return Core.k_WaitForEndOfFrame;

                action();
            }
        }

        public static Coroutine Repeat(this MonoBehaviour obj, Action action, int repeat, float repeatInterval) 
        {
            return obj.StartCoroutine(_run(repeat, action, repeatInterval));

            static IEnumerator _run(int repeat, Action action, float repeatInterval) 
            {
                if (repeat <= 0)	yield break;
            
                var interval = new WaitForSeconds(repeatInterval);

                // repeat with interval
                do
                {
                    action();
                    yield return interval;
                }   
                while (repeat-- >= 0);
            }
        }

        public static Coroutine RepeatForever(this MonoBehaviour obj, Action action) 
        {
            return obj.StartCoroutine(_run(action));

            static IEnumerator _run(Action action) 
            {
                while (true)
                {
                    action();
                    yield return null;
                }
            }
        }
        
        public static Coroutine RepeatForever(this MonoBehaviour obj, Action action, float interval, bool realTime = false) 
        {
            return obj.StartCoroutine(_run(action, realTime ? new WaitForSecondsRealtime(interval) : new WaitForSeconds(interval)));

            // -----------------------------------------------------------------------
            static IEnumerator _run(Action action, object interval) 
            {
                yield return interval;

                while (true)
                {
                    action.Invoke();
                    yield return interval;
                }
            }
        }
        
        public static Coroutine While(this MonoBehaviour obj, Func<bool> condition, Action action)
        {
            return obj.StartCoroutine(WhileRun(condition, action));

            static IEnumerator WhileRun(Func<bool> condition, Action action) 
            {
                // while run
                while (condition())
                {
                    action.Invoke();
                    yield return null;
                }
            }
        }

        #endregion

        #region External

        public static void JsSyncFiles()
        {
            if (Application.isEditor == false)
                SyncFiles();
        }

#if UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern void SyncFiles();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern void WindowAlert(string message);
        
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern string WebURI();
#else
        public static void SyncFiles(){ }

        public static void WindowAlert(string message){ }

        public static string WebURI(){ return String.Empty; }
#endif

        #endregion

        #region Array2D

        public static T[,] Take<T>(this T[,] array, in RectInt square)
        {
            return Take(array, square.xMin, square.yMin, square.xMax, square.yMax);
        }

        public static T[,] Take<T>(this T[,] array, Vector2Int at, Vector2Int to)
        {
            return Take(array, at.x, to.x, at.y, to.y);
        }

        public static T[,] Take<T>(this T[,] array, int xMin, int yMin, int xMax, int yMax)
        {
            var takeWidth = xMax - xMin;
            var takeHeight = yMax - yMin;

            var result = new T[takeWidth, takeHeight];

            for (var x = 0; x < takeWidth; x++)
            for (var y = 0; y < takeHeight; y++)
                result[x, y] =  array[xMin + x, yMin + y];

            return result;
        }

        public static T GetValue<T>(this T[,] array, in Vector2Int index)
        {
            return array[index.x, index.y];
        }

        public static T GetValueSafe<T>(this T[,] array, in Vector2Int index)
        {
            return array.GetValueSafe(index.x, index.y);
        }
        
        public static T GetValueSafe<T>(this T[,] array, int x, int y)
        {
            return InBounds(array, x, y) ? array[x, y] : default;
        }

        public static void SetValue<T>(this T[,] array, T value, in Vector2Int index)
        {
            array[index.x, index.y] = value;
        }

        public static void SetValueSafe<T>(this T[,] array, in Vector2Int index, T value)
        {
            SetValueSafe(array, index.x, index.y, value);
        }

        public static void SetValueSafe<T>(this T[,] array, int x, int y, T value)
        {
            if (InBounds(array, x, y))
                array[x, y] = value;
        }

        public static bool InBounds<T>(this T[,] array, in Vector2Int index)
        {
            return InBounds(array, index.x, index.y);
        }

        public static bool InBounds<T>(this T[,] array, int x, int y)
        {
            return x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1);
        }

        public static bool TrySetValue<T>(this T[,] array, in Vector2Int index, T value)
        {
            return TrySetValue(array, index.x, index.y, value);
        }

        public static bool TrySetValue<T>(this T[,] array, int x, int y, T value)
        {
            if (InBounds(array, x, y))
            {
                array[x, y] = value;
                return true;
            }
            
            return false;
        }

        public static bool TryGetValue<T>(this T[,] array, in Vector2Int index, out T value)
        {
            return TryGetValue(array, index.x, index.y, out value);
        }

        public static bool TryGetValue<T>(this T[,] array, int x, int y, out T value)
        {
            if (InBounds(array, x, y))
            {
                value = array[x, y];
                return true;
            }

            value = default;
            return false;
        }
        
        public static int GetVolume<T>(this T[,] array)
        {
            return array.GetLength(0) * array.GetLength(1);
        }
        
        public static T Random<T>(this T[,] array)
        {
            var w = array.GetLength(0);
            var h = array.GetLength(1);
            return array[UnityEngine.Random.Range(0, w - 1), UnityEngine.Random.Range(0, h - 1)];
        }

        public static IEnumerable<T> ToEnumerable<T>(this T[,] array)
        {
            for (var y = 0; y < array.GetLength(1); y++)
            for (var x = 0; x < array.GetLength(0); x++)
                yield return array[x, y];
        }
        public static IEnumerable<T> ToEnumerable<T>(this T[,] array, int xMin, int yMin, int xMax, int yMax)
        {
            for (var y = yMin; y <= yMax; y++)
            for (var x = xMin; x <= xMax; x++)
                yield return array[x, y];
        }
        public static IEnumerable<(int x, int y, T value)> Enumerate<T>(this T[,] array)
        {
            for (var y = 0; y < array.GetLength(1); y++)
            for (var x = 0; x < array.GetLength(0); x++)
                yield return (x, y, array[x, y]);
        }

        public static List<T> ToList<T>(this T[,] array)
        {
            var result = new List<T>(array.GetLength(0) * array.GetLength(1));

            foreach (var element in ToEnumerable(array))
                result.Add(element);

            return result;
        }

        public static T[] ToArray<T>(this T[,] array)
        {
            var width  = array.GetLength(0);
            var height = array.GetLength(1);
            
            var result = new T[width * height];

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                result[y * width + x] = array[x, y];

            return result;
        }
        
        public static T[,] Initialize<T>(this T[,] array, Action<int, int, T[,]> action)
        {
            for (var x = 0; x < array.GetLength(0); x++)
            for (var y = 0; y < array.GetLength(1); y++)
                action(x, y, array);

            return array;
        }

        public static T[,] Initialize<T>(this T[,] array, Func<int, int, T> action)
        {
            for (var x = 0; x < array.GetLength(0); x++)
            for (var y = 0; y < array.GetLength(1); y++)
                array[x, y] = action(x, y);

            return array;
        }

        public static T[,] Clear<T>(this T[,] array)
        {
            Array.Clear(array, 0, array.GetLength(0) * array.GetLength(1));
            return array;
        }

        #endregion

        #region Zip

        public static byte[] Zip(this string str, CompressionLevel compression = CompressionLevel.Optimal) 
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) 
            {
                using (var gs = new GZipStream(mso, compression)) 
                    msi.CopyTo(gs);

                return mso.ToArray();
            }
        }

        public static string UnzipString(this byte[] data)
        {
            return Encoding.UTF8.GetString(data.Unzip());
        }
        
        public static byte[] Zip(this byte[] data) 
        {
            using (var msi = new MemoryStream(data))
            using (var mso = new MemoryStream()) 
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress)) 
                    msi.CopyTo(gs);

                return mso.ToArray();
            }
        }

        public static byte[] Unzip(this byte[] bytes) 
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    gs.CopyTo(mso);

                return mso.ToArray();
            }
        }

        #endregion

        #region Rect

        public static RectInt ToRectInt(this Rect rect)
        {
            return new RectInt((int)rect.xMin, (int)rect.yMin, Mathf.CeilToInt(rect.width), Mathf.CeilToInt(rect.height));
        }
        
        public static Rect ToRect(this RectInt rect)
        {
            return new Rect(rect.xMin, rect.yMin, rect.width, rect.height);
        }

        public static Vector2 CenterLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.center.y);
        }
        
        public static Vector2 CenterRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.center.y);
        }

        public static Vector2 CenterTop(this Rect rect)
        {
            return new Vector2(rect.center.x, rect.yMax);
        }

        public static Vector2 CenterBottom(this Rect rect)
        {
            return new Vector2(rect.center.x, rect.yMin);
        }
        
        public static Vector2Int RandomPos(this RectInt rect)
        {
            return new Vector2Int(UnityEngine.Random.Range(rect.xMin, rect.xMax), UnityEngine.Random.Range(rect.yMin, rect.yMax));
        }

        public static Rect WithXY(this Rect rect, float xMin, float yMin)
        {
            return new Rect(xMin, yMin, rect.width, rect.height);
        }

        public static Rect WithX(this Rect rect, float xMin)
        {
            return new Rect(xMin, rect.yMin, rect.width, rect.height);
        }
        
        public static Rect WithY(this Rect rect, float yMin)
        {
            return new Rect(rect.xMin, yMin, rect.width, rect.height);
        }
        
        public static Vector2 RandomPoint(this Rect rect)
        {
            return new Vector2(UnityEngine.Random.Range(rect.xMin, rect.xMax), UnityEngine.Random.Range(rect.yMin, rect.yMax));
        }

        public static Rect WidthSegment(this Rect rect, int parts, int index, float spacing = 0f)
        {
            var segmentSize = rect.width / parts;
            return new Rect(rect.xMin + index * segmentSize, rect.yMin, segmentSize - (parts - 1 == index ? 0f : spacing), rect.height);
        }

        public static Rect WithWidth(this Rect rect, float width)
        {
            return new Rect(rect.xMin, rect.yMin, width, rect.height);
        }
        public static Rect WithHeight(this Rect rect, float height)
        {
            return new Rect(rect.xMin, rect.yMin, rect.width, height);
        }
        public static Rect IncHeight(this Rect rect, float addHeight)
        {
            return new Rect(rect.x, rect.y, rect.width, rect.height + addHeight);
        }
        public static Rect IncHeight(this Rect rect, float addHeight, Vector2 pivot)
        {
            return new Rect(rect.x, rect.y + pivot.y * addHeight, rect.width, rect.height + addHeight);
        }

        public static Rect Inflate(this Rect rect, float amound)
        {
            return rect.Inflate(amound, amound);
        }
        
        public static Rect Inflate(this Rect rect, float x, float y)
        {
            return new Rect(rect.xMin - x, rect.yMin - y,
                            rect.width + x * 2, rect.height + y * 2);
        }
        
        public static IEnumerable<Vector2> Corners(this Rect rect)
        {
            yield return new Vector2(rect.xMin, rect.yMin);
            yield return new Vector2(rect.xMin, rect.yMax);
            yield return new Vector2(rect.xMax, rect.yMax);
            yield return new Vector2(rect.xMax, rect.yMin);
        }
        
        public static IEnumerable<Line> Lines(this Rect rect)
        {
            var lb = new Vector2(rect.xMin, rect.yMin);
            var lt = new Vector2(rect.xMin, rect.yMax);
            var rt = new Vector2(rect.xMax, rect.yMax);
            var rb = new Vector2(rect.xMax, rect.yMin);
            
            yield return new Line(lb, lt);
            yield return new Line(lt, rt);
            yield return new Line(rt, rb);
            yield return new Line(rb, lb);
        }


        public static Rect MulXY(this Rect rect, float mul)
        {
            return new Rect(rect.xMin * mul, rect.yMin * mul, rect.width, rect.height);
        }
        public static Rect IncXY(this Rect rect, float addX, float addY)
        {
            return new Rect(rect.xMin + addX, rect.yMin + addY, rect.width, rect.height);
        }

        public static Rect IncXY(this Rect rect, Vector2 offset)
        {
            return rect.IncXY(offset.x, offset.y);
        }
        
        public static Rect IncX(this Rect rect, float addX )
        {
            return new Rect(rect.xMin + addX, rect.yMin, rect.width, rect.height);
        }
        public static Rect IncY(this Rect rect, float addY)
        {
            return new Rect(rect.xMin, rect.yMin + addY, rect.width, rect.height);
        }

        public static Rect IncWidth(this Rect rect, float addWidth)
        {
            return new Rect(rect.x, rect.y, rect.width + addWidth, rect.height);
        }
        public static Rect IncWidth(this Rect rect, float addWidth, Vector2 pivot)
        {
            return new Rect(rect.x + pivot.x * addWidth, rect.y, rect.width + addWidth, rect.height);
        }
        
        public static float ScaleX(this Rect rect, float x)
        {
            return ((x - rect.x) / rect.width).Clamp01();
        }
        public static Vector2 Clamp(this Rect rect, Vector2 v)
        {
            return new Vector2(Mathf.Clamp(v.x, rect.xMin, rect.xMax), Mathf.Clamp(v.y, rect.yMin, rect.yMax));
        }

        public static IEnumerable<Vector2Int> Enumerate(this RectInt rect)
        {
            for (var y = rect.yMin; y < rect.yMax; y++)
            for (var x = rect.xMin; x < rect.xMax; x++)
                yield return new Vector2Int(x, y);
        }
        
        public static RectInt Clamp(this RectInt rect, RectInt region)
        {
            if (region.xMin < rect.xMin)
                region.xMin = rect.xMin;
            
            if (region.xMax > rect.xMax)
                region.xMax = rect.xMax;
            
            if (region.yMin < rect.yMin)
                region.yMin = rect.yMin;

            if (region.yMax > rect.yMax)
                region.yMax = rect.yMax;

            return region;
        }
        
        /// <summary>
        /// clamp min inclusive, max exlusive
        /// </summary>
        public static Vector2Int Clamp(this RectInt rect, Vector2Int pos)
        {
            if (pos.x < rect.xMin)
                pos.x = rect.xMin;
            else
            if (pos.x >= rect.xMax)
                pos.x = rect.xMax - 1;
            
            if (pos.y < rect.yMin)
                pos.y = rect.yMin;
            else
            if (pos.y >= rect.yMax)
                pos.y = rect.yMax - 1;
            
            return pos;
        }

        public static RectInt WithXY(this RectInt rect, int xMin, int yMin)
        {
            return new RectInt(xMin, yMin, rect.width, rect.height);
        }

        public static RectInt WithWH(this RectInt rect, int width, int height)
        {
            return new RectInt(rect.xMin, rect.yMin, width, height);
        }

        public static Rect ToRectXY(this Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        public static Rect ToRectXZ(this Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;
            return new Rect(min.x, min.z, max.x - min.x, max.z - min.z);
        }

        #endregion

        #region Texture

        public static RectInt GetRectInt(this Texture2D texture)
        {
            return new RectInt(0, 0, texture.width, texture.height);
        }
        public static Rect GetRect(this Texture2D texture)
        {
            return new Rect(0, 0, texture.width, texture.height);
        }
        public static Vector2Int GetSize(this Texture2D texture)
        {
            return new Vector2Int(texture.width, texture.height);
        }
        
        public static Texture2D Copy(this Texture2D texture)
        {
            return texture.Copy(texture.format);
        }
        public static Texture2D Clear(this Texture2D texture, Color color)
        {
            texture.SetPixels(Enumerable.Repeat(color, texture.GetSize().Square()).ToArray());
            texture.Apply();
            
            return texture;
        }

        public static Texture2D Copy(this Texture2D texture, TextureFormat format)
        {
            if (texture.isReadable == false)
                return texture.Rescale(texture.width, texture.height, format);
            
            var dst = new Texture2D(texture.width, texture.height, format, 0, true);
            dst.filterMode = texture.filterMode;
            dst.wrapMode = texture.wrapMode;
            dst.SetPixelData(texture.GetRawTextureData<byte>(), 0);
            dst.Apply();
            
            return dst;
        }

        public static Texture2D Copy(this Texture2D source, int width, int height, bool getPixelBilinear = true, TextureFormat format = TextureFormat.RGBA32)
        {
            if (source.width == width && source.height == height)
                return source.Copy();

            var result  = new Texture2D(width, height, format, false, false);
            var pixels = new Color[width * height];
            var incX    = (1f / width);
            var incY    = (1f / height);

            if (getPixelBilinear)
            {
                for (var px = 0; px < pixels.Length; px++)
                    pixels[px] = source.GetPixelBilinear(incX * (px % width), incY * (px / (float)width));
            }
            else
            {
                for (var px = 0; px < pixels.Length; px++)
                    pixels[px] = source.GetPixel((incX * (px % width)).FloorToInt(), (incY * (px / (float)width)).FloorToInt());
            }

            result.SetPixels(pixels, 0);
            result.Apply();
            return result;
        }

        public static Texture2D Copy(this Texture2D texture, RectInt rect, TextureFormat format)
        {
            var dst = new Texture2D(rect.width, rect.height, format, false, false);
            try
            {
                dst.filterMode = texture.filterMode;
                dst.wrapMode = texture.wrapMode;
                dst.SetPixels(0, 0, rect.width, rect.height, texture.GetPixels(rect.x, rect.y, rect.width, rect.height), 0);
                dst.Apply();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Can't copy texture {e}");
            }

            return dst;
        }

        public static Texture2D Rescale(this Texture2D texture, int width, int height, TextureFormat format)
        {
            var rt = new RenderTexture(width, height, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.None);
            RenderTexture.active = rt;
            Graphics.Blit(texture, rt);
            var result = new Texture2D(width, height, format, texture.mipmapCount > 0);
            result.filterMode = texture.filterMode;
            result.wrapMode = texture.wrapMode;
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            
            RenderTexture.active = null;
            rt.Release();
            
            return result;
        }

        #endregion

        #region Editor

        [Conditional("UNITY_EDITOR")]
        public static void SetDirty(this Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }
        
        public static IEnumerable<T> FindAssets<T>(string lable = null) where T : Object
        {
#if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets((lable.IsNullOrEmpty() ? "" : ("l:" + lable + " ")) + "t:" + typeof(T).Name);

            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var asset in assets.OfType<T>())
                {
                    if (asset != null)
                        yield return asset;
                }
            }
#endif
            yield break;
        }
        
#if UNITY_EDITOR
        public class DoCreateFile : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public Action<string> _create;
            
            // =======================================================================
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                _create(pathName);
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
            }
        }
        
        public static DoCreateFile CreateAsset(Action<string> onCreate)
        {
            var result = ScriptableObject.CreateInstance<DoCreateFile>();
            result._create = onCreate;
            return result;
        }
#endif

        #endregion
        
        public static void SetSpeed(this PlayableDirector pd, float speed)
        {
            // the graph must be created before getting the playable graph
            if (pd.playableGraph.IsValid() == false)
                pd.RebuildGraph();
            
            pd.playableGraph.GetRootPlayable(0).SetSpeed(speed);
        }
        
        public static WaitWhile Wait(this PlayableDirector pd)
        {
            return new WaitWhile(() => pd.state == PlayState.Playing);
        }
        
		public static Vector3 RandomPoint(this Camera cam)
        {
            var h = cam.transform.up * (cam.orthographicSize * 2f).Amplitude();
            var w = cam.transform.right * (cam.orthographicSize * cam.aspect * 2f).Amplitude();
            
            
            return cam.transform.position + w + h;
        }

        public static Bounds OrthographicBounds(this Camera camera)
        {
            var  screenAspect = (float)Screen.width / (float)Screen.height;
            var  cameraHeight = camera.orthographicSize * 2;
            var bounds        = new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }
        
		public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
		{
			return new UnityWebRequestAwaiter(asyncOp);
		}
        
        public static IEnumerable<T> ReadCVS<T>(this TextAsset asset)
        {
            using var stream = new MemoryStream(asset.bytes, false);
            using var reader = new StreamReader(stream);
            using var cvs = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            foreach (var record in cvs.GetRecords<T>())
                yield return record;
        }
        
        public static void Set(this BoxCollider2D box, Rect rect)
        {
            box.Set(rect.center, rect.size);
        }

        public static void Set(this BoxCollider2D box, Vector2 offset, Vector2 size)
        {
            box.offset = offset;
            box.size = size;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TRef Ref<TRef>(this IRef reference) where TRef : IRef
        {
            return ((TRef)reference);
        }
        
        public static float LineDistance(in Vector3 a1, in Vector3 a2, in Vector3 point)
        {
            return Vector3.Distance(ClosestLinePoint(in a1, in a2, in point),point);
        }

        public static Vector3 ClosestLinePoint(in Vector3 a1, in Vector3 a2, in Vector3 point)
        {
            var wander = point - a1;
            var span   = a2 - a1;

            var t = Vector3.Dot(wander, span) / span.sqrMagnitude;

            t = Mathf.Clamp01(t);

            return a1 + t * span;
        }

        public static bool LineIntersection(in Vector2 a1, in Vector2 a2, in Vector2 b1, in Vector2 b2, out Vector2 intersection)
        {
            intersection = default;

            var d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x);

            if (d == 0f)
                return false;

            var u = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
            var v = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d;

            if (u < 0f || u > 1f || v < 0f || v > 1f)
                return false;

            intersection.x = a1.x + u * (a2.x - a1.x);
            intersection.y = a1.y + u * (a2.y - a1.y);

            return true;
        }

        public static bool HasComponent<T>(this GameObject go)
            where T : Component
        {
            return go.GetComponent<T>().IsNull() == false;
        }
        
        public static int GetAbsoluteSiblingWeight(this Transform trans)
        {
            var result = 0;
            var mul = 1;
            
            do
            {
                result += trans.GetSiblingIndex() * mul;
                mul += 100;
                trans = trans.parent;
            }
            while (trans != null);
            
            return result;
        }

        public static void InvokeSignal(this GameObject go, SignalAsset signal, bool includeInactive = false)
        {
            foreach (var trigger in go.GetComponentsInChildren<OnSignal>(includeInactive))
                trigger.React(signal);
        }
        
        public static string GetGameObjectPath(this GameObject obj, GameObject root)
        {
            var path = obj.name;
            while (obj.transform.parent != root.transform)
            {
                obj  = obj.transform.parent.gameObject;
                path = $"{obj.name}/{path}";
            }
            
            return path;
        }
        
        public static TileBase[] AllTiles(this Tilemap tilemap)
        {
            return tilemap.GetTilesBlock(tilemap.cellBounds);
        }
        
        public static void IgnoreColisions(this Rigidbody2D rb, Collider2D dest)
        {
            foreach (var source in rb.Colliders())
                Physics2D.IgnoreCollision(source, dest, true);
        }
        
        public static IEnumerable<Collider2D> Colliders(this Rigidbody2D rb)
        {
            var count = rb.GetAttachedColliders(s_Collider2DBuffer);
            for (var n = 0; n < count; n++)
                yield return s_Collider2DBuffer[n];
        }
        
        public static bool TryExtract<T>(this Collider2D collider, out T comp) where T : class
        {
            comp = default;

            // get from rb or collider
            if (collider.attachedRigidbody.IsNull())
            {
                if (collider.TryGetComponent(out comp) == false)
                    return false;
            }
            else
            {
                if (collider.attachedRigidbody.TryGetComponent(out comp) == false)
                    return false;
            }

            return true;
        }
        
        public static bool HasComponent<T>(this Component comp)
            where T : Component
        {
            return comp.GetComponent<T>().IsNull() == false;
        }
        
        public static void DestroyGo(this Component obj)
        {
            Object.Destroy(obj.gameObject);
        }
        
        public static void DestroyGo(this Component obj, float delay)
        {
            Object.Destroy(obj.gameObject, delay);
        }

        public static T SceneData<T>(this Component obj)
            where T : SceneData
        {
            if (SceneManager.Instance.m_SceneData.TryGetValue(obj.gameObject.scene.handle, out var data))
                return data as T;

            return null;
        }

        public static T Instantiate<T>(this T obj) where T : Object
        {
            return Object.Instantiate(obj);
        }
        
        public static T Instantiate<T>(this T obj, Transform root) where T : Object
        {
            return Object.Instantiate(obj, root);
        }
        
        public static T Instantiate<T>(this T obj, Vector3 pos) where T : Object
        {
            return Object.Instantiate(obj, pos, Quaternion.identity);
        }
        public static T Instantiate<T>(this T obj, Vector3 pos, Quaternion rot) where T : Object
        {
            return Object.Instantiate(obj, pos, rot);
        }
        
        public static void Destroy(this Object obj)
        {
            Object.Destroy(obj);
        }

        public static void Destroy(this Object obj, float delay)
        {
            Object.Destroy(obj, delay);
        }

        public static void DestroyImmediate(this Object obj)
        {
            Object.DestroyImmediate(obj);
        }

        public static TValue GetValue<TValue>(this IValueProvider valueProvider)
        {
            var castResult = valueProvider as IValueProvider<TValue>;
            if (castResult == null)
                return default;

            return castResult.Value;
        }

        public static TValue Value<TValue>(this IValueProvider<TValue> valueProvider)
        {
            return valueProvider.Value;
        }

        // =======================================================================
        public static void Log(this object obj)
        {
            Debug.Log(obj);
        }

        public static bool IsNull(this object obj)
        {
            return ReferenceEquals(obj, null);
        }
        
        public static string TrimStart(this string str, string trim)
        {
            if (str.StartsWith(trim))
                return str.Substring(trim.Length);

            return str;
        }
        public static string TrimEnd(this string str, string trim)
        {
            if (str.EndsWith(trim))
                return str.Substring(0, str.Length - trim.Length);

            return str;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        
        public static TEnum ToEnum<TEnum>(this string str)
            where TEnum : unmanaged, Enum
        {
            Enum.TryParse(str, out TEnum result);
            return result;
        }
        
        public static bool HasFlags<TEnum>(this TEnum e, TEnum flags)
            where TEnum : struct, Enum, IConvertible
        {
            var a = Convert.ToInt32(e);
            var b = Convert.ToInt32(flags);
            return (a & b) != 0;
        }

        public static bool IsOnScreen(this Transform t) 
        {
            var screenPos = Camera.current.WorldToViewportPoint(t.position);
            return screenPos.z > 0 && screenPos.x > 0 && screenPos.y > 0 && screenPos.x < 1 && screenPos.y < 1;
        }

        public static Vector3 ScreenPos(this Transform trans)
        {
            return Core.Camera.WorldToScreenPoint(trans.position);
        }

        public static Vector3 ScreenPosUV(this Transform trans)
        {
            var screenPos = Core.Camera.WorldToScreenPoint(trans.position);
            return new Vector3(screenPos.x / Screen.width, screenPos.y  / Screen.height, screenPos.z);
        }

        public static Vector2 GUIPos(this Transform trans)
        {
            var screenPos = Core.Camera.WorldToScreenPoint(trans.position);
            return new Vector2(screenPos.x, Core.Camera.pixelHeight - screenPos.y);
        }

        /*public static bool CompareTagSafe(this GameObject go, string tag)
        {
            go.CompareTag(string.IsNullOrEmpty(tag)
        }*/
        public static IEnumerable<object> ToEnumerable(this IEnumerator iter)
        {
            while (iter.MoveNext())
                yield return iter.Current;
        }

        public static T FirstOfType<T>(this IEnumerable<object> enumerable)
        {
            return (T)enumerable.First(n => n is T);
        }
        
        public static T FirstOrDefaultOfType<T>(this IEnumerable<object> enumerable)
        {
            var v = enumerable.FirstOrDefault(n => n is T);
            if (v != default)
                return (T)v;

            return default;
        }
        
        public static T FirstOrDefaultOfType<T>(this IEnumerable<object> enumerable, Predicate<T> condition)
        {
            var v = enumerable.FirstOrDefault(n => n is T cast && condition(cast));
            if (v != default)
                return (T)v;

            return default;
        }

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator) 
        {
            while (enumerator.MoveNext()) 
                yield return enumerator.Current;
            
            enumerator.Dispose();
        }

        public static IEnumerator<T> ToCircular<T>(this IEnumerable<T> t) 
        {
            return new CircularEnumarator<T>(t.GetEnumerator());
        }
        
        public static IEnumerable<T> GetColumn<T>(this T[,] t, int column) 
        {
            var height = t.GetLength(1);
            for (var y = 0; y < height; y++)
                yield return t[column, y];
        }
        
        public static T[,] ToArray2D<T>(this IEnumerable<T> t, int width, int height) 
        {
            using var e = t.GetEnumerator();
            
            var result = new T[width, height];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                result[x, y] = e.Next();
            
            return result;
        }
        
        
        public static void SetValue<T>(this IList<T> list, int index, in T value)
        {
            if (list.Count <= index)
                return;
            
            list[index] = value;
        }
        
        public static bool TrySetValue<T>(this IList<T> list, int index, in T value)
        {
            if (list.Count <= index)
                return false;
            
            list[index] = value;
            
            return true;
        }
        
        public static bool TryGetValue<T>(this IEnumerable<T> enumerable, Func<T, bool> check, out T value) 
            where T : class
        {
            value = enumerable.FirstOrDefault(check);
            return value.IsNull() == false;
        }
        
        public static bool TryGetValue<T>(this IEnumerable enumerable, out T value) 
            where T : class
        {
            foreach (var n in enumerable)
            {
                if (n is T result)
                {
                    value = result;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public static IEnumerable<(int index, T item)> Enumerate<T>(this IEnumerable<T> t) 
            where T : class
        {
            var index = 0;
            var enumerator = t.GetEnumerator();
            while (enumerator.MoveNext())
                yield return (index ++, enumerator.Current);
            enumerator.Dispose();
        }

        public static T Next<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator.MoveNext())
                return enumerator.Current;

            return default;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T item)
        {
            return enumerable.Where(n => Equals(n, item) == false);
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> items)
        {
            var itemsArray = items as T[] ?? items.ToArray();
            return GetPermutations(itemsArray, itemsArray.Length);
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> items, int count)
        {
            var itemsArray = items as T[] ?? items.ToArray();
            return GetPermutations(itemsArray, count);
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(T[] items, int count)
        {
            if (count == 1)
            {
                foreach (var item in items)
                    yield return new T[] {item};

                yield break;
            }

            foreach(var item in items)
            {
                foreach (var result in GetPermutations(items.Except(new [] { item }).ToArray(), count - 1))
                    yield return new T[] { item }.Concat(result);
            }
        }

        public static string FullActionName(this InputAction inputAction)
        {
            if (inputAction.actionMap == null)
                return inputAction.name;

            return inputAction.actionMap.name + '/' + inputAction.name;
        }


        private class GeneralPropertyComparer<T,TKey> : IEqualityComparer<T>
        {
            private Func<T, TKey> expr { get; }

            public GeneralPropertyComparer (Func<T, TKey> expr)
            {
                this.expr = expr;
            }

            public bool Equals(T left, T right)
            {
                var leftProp  = expr.Invoke(left);
                var rightProp = expr.Invoke(right);

                if (leftProp == null && rightProp == null)
                    return true;

                if (leftProp == null ^ rightProp == null)
                    return false;

                return leftProp.Equals(rightProp);
            }
            public int GetHashCode(T obj)
            {
                var prop = expr.Invoke(obj);
                return (prop==null)? 0:prop.GetHashCode();
            }
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.Distinct(new GeneralPropertyComparer<T,TKey>(property));
        }

        public static LinkedListNode<T> FirstOrDefault<T>(this LinkedList<T> source, Func<LinkedListNode<T>, bool> predicate)
        {
            for (var current = source.First; current != null;  current = current.Next)
                if (predicate(current))
                    return current;

            return null;
        }
        
        public static bool Implements<T>(this Type type) where T : class
        {
            return typeof(T).IsAssignableFrom(type);
        }
        public static bool Implements(this Type type, Type t)
        {
            return t.IsAssignableFrom(type);
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return Attribute.GetCustomAttribute(type, typeof(T)) != null;
        }

        public static bool DynamicCast<TType>(this object source, out TType result)
        {
            var srcType = source.GetType();
            var destType = typeof(TType);

            if (srcType == destType)
            {
                result = (TType)source;
                return true;
            }
            
            if (destType.IsEnum)
            {
                result = (TType)Enum.ToObject(destType, srcType);
                return true;
            }

            if (s_DynamicCastCache.TryGetValue((srcType, destType), out var cast) == false)
            {
                cast = GetCastOperator<TType>(source);
                s_DynamicCastCache.Add((srcType, destType), cast);
            }

            if (cast == null)
            {
                result = default;
                return false;
            }

            result = (TType)cast.Invoke(null, new object[] { source });

            return true;

        }

        public static MethodInfo GetCastOperator<TType>(this object source)
        {
            var srcType = source.GetType();
            var destType = typeof(TType);

            while (srcType != null)
            {
                var cast = srcType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                  .Where(mi =>
                                  {
                                      if ((mi.Name == "op_Explicit" || mi.Name == "op_Implicit") == false)
                                          return false;

                                      if (mi.ReturnType != destType)
                                          return false;

                                      var pars = mi.GetParameters();
                                      //if (pars.Length != 1 || pars[0].ParameterType != srcType)
                                      if (pars.Length != 1 || pars[0].ParameterType.IsAssignableFrom(srcType) == false)
                                          return false;

                                      return true;
                                  })
                                  .FirstOrDefault();

				if (cast != null)
					return cast;
				
                srcType = srcType.BaseType;
            }

            return null;
        }

        public static Type GetGenericTypeArgument(this Type source, Type genericTypeDefinition, int index = 0)
        {
            var current = source;

            while (current != null)
            {
                if (current.IsGenericType)
                {
                    var gtd = current.GetGenericTypeDefinition();
                    if (gtd == genericTypeDefinition)
                        return current.GetGenericArguments()[index];
                }
                current = current.BaseType;
            }
            
            // try get from interface
            foreach (var inter in source.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == genericTypeDefinition)
                    return inter.GetGenericArguments()[index];
            }
            
            return null;
        }
        public static bool HasGetGenericTypeArgument(this Type source, Type genericTypeDefinition, Type type, int index = 0)
        {
            var current = source;

            while (current != null)
            {
                if (current.IsGenericType)
                {
                    var gtd = current.GetGenericTypeDefinition();
                    if (gtd == genericTypeDefinition && current.GetGenericArguments()[index] == type)
                        return true;
                }
                current = current.BaseType;
            }
            
            // try get from interface
            foreach (var inter in source.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == genericTypeDefinition && inter.GetGenericArguments()[index] == type)
                    return true;
            }
            
            return false;
        }
        
        public static IEnumerable<Type> GetBaseTypes(this Type source)
        {
            var current = source;

            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
        
        public static IEnumerable<T> GetFields<T>(this object obj)
        {
            return obj.GetType()
                      .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                      .Where(n => typeof(T).IsAssignableFrom(n.FieldType))
                      .Select(n => (T)n.GetValue(obj));
        }
        
        public static T[] GetEnum<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }
        public static int GetEnumIndex<T>(int value) where T : Enum
        {
            return Array.IndexOf(GetEnum<T>(), (T)(object)value);
        }
        public static T GetEnumValue<T>(int index) where T : Enum
        {
            return GetEnum<T>()[index];
        }

        public static void DrawCircle(Vector3 pos, float radius, Vector3 up, Color color, int segments = 20, float duration = 0)
        {
            DrawEllipse(pos, Quaternion.LookRotation(up), radius, radius, color, segments, duration);
        }

        public static void DrawEllipse(Vector3 pos, float radius, Color color, int segments = 20, float duration = 0)
        {
            DrawEllipse(pos, Quaternion.identity, radius, radius, color, segments, duration);
        }

        public static void DrawEllipse(Vector3 pos, Quaternion rotation, float radiusX, float radiusY, Color color, int segments, float duration = 0)
        {
            var angle     = 0f;
            var rot       = rotation;
            var lastPoint = Vector3.zero;
            var thisPoint = Vector3.zero;
 
            for (var i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;
 
                if (i > 0)
                {
                    Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
                }
 
                lastPoint =  thisPoint;
                angle     += 360f / segments;
            }
        }

        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return GetEnum<T>();
        }

        public static IEnumerable<T> GetValues<T>(this Enum en) where T : Enum
        {
            return GetEnum<T>();
        }

        public static IEnumerable<T> GetFlags<T>(this T en) where T : Enum
        {
            foreach (T value in Enum.GetValues(typeof(T)))
                if (en.HasFlag(value))
                    yield return value;
        }

        public static T NextEnum<T>(this T en) where T : Enum
        {
            // get values
            var valueList = GetEnum<T>().ToList();

            // get value index
            var index = valueList.IndexOf(en);

            // reset or increment index
            if (index < 0 || (index + 1) >= valueList.Count)
                index = 0;
            else
                index ++;

            return valueList[index];
        }

        public static void RemoveAllBut<T>(this List<T> source, Predicate<T> predicate)
        {
            source.RemoveAll(inverse);

            bool inverse(T item) => !predicate(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this ICollection collection)
        {
            return collection.Count > 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        /*public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            //nothing to add
            if (other == null)
                return;

            foreach (var obj in other)
            {
                collection.Add(obj);
            }
        }*/

        public static IEnumerable<Transform> GetSiblings(this Transform trans)
        {
            var parent = trans.parent;
            if (parent == null)
                yield break;

            foreach (var sibling in parent.GetChildren())
            {
                if (sibling == trans)
                    continue;
                
                yield return sibling;
            }
        }
        
        public static IEnumerable<Transform> GetChildren(this GameObject go)
        {
            for (var n = 0; n < go.transform.childCount; n++)
                yield return go.transform.GetChild(n);
        }
        
        public static IEnumerable<T> GetChildren<T>(this GameObject go)
        {
            for (var n = 0; n < go.transform.childCount; n++)
                if (go.transform.GetChild(n).TryGetComponent<T>(out var cmp))
                    yield return cmp;
        }
        
        public static GameObject CopyHierarchy(this GameObject go, Action<GameObject, GameObject> onCopy)
        {
            var root = new GameObject(go.name);
            onCopy(go, root);
            _copyHierarchy(go, root);
            
            return root;
            
            // -----------------------------------------------------------------------
            void _copyHierarchy(GameObject initial, GameObject cur)
            {
                foreach (var child in initial.GetChildren())
                {
                    var copy = new GameObject(child.name);
                    copy.transform.SetParent(cur.transform);
                    
                    onCopy(child.gameObject, copy);
                    
                    _copyHierarchy(child.gameObject, copy);
                }
            }
        }
        
        public static T GetComponentInParentLast<T>(this GameObject go) where T : Component
        {
            var cmp = go.GetComponentInParent<T>();
            if (cmp == null)
                return null;
            
            while (true)
            {
                if (cmp.transform.parent == null)
                    break;
                
                var cmpRoot = cmp.transform.parent.GetComponentInParent<T>();
                if (cmpRoot == null)
                    break;
                
                cmp = cmpRoot;
            }
            
            return cmp;
        }
        
        public static IEnumerable<T> GetComponentInParentAll<T>(this GameObject go) where T : Component
        {
            var cmp = go.GetComponentInParent<T>();
            
            while (true)
            {
                if (cmp == null)
                    yield break;
                
                yield return cmp;
                
                if (cmp.transform.parent == null)
                    break;
                
                cmp = cmp.transform.parent.GetComponentInParent<T>();
            }
        }

        public static TComponent AddChild<TComponent>(this Transform transform, TComponent prefab, Action<TComponent> setup = null, string name = "")
            where TComponent : Object
        {
            var mb = Object.Instantiate(prefab, transform, false);
            if (name.IsNullOrEmpty() == false)
                mb.name = name;
            setup?.Invoke(mb);

            return mb;
        }

        public static TComponent AddChild<TComponent>(this Transform transform, Action<TComponent> setup = null, string name = "") 
            where TComponent : Component
        {
            var go = new GameObject(name.IsNullOrEmpty() ? nameof(TComponent): name);
            go.SetActive(false);
            go.transform.SetParent(transform);
            
            var mb = go.AddComponent<TComponent>();
            setup?.Invoke(mb);

            go.SetActive(true);

            return mb;
        }

        public static void DestroyChildren(this GameObject obj)
        {
            var childList = obj.GetChildren().ToArray();

#if UNITY_EDITOR
            if (Application.isPlaying)
                foreach (var child in childList)
                    Object.Destroy(child.gameObject);
            else
                foreach (var child in childList)
                    Object.DestroyImmediate(child.gameObject);
#else
        foreach (var child in childList)
            UnityEngine.Object.Destroy(child.gameObject);
#endif
        }
        
        public static bool Has<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return source.ContainsKey(key);
        }

        public static bool Has<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Any(predicate);
        }
    
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, TSource noOptionsValue = default)
        {
            return source.MinBy(selector, Comparer<TKey>.Default, noOptionsValue);
        }
    
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, TSource noOptionsValue = default)
        {
            using (var sourceIterator = source.GetEnumerator())
            {
                if (sourceIterator.MoveNext() == false)
                    return noOptionsValue;

                var min = sourceIterator.Current;
                var minKey = selector(min);
	
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        public static TSource MaxByOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, TSource noOptionsValue = default)
        {
            return source.MaxByOrDefault(selector, Comparer<TKey>.Default, noOptionsValue);
        }
    
        public static TSource MaxByOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, TSource noOptionsValue = default)
        {
            using (var sourceIterator = source.GetEnumerator())
            {
                if (sourceIterator.MoveNext() == false)
                    return noOptionsValue;

                var max = sourceIterator.Current;
                var maxKey = selector(max);
	
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            for (var n = 0; n < transform.childCount; n++)
                yield return transform.GetChild(n);
        }

        public static IEnumerable<T> GetChildren<T>(this Transform transform) where T : Component
        {
            for (var n = 0; n < transform.childCount; n++)
            {
                if (transform.GetChild(n).TryGetComponent(out T comp))
                    yield return comp;
            }
        }

        public static IEnumerable<T> GetChildren<T>(this Transform transform, bool includeInactive) where T : Component
        {
            for (var n = 0; n < transform.childCount; n++)
            {
                var child = transform.GetChild(n);
                var isValid = includeInactive || child.gameObject.activeSelf;
                if (isValid && child.TryGetComponent(out T comp))
                    yield return comp;
            }
        }
        
        public static IEnumerable<Transform> GetChildrenRecursively(this Transform transform)
        {
            for (var n = 0; n < transform.childCount; n++)
            {
                var child = transform.GetChild(n);
                yield return child;

                foreach (var grandchild in child.GetChildrenRecursively())
                    yield return grandchild;
            }
        }

        public static T CloneOrDefault<T>(this T scriptableObject) where T : ScriptableObject
        {
            if (scriptableObject == null)
                return (T)ScriptableObject.CreateInstance(typeof(T));

            var instance = Object.Instantiate(scriptableObject);
            instance.name = scriptableObject.name; // remove (Clone) from name
            return instance;
        }

        public static IEnumerable<TResult> Sample<TType, TResult>(this IList<TType> list, Func<TType, TType, TResult> sample, int step)
        {
            var steps = step == 1 ? list.Count - 1 : list.Count / step;
            for (var n = 0; n < steps; n++)
                yield return sample(list[n * step], list[n * step + 1]);
        }

        public static void AddUnique<T>(this IList list, T item)
        {
            if (list.Contains(item))
                return;

            list.Add(item);
        }
        
        public static int IndexOf<T>(this T[] array, T item)
        {
            return Array.IndexOf(array, item);
        }
        public static void Clear<T>(this T[] array)
        {
            Array.Clear(array, 0, array.Length);
        }

        public static T Random<T>(this IEnumerable<T> list)
        {
            return UnityRandom.RandomFromList(list.ToArray());
        }

        public static T Random<T>(this IEnumerable<T> list, T drawback)
        {
            return UnityRandom.RandomFromList(list.ToArray(), drawback);
        }

        public static T RandomOrDefault<T>(this IEnumerable<T> list)
        {
            return UnityRandom.RandomFromList(list.ToArray(), default);
        }

        public static T GetClamp<T>(this IList<T> list, int index)
        {
            return list[index.Clamp(0, list.Count - 1)];
        }
        
        public static T GetLoop<T>(this IList<T> list, int index)
        {
            // 0 2 1 | 0 1 2 0 1 2
            index %= list.Count;
            if (index < 0)
                index = list.Count + index;
            
            return list[index];
        }
        
        public static T GetPingPong<T>(this IList<T> list, int index)
        {
            // 2 1 0 | 0 1 2 2 1 0
            if (index < 0)
            {
                index ++;
                index = Mathf.Abs(index);
            }
            
            var isOdd = Mathf.FloorToInt(index / (float)list.Count) % 2 == 1;
            if (isOdd)
            {
                // return inverted
                index = (list.Count - 1) - index % list.Count;
                return list[index];                
            }
            else
            {
                index %= list.Count;
                if (index < 0)
                    index = list.Count + index;

                return list[index];
            }
        }
        
        public static T Random<T>(this IList<T> list)
        {
            return UnityRandom.RandomFromList(list);
        }

        public static T Random<T>(this IList<T> list, T drawback)
        {
            return UnityRandom.RandomFromList(list, drawback);
        }
        
        public static T RandomOrDefault<T>(this IList<T> list)
        {
            return UnityRandom.RandomFromList(list, default);
        }

        public static TList Randomize<TList>(this TList list)
            where TList : IList
        {
            UnityRandom.RandomizeList(list);
            return list;
        }
        
        public static List<T> Randomize<T>(this IEnumerable<T> enumerable)
        {
            var result = enumerable.ToList();
            UnityRandom.RandomizeList(result);
            return result;
        }

        public static bool TryGetValue<T>(this IList<T> list, int index, out T value)
        {
            if (index < 0 || index >= list.Count)
            {
                value = default;
                return false;
            }

            value = list[index];
            return true;
        }
        
        public static T PrevItem<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index == -1 || index - 1 < 0)
                return default;

            return list[index - 1];
        }

        public static T NextItem<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index == -1 || list.Count <= index + 1)
                return default;

            return list[index + 1];
        }

        public static T NextItem<T>(this IList<T> list, T item, out int index)
        {
            index = list.IndexOf(item);
            if (index == -1 || list.Count <= ++index)
                return default;

            return list[index];
        }

        public static T NextItem<T>(this IList<T> list, ref int index)
        {
            if (list.Count <= ++index)
            {
                index = -1;
                return default;
            }

            return list[index];
        }
        
        public static IEnumerable<Vector2Int> ToEnumerableSquare(this Vector2Int vec)
        {
            for (var x = 0; x < vec.x; x++)
            for (var y = 0; y < vec.y; y++)
                yield return new Vector2Int(x, y);
        }

        public static int Square(this Vector2Int vec)
        {
            return vec.x * vec.y;
        }
        
        public static Vector2Int RandomPoint(this Vector2Int vec)
        {
            return new Vector2Int(UnityEngine.Random.Range(0, vec.x), UnityEngine.Random.Range(0, vec.y));
        }
        
        public static int Quantize(this float val)
        {
            return val == 0f ? 0 : (val > 0f ? 1 : -1);
        }
        
        public static Vector2Int Quantize(this Vector2 vector)
        {
            return new Vector2Int(vector.x.Quantize(), vector.y.Quantize());
        }

        public static Vector2 WithX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 WithY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector2 WithIncX(this Vector2 vector, float xInc)
        {
            return new Vector2(vector.x + xInc, vector.y);
        }
        
        public static Vector2 WithIncXY(this Vector2 vector, Vector2 inc)
        {
            return new Vector2(vector.x + inc.x, vector.y + inc.y);
        }
        
        public static Vector2 WithIncXY(this Vector2 vector, float inc)
        {
            return new Vector2(vector.x + inc, vector.y + inc);
        }

        public static Vector2 WithIncY(this Vector2 vector, float yInc)
        {
            return new Vector2(vector.x, vector.y + yInc);
        }
        
        public static Vector2 WithMulX(this Vector2 vector, float xMul)
        {
            return new Vector2(vector.x * xMul, vector.y);
        }

        public static Vector2 WithMulY(this Vector2 vector, float yMul)
        {
            return new Vector2(vector.x, vector.y * yMul);
        }

        public static Vector3 WithIncX(this Vector3 vector, float xInc)
        {
            return new Vector3(vector.x + xInc, vector.y, vector.z);
        }

        public static Vector3 WithIncY(this Vector3 vector, float yInc)
        {
            return new Vector3(vector.x, vector.y + yInc, vector.z);
        }

        public static Vector3 WithIncZ(this Vector3 vector, float zInc)
        {
            return new Vector3(vector.x, vector.y, vector.z + zInc);
        }
        
        public static Vector3 WithMulX(this Vector3 vector, float xMul)
        {
            return new Vector3(vector.x * xMul, vector.y, vector.z);
        }

        public static Vector3 WithMulY(this Vector3 vector, float yMul)
        {
            return new Vector3(vector.x, vector.y * yMul, vector.z);
        }

        public static Vector3 WithMulZ(this Vector3 vector, float zMul)
        {
            return new Vector3(vector.x, vector.y, vector.z * zMul);
        }

        public static Vector3 To3DXZ(this Vector2 vector, float y)
        {
            return new Vector3(vector.x, y, vector.y);
        }

        public static Vector3 To3DXZ(this Vector2 vector)
        {
            return vector.To3DXZ(0);
        }

        public static Vector3 To3DXY(this Vector2 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 To3DXY(this Vector2 vector)
        {
            return vector.To3DXY(0);
        }

        public static Vector3 To3DYZ(this Vector2 vector, float x)
        {
            return new Vector3(x, vector.x, vector.y);
        }

        public static Vector3 To3DYZ(this Vector2 vector)
        {
            return vector.To3DYZ(0);
        }

        public static Vector2 YX(this Vector2 vector)
        {
            return new Vector2(vector.y, vector.x);
        }
        
        public static Vector2 DirTo(this Vector2 v, Vector2 pos)
        {
            return pos - v;
        }

        public static Vector2 To2DXZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector2 To2DXY(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 To2DYZ(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.z);
        }
        
        public static Vector3 DirTo(this Vector3 vector, Vector3 pos)
        {
            return pos - vector;
        }
        
        public static Color ToColor(this Vector3 vector)
        {
            return new Color(vector.x, vector.y, vector.z);
        }

        public static Vector4 WithX(this Vector4 vector, float x)
        {
            return new Vector4(x, vector.y, vector.z, vector.w);
        }

        public static Vector4 WithY(this Vector4 vector, float y)
        {
            return new Vector4(vector.x, y, vector.z, vector.w);
        }
        
        public static Vector4 WithZ(this Vector4 vector, float z)
        {
            return new Vector4(vector.x, vector.y, z, vector.w);
        }
        
        public static Vector4 WithW(this Vector4 vector, float w)
        {
            return new Vector4(vector.x, vector.y, vector.z, w);
        }

        public static float Sum(this Vector2 vector)
        {
            return vector.x + vector.y;
        }

        public static Vector2Int Cell(this Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }
        
        public static Vector3Int Cell(this Vector3 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
        }
        public static Vector2Int CellXY(this Vector3 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }
        public static Vector2Int CellXY(this Vector3 v, Vector2 cellSize)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x / cellSize.x), Mathf.FloorToInt(v.y / cellSize.x));
        }
        public static Vector2 CellCenter(this Vector3Int v)
        {
            return new Vector2(v.x + .5f, v.y + .5f);
        }
        
        public static Vector2 CellCenter(this Vector2Int v)
        {
            return new Vector2(v.x + .5f, v.y + .5f);
        }
        
        public static Vector2 CellCenter(this Vector3 v)
        {
            return new Vector2(Mathf.FloorToInt(v.x) + .5f, Mathf.FloorToInt(v.y) + .5f);
        }

        public static Vector3 Snap(this Vector3 v)
        {
            return Snap(v, in MathLib.V3Half);
        }
        
        public static Vector3 Snap(this Vector3 v, in Vector3 pivot)
        {
            return new Vector3(Mathf.Floor(v.x) + pivot.x, Mathf.Floor(v.y) + pivot.y, Mathf.Floor(v.z) + pivot.z);
        }

        public static float Sum(this Vector3 vector)
        {
            return vector.x + vector.y + vector.z;
        }
        public static bool IsZero(this Vector3 vector)
        {
            return vector == Vector3.zero;
        }

        public static Vector3 YZX(this Vector3 vector)
        {
            return new Vector3(vector.y, vector.z, vector.x);
        }

        public static Vector3 XZY(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.z, vector.y);
        }

        public static Vector3 ZXY(this Vector3 vector)
        {
            return new Vector3(vector.z, vector.x, vector.y);
        }

        public static Vector3 YXZ(this Vector3 vector)
        {
            return new Vector3(vector.y, vector.x, vector.z);
        }

        public static Vector3 ZYX(this Vector3 vector)
        {
            return new Vector3(vector.z, vector.y, vector.x);
        }

        public static Vector3 WithOffsetXZ(this Vector3 vector, float radius)
        {
            return vector + UnityRandom.Normal2D(radius).To3DXZ();
        }

        public static Vector2 ReflectAboutX(this Vector2 vector)
        {
            return new Vector2(vector.x, -vector.y);
        }

        public static Vector2 ReflectAboutY(this Vector2 vector)
        {
            return new Vector2(-vector.x, vector.y);
        }
	
        public static Vector2 Rotate(this Vector2 vector, float rad)
        {
            var cosAngle = Mathf.Cos(rad);
            var sinAngle = Mathf.Sin(rad);

            var x = vector.x * cosAngle - vector.y * sinAngle;
            var y = vector.x * sinAngle + vector.y * cosAngle;

            return new Vector2(x, y);
        }
        
        public static Vector2 CellCenter(this Vector2 v)
        {
            return new Vector2(Mathf.FloorToInt(v.x) + .5f, Mathf.FloorToInt(v.y) + .5f);
        }

        public static Vector2 RotateAround(this Vector2 vector, float rad, Vector2 axisPosition)
        {
            return (vector - axisPosition).Rotate(rad) + axisPosition;
        }

        
        public static Vector2 Rotate90(this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }
        
        public static Vector2 Rotate90CCW(this Vector2 vector)
        {
            return new Vector2(-vector.y, vector.x);
        }

        public static Vector2 Rotate180(this Vector2 vector)
        {
            return new Vector2(-vector.x, -vector.y);
        }

        /// <summary>
        /// Returns the vector rotated 90 degrees counter-clockwise.
        /// </summary>
        /// <remarks>
        /// 	<para>The returned vector is always perpendicular to the given vector. </para>
        /// 	<para>The perp dot product can be calculated using this: <c>var perpDotPorpduct = Vector2.Dot(v1.Perp(), v2);</c></para>
        /// </remarks>
        /// <param name="vector"></param>
        public static Vector2 Perp(this Vector2 vector)
        {
            return vector.Rotate90CCW();
        }
        
        public static Vector2 VectorTo(this Vector2 v, Vector2 target)
        {
            return target - v;
        }
        
        public static Vector2 NormalTo(this Vector2 v, Vector2 target)
        {
            return (target - v).normalized;
        }

        public static float Dot(this Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }
        
        public static float Distance(this Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        public static float Dot(this Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float Dot(this Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary> Returns the projection of this vector onto the given base. </summary>
        public static Vector2 Proj(this Vector2 vector, Vector2 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector2.Dot(vector, direction);

            return direction * magnitude;
        }

        /// <summary> Returns the rejection of this vector onto the given base. </summary>
        public static Vector2 Rej(this Vector2 vector, Vector2 baseVector)
        {
            return vector - vector.Proj(baseVector);
        }

        public static Vector3 Proj(this Vector3 vector, Vector3 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector3.Dot(vector, direction);

            return direction * magnitude;
        }

        /// <summary> Returns the rejection of this vector onto the given base. </summary>
        public static Vector3 Rej(this Vector3 vector, Vector3 baseVector)
        {
            return vector - vector.Proj(baseVector);
        }

        /// <summary>
        /// Returns the projection of this vector onto the given base.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="baseVector"></param>
        public static Vector4 Proj(this Vector4 vector, Vector4 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector2.Dot(vector, direction);

            return direction * magnitude;
        }

        /// <summary>
        /// Returns the rejection of this vector onto the given base.
        /// The sum of a vector's projection and rejection on a base is
        /// equal to the original vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="baseVector"></param>
        public static Vector4 Rej(this Vector4 vector, Vector4 baseVector)
        {
            return vector - vector.Proj(baseVector);
        }

        /// <summary>
        /// Turns the vector 90 degrees anticlockwise as viewed from the top (keeping the y coordinate intact).
        /// Equivalent to <code>v.To2DXZ().Perp().To3DXZ(v.y);</code>
        /// </summary>
        public static Vector3 PerpXZ(this Vector3 v)
        {
            return new Vector3(-v.z, v.y, v.x);
        }

        /// <summary>
        /// Turns the vector 90 degrees anticlockwise as viewed from the front (keeping the z coordinate intact).
        /// Equivalent to <code>v.To2DXY().Perp().To3DXY(v.z);</code>
        /// </summary>
        public static Vector3 PerpXY(this Vector3 v)
        {
            return new Vector3(-v.y, v.x, v.z);
        }
    
        public static Vector2 Mul(this Vector2 v, float x, float y)
        {
            return new Vector2(v.x * x, v.y * y);
        }
        
        public static Vector2 Mul(this Vector2 v, Vector2 mul)
        {
            return new Vector2(v.x * mul.x, v.y * mul.y);
        }

        /// <summary>
        /// Divides one vector component by component by another.
        /// </summary>
        public static Vector2 Div(this Vector2 thisVector, Vector2 otherVector)
        {
            return new Vector2(thisVector.x / otherVector.x, thisVector.y / otherVector.y);
        }
    
        public static Vector3 Mul(this Vector3 thisVector, Vector3 otherVector)
        {
            return new Vector3(
                thisVector.x * otherVector.x, 
                thisVector.y * otherVector.y,
                thisVector.z * otherVector.z);
        }
    
        public static Vector3 Div(this Vector3 thisVector, Vector3 otherVector)
        {
            return new Vector3(
                thisVector.x / otherVector.x, 
                thisVector.y / otherVector.y,
                thisVector.z / otherVector.z);
        }
    
        public static Vector4 Mul(this Vector4 thisVector, Vector4 otherVector)
        {
            return new Vector4(
                thisVector.x * otherVector.x,
                thisVector.y * otherVector.y,
                thisVector.z * otherVector.z,
                thisVector.w * otherVector.w);
        }
        public static Vector4 Div(this Vector4 thisVector, Vector4 otherVector)
        {
            return new Vector4(
                thisVector.x / otherVector.x,
                thisVector.y / otherVector.y,
                thisVector.z / otherVector.z,
                thisVector.w / otherVector.w);
        }

        public static Vector3Int Down(this Vector3Int v)
        {
            return v.WithIncY(-1);
        }
        public static Vector3Int Up(this Vector3Int v)
        {
            return v.WithIncY(1);
        }
        public static Vector3Int Left(this Vector3Int v)
        {
            return v.WithIncX(-1);
        }
        public static Vector3Int Right(this Vector3Int v)
        {
            return v.WithIncX(1);
        }
		public static Vector3Int WithIncX(this Vector3Int v, int incX)
        {
            return new Vector3Int(v.x + incX, v.y, v.z);
        }
		public static Vector3Int WithIncY(this Vector3Int v, int incY)
        {
            return new Vector3Int(v.x, v.y + incY, v.z);
        }
		public static Vector3Int WithIncZ(this Vector3Int v, int incZ)
        {
            return new Vector3Int(v.x, v.y, v.z + incZ);
        }
        public static Vector2Int To2DXY(this Vector3Int v)
        {
            return new Vector2Int(v.x, v.y);
        }
        public static Vector2Int To2DXZ(this Vector3Int v)
        {
            return new Vector2Int(v.x, v.z);
        }
        public static Vector2 ToVector2(this Vector3Int v)
        {
            return new Vector2(v.x, v.y);
        }
        public static Vector3 ToVector3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }


        public static Vector2Int Down(this Vector2Int v)
        {
            return v.WithIncY(-1);
        }
        public static Vector2Int Up(this Vector2Int v)
        {
            return v.WithIncY(1);
        }
        public static Vector2Int Left(this Vector2Int v)
        {
            return v.WithIncX(-1);
        }
        public static Vector2Int Right(this Vector2Int v)
        {
            return v.WithIncX(1);
        }
		public static Vector2Int WithIncX(this Vector2Int v, int incX)
        {
            return new Vector2Int(v.x + incX, v.y);
        }
		public static Vector2Int WithIncY(this Vector2Int v, int incY)
        {
            return new Vector2Int(v.x, v.y + incY);
        }
        public static Vector2 ToVector2(this Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }
        public static Vector3Int To3DXY(this Vector2Int v)
        {
            return new Vector3Int(v.x, v.y, 0);
        }

        public static Vector3Int To3DXZ(this Vector2Int v)
        {
            return new Vector3Int(v.x, 0, v.y);
        }
        
        public static Vector2Int RotateCW(this Vector2Int v)
        {
            return new Vector2Int(v.y, -v.x);
        }
        
        public static Vector2Int RotateCCW(this Vector2Int v)
        {
            return new Vector2Int(-v.y, v.x);
        }
        
        public static Vector2Int One(this Vector2Int v)
        {

            var x = 0;
            if (v.x == 0) x = 0;
            else
            if (v.x > 0) x = 1;
            else
            if (v.x < 0) x = -1;

            var y = 0;
            if (v.y == 0) y = 0;
            else
            if (v.y > 0) y = 1;
            else
            if (v.y < 0) y = -1;

            return new Vector2Int(x, y);
        }

        public static int Hash(this Vector2Int v)
        {
            return (v.x << 16) | v.y;
        }


        public static void SetMax(this Vector2Int v, Vector2Int max)
        {
            v.Set(v.x > max.x ? max.x : v.x,
                v.y > max.y ? max.y : v.y);
        }

        public static void SetMin(this Vector2Int v, Vector2Int min)
        {
            v.Set(v.x < min.x ? min.x : v.x, 
                v.y < min.y ? min.y : v.y);
        }

        public static Vector2Int Center(this Vector2Int v)
        {
            return new Vector2Int(v.x / 2, v.y / 2);
        }

        public static Vector2Int CenterRound(this Vector2Int v)
        {
            return new Vector2Int(Mathf.RoundToInt((float)v.x / 2.0f), Mathf.RoundToInt((float)v.y / 2.0f));
        }

        /// <summary> contains inclusive </summary>
        public static bool Contains(this Vector2Int v, int val)
        {
            return v.x <= val && val <= v.y;
        }
	
        /// <summary>Max value </summary>
        public static int Max(this Vector2Int v)
        {
            return v.x > v.y ? v.x : v.y;
        }

        // min value
        public static int Min(this Vector2Int v)
        {
            return v.x < v.y ? v.x : v.y;
        }
	
        /// <summary>
        /// Returns random value from vector range
        /// </summary>
        public static float Range(this Vector2 v)
        {
            return UnityEngine.Random.Range(v.x, v.y);
        }
        
        public static float Segment(this Vector2 v)
        {
            return (v.x - v.y).Abs();
        }
        
        /// <summary>
        /// Contains value in vector range
        /// </summary>
        public static bool Cointains(this Vector2 v, float val)
        {
            return val >= v.x && val <= v.y;
        }

        public static float ClosesdValue(this Vector2 v, float pos)
        {
            return Mathf.Abs(v.x - pos) < Mathf.Abs(v.y - pos) ? v.x : v.y;
        }

        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(v.x < 0.0f ? -v.x : v.x, v.y < 0.0f ? -v.y : v.y);
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(v.x < 0.0f ? -v.x : v.x, v.y < 0.0f ? -v.y : v.y, v.z < 0.0f ? -v.z : v.z);
        }

        public static Vector3 RotateXY(this Vector3 v, float degree)
        {
            var rad = degree.Deg2Rad();
            var sin = Mathf.Sin(rad);
            var cos = Mathf.Cos(rad);
            return new Vector3(v.x * cos - v.y * sin, v.x * sin + v.y * cos, v.z);
        }

        public static Vector2 ClampLenght(this Vector2 v, float lenghtAbs)
        {
            var lenght = v.magnitude;
            if (lenght > lenghtAbs)
                v *= lenghtAbs / lenght;

            return v;
        }
        
        public static Quaternion ToRotationXY(this float rad)
        {
            //return new Quaternion(0f, 0f, Mathf.Sin(rad), Mathf.Cos(rad));
            return Quaternion.AngleAxis(rad.Rad2Deg(), Vector3.forward);
        }
        
        public static Vector2 ToVector2X(this float value)
        {
            return new Vector2(value, 0);
        }
        public static Vector2 ToVector2X(this float value, float y)
        {
            return new Vector2(value, y);
        }
        public static Vector2 ToVector2Y(this float value)
        {
            return new Vector2(0, value);
        }
        public static Vector2 ToVector2Y(this float value, float x)
        {
            return new Vector2(x, value);
        }
        public static Vector2 ToVector2(this float value)
        {
            return new Vector2(value, value);
        }
    
        public static Vector3 ToVector3X(this float value)
        {
            return new Vector3(value, 0, 0);
        }
        public static Vector3 ToVector3Y(this float value)
        {
            return new Vector3(0, value, 0);
        }
        public static Vector3 ToVector3Z(this float value)
        {
            return new Vector3(0, 0, value);
        }
        public static Vector3 ToVector3(this float value)
        {
            return new Vector3(value, value, value);
        }
        public static Vector3 ToVector3XY(this float value)
        {
            return new Vector3(value, value, 0);
        }
        public static Vector3 ToVector3XY(this float value, float z)
        {
            return new Vector3(value, value, z);
        }
        
        public static Color ToColor(this float value)
        {
            return new Color(value, value, value, value);
        }
        
        public static float ToRelative(this float value, float other)
        {
            return other - value;
        }
        
        public static float AngleDeg(this Vector2 v)
        {
            return (Mathf.Atan2(v.y, v.x)) * Mathf.Rad2Deg;
        }
        public static float AngleRad(this Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x);
        }

        public static Vector2Int Round(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
        public static Vector2Int Ceil(this Vector2 v)
        {
            return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
        }
        public static Vector2Int Floor(this Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }
        public static Rect ToRect(this Vector2 v, float width, float height)
        {
            return new Rect(v.x - width * 0.5f, v.y - height * 0.5f, width, height);
        }
        public static Rect ToRect(this Vector2 v, Vector2 size)
        {
            return v.ToRect(size.x, size.y);
        }
        
        public static IEnumerable<Vector2Int> LeftNeighbours(this Vector2Int v, int count)
        {
            for (var x = 1; x <= count; x++)
                yield return v.WithIncX(-x);
        }
        public static IEnumerable<Vector2Int> RightNeighbours(this Vector2Int v, int count)
        {
            for (var x = 1; x <= count; x++)
                yield return v.WithIncX(x);
        }
        public static IEnumerable<Vector2Int> UpNeighbours(this Vector2Int v, int count)
        {
            for (var y = 1; y <= count; y++)
                yield return v.WithIncY(y);
        }
        public static IEnumerable<Vector2Int> DownNeighbours(this Vector2Int v, int count)
        {
            for (var y = 1; y <= count; y++)
                yield return v.WithIncY(-y);
        }
        /// <summary> MooreNeighborhood 8 king neighborhood </summary>
        public static IEnumerable<Vector2Int> BoxNeighbours(this Vector2Int v)
        {
            foreach (var neighbour in CrossNeighbours(v))
                yield return neighbour;

            yield return new Vector2Int(v.x - 1, v.y + 1);
            yield return new Vector2Int(v.x - 1, v.y - 1);
            yield return new Vector2Int(v.x + 1, v.y + 1);
            yield return new Vector2Int(v.x + 1, v.y - 1);
        }

        /// <summary> VonNeumannNeighborhood 4 cross neighbors </summary>
        public static IEnumerable<Vector2Int> CrossNeighbours(this Vector2Int v)
        {
            yield return new Vector2Int(v.x - 1, v.y);
            yield return new Vector2Int(v.x + 1, v.y);
            yield return new Vector2Int(v.x, v.y + 1);
            yield return new Vector2Int(v.x, v.y - 1);
        }
        
        /// <summary> 4 diagonal neighbors </summary>
        public static IEnumerable<Vector2Int> StartNeighbours(this Vector2Int v)
        {
            yield return new Vector2Int(v.x - 1, v.y - 1);
            yield return new Vector2Int(v.x - 1, v.y + 1);
            yield return new Vector2Int(v.x + 1, v.y + 1);
            yield return new Vector2Int(v.x + 1, v.y - 1);
        }

        public static Vector2 Clamp(this Vector2 v, float min, float max)
        {
            return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        public static Vector3 Clamp(this Vector3 v, float min, float max)
        {
            return new Vector3(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }
        public static Vector3 ClampMagnitude(this Vector3 v, float max)
        {
            return Vector3.ClampMagnitude(v, max);
        }
        public static Vector2 ClampMagnitude(this Vector2 v, float max)
        {
            return Vector2.ClampMagnitude(v, max);
        }

        public static Vector2Int Clamp(this Vector2Int v, RectInt bounds)
        {
            return new Vector2Int(Mathf.Clamp(v.x, bounds.xMin, bounds.xMax), Mathf.Clamp(v.y, bounds.yMin, bounds.yMax));
        }
        public static Vector2Int Clamp(this Vector2Int v, int min, int max)
        {
            return new Vector2Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        public static Vector3Int Clamp(this Vector3Int v, int min, int max)
        {
            return new Vector3Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }
	
        public static Vector2 ToNormal(this float rad, float scale)
        {
            return new Vector2(Mathf.Cos(rad) * scale, Mathf.Sin(rad) * scale);
        }
        
        public static Vector2 ToNormal(this float rad)
        {
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
        public static Vector3 NormalXY(this float rad)
        {
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        public static bool IsAproximatlyZero(this float val)
        {
            return Mathf.Approximately(val, 0.0f);
        }
        public static bool Aproximatly(this float val, float equal)
        {
            return Mathf.Approximately(val, equal);
        }
        
        public static IEnumerable<int> BitPositions(this int val, bool positiveBits = true)
        {
            var bit = 1;
            for (var n = 0; n < 32; n++)
            {
                if (((val & bit) != 0) == positiveBits)
                    yield return n;

                bit <<= 1;
            }
        }

        public static uint ToIndex(this Vector2Int v)
        {
            return ToIndex((short)v.x, (short)v.y);
        }
        
        public static uint ToIndex(short x, short y)
        {
            unchecked
            {
                var ux = (uint)x << 16;
                var uy = (uint)y;
                return ux | uy;
            }
        }
        
        public static Vector2Int ToVector2Int(this uint v)
        {
            return new Vector2Int((short)(v >> 16), (short)(v & 0x0000ffff));
        }

        public static int Sum(this Vector2Int v)
        {
            return v.x + v.y;
        }

        public static int SumAbs(this Vector2Int v)
        {
            return Mathf.Abs(v.x) + Mathf.Abs(v.y);
        }

        public static Color Evaluate(this Gradient grad)
        {
            return grad.Evaluate(UnityEngine.Random.value);
        }
        
        public static Color Evaluate(this ParticleSystem.MinMaxGradient gradient)
        {
            return gradient.mode switch
            {
                ParticleSystemGradientMode.Color        => gradient.color,
                ParticleSystemGradientMode.Gradient     => gradient.Evaluate(UnityEngine.Random.value),
                ParticleSystemGradientMode.TwoColors    => gradient.Evaluate(UnityEngine.Random.value),
                ParticleSystemGradientMode.TwoGradients => gradient.Evaluate(UnityEngine.Random.value, UnityEngine.Random.value),
                ParticleSystemGradientMode.RandomColor  => gradient.Evaluate(UnityEngine.Random.value, UnityEngine.Random.value),
                _                                       => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static float Evaluate(this ParticleSystem.MinMaxCurve curve)
        {
            return curve.mode switch
            {
                ParticleSystemCurveMode.Constant     => curve.constant,
                ParticleSystemCurveMode.Curve        => curve.Evaluate(UnityEngine.Random.value),
                ParticleSystemCurveMode.TwoCurves    => curve.Evaluate(UnityEngine.Random.value, UnityEngine.Random.value),
                ParticleSystemCurveMode.TwoConstants => curve.Evaluate(0f, UnityEngine.Random.value),
                _                                    => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static Keyframe Last(this AnimationCurve curve)
        {
            return curve.keys[curve.length - 1];
        }
        
        public static Keyframe First(this AnimationCurve curve)
        {
            return curve.keys[0];
        }

        public static float EndTime(this AnimationCurve curve)
        {
            return curve.keys[curve.length - 1].time;
        }

        public static float BeginTime(this AnimationCurve curve)
        {
            return curve.keys[0].time;
        }

        public static float Duration(this AnimationCurve curve)
        {
            return curve.EndTime() - curve.BeginTime();
        }

        public static Color WithA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static Color IncA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, color.a + a);
        }
        public static Color Clamp01(this Color color)
        {
            return new Color(color.r.Clamp01(), color.g.Clamp01(), color.b.Clamp01(), color.a.Clamp01());
        }
        
        public static float AverageRGB(this Color color)
        {
            return (color.r + color.g + color.b) / 3f;
        }
        
        public static Color AddHSV(this Color color, float h, float s, float v)
        {
            Color.RGBToHSV(color, out var cH, out var cS, out var cV);
            var result = Color.HSVToRGB(cH + h, cS + s, cV + v);
            result.a = color.a;
            return result;
        }
        
        public static Vector3 ToVector3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        public static Color MulA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, color.a * a);
        }

        public static Color32 Mul(this Color32 color, float w)
        {
            return new Color32((byte)(color.r * w), (byte)(color.g * w), (byte)(color.b * w), (byte)(color.a * w));
        }
        
        public static double Clamp01(this double d)
        {
            return d.Clamp(0d, 1d);
        }

        public static double Clamp(this double d, double min, double max)
        {
            if (d < min)
                return min;
            if (d > max)
                return max;

            return d;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(this double d)
        {
            return (float)d;
        }
        
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }
        
        public static float ToFloat(this bool b)
        {
            return b ? 1f : 0f;
        }

        public static float Ratio(this float f, float relative)
        {
            return f / relative;
        }

        public static float Ratio01(this float f, float relative)
        {
            return (f / relative).Clamp01();
        }
        
        public static float Clamp01(this float f)
        {
            return Mathf.Clamp01(f);
        }

        public static float ClampPositive(this float f)
        {
            return f.ClampDown(0f);
        }

        public static float ClampNegative(this float f)
        {
            return f.ClampUp(0f);
        }

        public static float ClampAbs(this float f, float magnitude)
        {
            return f > 0f ? Mathf.Clamp(f, 0, magnitude) : Mathf.Clamp(f, -magnitude, 0);
        }
        
        public static float ClampMagnitude(this float f, float magnitude)
        {
            return Mathf.Clamp(f, -magnitude, magnitude);
        }

        public static float OneMinus(this float f)
        {
            return 1f - f;
        }

        public static float Sign(this float f)
        {
            return MathF.Sign(f);
        }
        
        public static float Clamp(this float f, float min, float max)
        {
            return Mathf.Clamp(f, min, max);
        }

        public static float ClampUp(this float f, float max)
        {
            return f > max ? max : f;
        }
        
        public static float ClampDown(this float f, float min)
        {
            return f < min ? min : f;
        }
        
        public static float Max(this float f, float max)
        {
            return f > max ? f : max;
        }

        public static float PositivePart(this float f)
        {
            return f.Max(0f);
        }
        public static float NegativePart(this float f)
        {
            return f.Min(0f);
        }

        public static float Deg2Rad(this float f)
        {
            return f * Mathf.Deg2Rad;
        }
        public static float Rad2Deg(this float f)
        {
            return f * Mathf.Rad2Deg;
        }

        public static float Min(this float f, float min)
        {
            return f < min ? f : min;
        }

        public static float Pow(this float f, float power)
        {
            return Mathf.Pow(f, power);
        }
        
        public static float Mul(this float f, float mul)
        {
            return f * mul;
        }
        
        public static float Add(this float f, float add)
        {
            return f + add;
        }

        public static float Abs(this float f)
        {
            return f < 0 ? -f : f;
        }

        public static float Quantize(this float f, int samples)
        {
            return f - f % (1f / samples);
        }
        
        public static float Round(this float f)
        {
            return Mathf.Round(f);
        }

        public static int RoundToInt(this float f)
        {
            return Mathf.RoundToInt(f);
        }
        
        public static float Floor(this float f)
        {
            return Mathf.Floor(f);
        }

        public static float Frac(this float f)
        {
            return f % 1f;
        }
        public static int FloorToInt(this float f)
        {
            return Mathf.FloorToInt(f);
        }
        
        public static float Half(this float f)
        {
            return f * .5f;
        }
        
        public static float Ceil(this float f)
        {
            return Mathf.Ceil(f);
        }

        public static int CeilToInt(this float f)
        {
            return Mathf.CeilToInt(f);
        }

        public static bool Chance(this float f)
        {
            return UnityRandom.Bool(f);
        }

        public static float Amplitude(this float f)
        {
            var half = f * 0.5f;
            return UnityEngine.Random.Range(-half, half);
        }
        
        public static float Range(this float f)
        {
            return f > 0f ? UnityEngine.Random.Range(0f, f) : UnityEngine.Random.Range(f, 0f);
        }
        
        public static bool TickRate(this ref float f, float tick)
        {
            var result = false;
            if (f <= 0)
            {
                f += tick;
                result = true;
            }
            
            f -= Time.deltaTime;
            return result;
        }
        
        public static float Remap(this float f, float min, float max)
        {
            return min + (max - min) * f;
        }
        
        public static float MoveTowards(this float f, float target, float step)
        {
            return Mathf.MoveTowards(f, target, step);
        }

        public static bool ToBool(this int n)
        {
            return n > 0;
        }
        
        public static int Add(this int f, int inc)
        {
            return f + inc;
        }
        
        public static bool IsEven(this int number)
        {
            return (number & 0x1) == 0x0;
        }

        public static int Range(this int number)
        {
            return UnityEngine.Random.Range(0, number);
        }
        
        public static int Amplitude(this int number)
        {
            var half = number >> 1;
            return UnityEngine.Random.Range(-half, half);
        }
        
        /// <summary> value in range, min max inclusive </summary>
        public static bool InRange(this int number, int min, int max)
        {
            return number >= min && number <= max;
        }
        
        public static bool IsOdd(this int number)
        {
            return (number & 0x1) == 0x1;
        }

        public static int Clamp(this int number, int min, int max)
        {
            return Mathf.Clamp(number, min, max);
        }
        public static int ClampUp(this int number, int max)
        {
            return number > max ? max : number;
        }
        public static int ClampDown(this int number, int min)
        {
            return number < min ? min : number;
        }
        
        public static int Max(this int number, int min)
        {
            return number < min ? min : number;
        }

        public static int Min(this int number, int max)
        {
            return number > max ? max : number;
        }

        public static void Set<T>(this ExposedValue<T> exposedValue, T value)
        {
            if (exposedValue == null)
                return;

            exposedValue.Value = value;
        }

        public static bool IsEqual<T>(this ExposedValue<T> exposedValue, T value)
        {
            if (exposedValue == null)
                return false;

            return Equals(exposedValue.Value, value);
        }

        public static void Play(this IAudioProvider audio)
        {
            audio?.Audio.Play(SoundManager.Instance);
        }

        private static AudioSourceAdapter s_AudioAdapter = new AudioSourceAdapter();
        private class AudioSourceAdapter : IAudioContext
        {
            public AudioSource AudioSource { get; set; }
        }
        
        public static void Play(this AudioSource source, IAudioProvider sound)
        {
            s_AudioAdapter.AudioSource = source;
            sound.Audio.Play(s_AudioAdapter);
        }
        
        public static void Play(this AudioSource source, IAudioProvider sound, float vol)
        {
            s_AudioAdapter.AudioSource = source;
            sound.Audio.Play(s_AudioAdapter, vol);
        }

        private static Dictionary<Type, FieldInfo[]> s_ResolverCache = new Dictionary<Type, FieldInfo[]>(64);
        private static MethodInfo s_ResolveMethod = typeof(RefLinkResolver.IResolvable).GetMethod(nameof(RefLinkResolver.IResolvable.Resolve), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static object[] s_ResolveParams = new object[1];
        
        public static void ResolveRefLinksInParent(this MonoBehaviour obj)
        {
            var resolver = obj.GetComponentInParent<RefLinkResolver>();
            ResolveRefLinks(obj, resolver);
        }
        
        public static void ResolveRefLinks(this MonoBehaviour obj, RefLinkResolver resolver)
        {
            var type = obj.GetType();
            if (s_ResolverCache.TryGetValue(type, out var fields) == false)
            {
                fields = type
                         .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                         .Where(n => typeof(RefLinkResolver.IResolvable).IsAssignableFrom(n.FieldType))
                         .ToArray();
                
                s_ResolverCache.Add(type, fields);
            }
            
            s_ResolveParams[0] = resolver;
            foreach (var fieldInfo in fields)
            {
                s_ResolveMethod.Invoke(fieldInfo.GetValue(obj), s_ResolveParams);
            }
        }
        
        public static SplinePath GetSplinePath(this Spline spline)
        {
            return new SplinePath(new[] { new SplineSlice<Spline>(spline, new SplineRange(0, spline.Count)) });
        }
        
        public static float GetKnotLenght(this Spline spline, int knot)
        {
            var sum = 0f;
            for (var n = 0; n < knot; n++)
                sum += spline.GetCurveLength(n);
            
            return sum;
        }
        
        public static int IndexOf(this SplineContainer splineCont, Spline spline)
        {
            var splines = splineCont.Splines;
            for (var n = 0; n < splines.Count; n++)
            {
                if (splines[n] == spline)
                    return n;
            }
            
            return -1;
        }
        
        public static float GetKnotTime(this Spline spline, int knot)
        {
            return spline.GetKnotLenght(knot) / spline.GetLength();
        }
        
        public static int GetClosestKnot(this Spline spline, float t)
        {
            var length = spline.GetLength() * t;
            for (var knotIndex = 0; knotIndex < spline.Count; knotIndex++)
            {
                var curveLight = spline.GetCurveLength(knotIndex);
                if (length - curveLight < 0)
                    return knotIndex + (length / curveLight > .5f ? 1 : 0); 
                        
                length -= curveLight;
            }
            
            return 0;
        }
        
        public static Vector3 GetClosestPos(this SplineContainer splineCont, Vector3 pos, out Spline spline, out float time)
        {
            spline = null;
            time   = 0f;
            
            var minDist = float.MaxValue;
            var closest = float3.zero;
            
            for (var n = 0; n < splineCont.Splines.Count; n++)
            {
                using var native = new NativeSpline(splineCont.Splines[n], splineCont.transform.localToWorldMatrix);
                var       dist   = SplineUtility.GetNearestPoint(native, pos, out var point, out var t);

                if (dist <= minDist)
                {
                    minDist = dist;
                    closest = point;
                    time    = t;
                    spline  = splineCont.Splines[n];
                }
            }
            
            return closest;
        }
        
        public static SplineKnotIndex GetClosestKnot(this SplineContainer splineCont, Vector3 pos)
        {
            var minDist = float.MaxValue;
            var time = 0f;
            var splineIndex = 0;
            var knotIndex = 0;
            
            for (var n = 0; n < splineCont.Splines.Count; n++)
            {
                var       spline = splineCont.Splines[n];
                using var native = new NativeSpline(spline, splineCont.transform.localToWorldMatrix);
                var       dist   = SplineUtility.GetNearestPoint(native, pos, out _, out var t);

                if (dist <= minDist)
                {
                    minDist = dist;
                    splineIndex = n;
                    time = t;
                }
            }
            
            knotIndex = splineCont.Splines[splineIndex].GetClosestKnot(time);

            return new SplineKnotIndex(splineIndex, knotIndex);
        }
        
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static bool Approximately(float a, float b, float tolerance = 1e-5f)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        public static float CrossProduct2D(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        public static bool Intersect(this Line a, Line b, out Vector2 hit)
        {
            return Intersect(a.start, a.end, b.start, b.end, out hit);
        }
        /// <summary>
        /// Determine whether 2 lines intersect, and give the intersection point if so.
        /// </summary>
        /// <param name="p1start">Start point of the first line</param>
        /// <param name="p1end">End point of the first line</param>
        /// <param name="p2start">Start point of the second line</param>
        /// <param name="p2end">End point of the second line</param>
        /// <param name="hit">If there is an intersection, this will be populated with the point</param>
        /// <returns>True if the lines intersect, false otherwise.</returns>
        public static bool Intersect(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end, out Vector2 hit)
        {
            // Consider:
            //   p1start = p
            //   p1end = p + r
            //   p2start = q
            //   p2end = q + s
            // We want to find the intersection point where :
            //  p + t*r == q + u*s
            // So we need to solve for t and u
            var   p        = p1start;
            var   r        = p1end - p1start;
            var   q        = p2start;
            var   s        = p2end - p2start;
            var   qminusp  = q - p;
            var cross_rs = CrossProduct2D(r, s);
            if (Approximately(cross_rs, 0f))
            {
                // Parallel lines
                if (Approximately(CrossProduct2D(qminusp, r), 0f))
                {
                    // Co-linear lines, could overlap
                    var rdotr = Vector2.Dot(r, r);
                    var sdotr = Vector2.Dot(s, r);
                    // this means lines are co-linear
                    // they may or may not be overlapping
                    var t0 = Vector2.Dot(qminusp, r / rdotr);
                    var t1 = t0 + sdotr / rdotr;
                    if (sdotr < 0)
                    {
                        // lines were facing in different directions so t1 > t0, swap to simplify check
                        Swap(ref t0, ref t1);
                    }

                    if (t0 <= 1 && t1 >= 0)
                    {
                        // Nice half-way point intersection
                        var t = Mathf.Lerp(Mathf.Max(0, t0), Mathf.Min(1, t1), 0.5f);
                        hit = p + t * r;
                        return true;
                    }
                    else
                    {
                        // Co-linear but disjoint
                        hit = Vector2.zero;
                        return false;
                    }
                }
                else
                {
                    // Just parallel in different places, cannot intersect
                    hit = Vector2.zero;
                    return false;
                }
            }
            else
            {
                // Not parallel, calculate t and u
                var t = CrossProduct2D(qminusp, s) / cross_rs;
                var u = CrossProduct2D(qminusp, r) / cross_rs;
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    hit = p + t * r;
                    return true;
                }
                else
                {
                    // Lines only cross outside segment range
                    hit = Vector2.zero;
                    return false;
                }
            }
        }
    }

    public static class Actions
    {
        public static void Empty() { }
        public static void Empty<T>(T value) { }
        public static void Empty<T1, T2>(T1 value1, T2 value2) { }
    }

    public static class Functions
    {
        public static T Identity<T>(T value) { return value; }

        public static T Default<T>() { return default; }

        public static bool IsNull<T>(T entity) where T : class { return entity == null; }
        public static bool IsNonNull<T>(T entity) where T : class { return entity != null; }

        public static bool True<T>(T entity) { return true; }
        public static bool False<T>(T entity) { return false; }
    }
}