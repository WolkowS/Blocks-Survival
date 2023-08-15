using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render.RenderFeature
{
    public static class Utils
    {
        public static int s_MainTexId = Shader.PropertyToID("_MainTex");
        
        private static Mesh s_FullscreenQuad;
        private static Mesh s_FullscreenTriangle;
        public static  Mesh FullscreenMesh
        {
            get
            {
                _initFullScreenMeshes();
                return s_FullscreenTriangle;
            }
        }
        
        private static Matrix4x4 s_IndentityInvert = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, -1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1));

        // =======================================================================
        private static void _initFullScreenMeshes()
        {
            // quad
            if (s_FullscreenQuad == null)
            {
                s_FullscreenQuad = new Mesh { name = "Fullscreen Quad" };
                s_FullscreenQuad.SetVertices(new List<Vector3>
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3(-1.0f, 1.0f, 0.0f),
                    new Vector3(1.0f, -1.0f, 0.0f),
                    new Vector3(1.0f, 1.0f, 0.0f)
                });

                s_FullscreenQuad.SetUVs(0, new List<Vector2>
                {
                    new Vector2(0.0f, 1f),
                    new Vector2(0.0f, 0f),
                    new Vector2(1.0f, 1f),
                    new Vector2(1.0f, 0f)
                });

                s_FullscreenQuad.SetIndices(new[] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, false);
                s_FullscreenQuad.UploadMeshData(true);
            }
            
            // triangle
            if (s_FullscreenTriangle == null)
            { 
                s_FullscreenTriangle           = new Mesh() { name = "Fullscreen Triangle" };
                s_FullscreenTriangle.vertices  = _verts(0f);
                s_FullscreenTriangle.uv        = _texCoords();
                s_FullscreenTriangle.triangles = new int[3] { 0, 1, 2 };

                s_FullscreenTriangle.UploadMeshData(true);

                // -----------------------------------------------------------------------
                Vector3[] _verts(float z)
                {
                    var r = new Vector3[3];
                    for (var i = 0; i < 3; i++)
                    {
                        var uv = new Vector2((i << 1) & 2, i & 2);
                        r[i] = new Vector3(uv.x * 2f - 1f, uv.y * 2f - 1f, z);
                    }

                    return r;
                }

                Vector2[] _texCoords()
                {
                    var r = new Vector2[3];
                    for (var i = 0; i < 3; i++)
                    {
                        if (SystemInfo.graphicsUVStartsAtTop)
                            r[i] = new Vector2((i << 1) & 2, 1.0f - (i & 2));
                        else
                            r[i] = new Vector2((i << 1) & 2, i & 2);
                    }

                    return r;
                }
            }
        }
        
        public static void Blit(CommandBuffer cmd, RTHandle source, RTHandle destination, Material material, int pass = 0, bool invert = false)
        {
            cmd.SetGlobalTexture(s_MainTexId, source);
            cmd.SetRenderTarget(destination);
            cmd.DrawMesh(FullscreenMesh, invert ? s_IndentityInvert : Matrix4x4.identity, material, 0, pass);
        }
    }
}