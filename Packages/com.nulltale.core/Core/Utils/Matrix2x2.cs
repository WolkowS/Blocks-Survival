using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class Matrix2x2
    {
        public float    m00
        {
            get => m_Matrix[0, 0];
            set => m_Matrix[0, 0] = value;
        }

        public float    m01
        {
            get => m_Matrix[1, 0];
            set => m_Matrix[1, 0] = value;
        }

        public float    m10
        {
            get => m_Matrix[0, 1];
            set => m_Matrix[0, 1] = value;
        }

        public float    m11
        {
            get => m_Matrix[1, 1];
            set => m_Matrix[1, 1] = value;
        }

        public float[,] m_Matrix = new float[2, 2];

        public static float MATRIX_EPSILON = 1e-6f;

        // =======================================================================
        public Matrix2x2()
        {
            SetValue(1, 0, 0, 1);
        }

        public Matrix2x2(float m00, float m01, float m10, float m11)
        {
            SetValue(m00, m01, m10, m11);
        }

        public Matrix2x2(Matrix2x2 m)
        {
            SetValue(m[0, 0], m[0, 1], m[1, 0], m[1, 1]);
        }

        public Matrix2x2(float m00, float m11)
        {
            // Diagonal
            SetValue(m00, 0, 0, m11);
        }

        public void LoadIdentity()
        {
            SetValue(1, 0, 0, 1);
        }

        public void SetValue(float m00, float m01, float m10, float m11)
        {
            this[0, 0] = m00;
            this[0, 1] = m01;
            this[1, 0] = m10;
            this[1, 1] = m11;
        }

        public void SetValue(float m00, float m11)
        {
            SetValue(m00, 0, 0, m11);
        }

        public void SetValue(in Matrix2x2 m)
        {
            SetValue(m[0, 0], m[0, 1], m[1, 0], m[1, 1]);
        }

        public void SetValue(float value)
        {
            SetValue(value, value, value, value);
        }

        public void Normalize()
        {
            for (int row = 0; row < 2; row++)
            {
                float l = 0;
                for (int column = 0; column < 2; column++)
                {
                    l += this[row, column] * this[row, column];
                }

                l = Mathf.Sqrt(l);

                for (int column = 0; column < 2; column++)
                {
                    this[row, column] /= l;
                }
            }
        }

        public float Determinant()
        {
            return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
        }

        public Matrix2x2 Transpose()
        {
            return new Matrix2x2(this[0, 0], this[1, 0], this[0, 1], this[1, 1]);
        }

        public Matrix2x2 Inverse()
        {
            float det = Determinant();
            return new Matrix2x2(this[1, 1] / det, -this[0, 1] / det, -this[1, 0] / det, this[0, 0] / det);
        }

        public Matrix2x2 Cofactor()
        {
            return new Matrix2x2(this[1, 1], -this[1, 0], -this[0, 1], this[0, 0]);
        }

        public float FrobeniusInnerProduct(in Matrix2x2 m)
        {
            float prod = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    prod += this[i, j] * m[i, j];
                }
            }

            return prod;
        }

        /// <summary>
        /// Singular Value Decomposition
        /// </summary>
        /// <param name="w">Returns rotation matrix</param>
        /// <param name="e">Returns sigma matrix</param>
        /// <param name="v">Returns (not transposed)</param>
        public void SVD(ref Matrix2x2 w, ref Matrix2x2 e, ref Matrix2x2 v)
        {
            // If it is diagonal, SVD is trivial
            if (Mathf.Abs(this[1, 0] - this[0, 1]) < MATRIX_EPSILON && Mathf.Abs(this[1, 0]) < MATRIX_EPSILON)
            {
                w.SetValue(this[0, 0] < 0 ? -1 : 1, 0, 0, this[1, 1] < 0 ? -1 : 1);
                e.SetValue(Mathf.Abs(this[0, 0]), Mathf.Abs(this[1, 1]));
                v.LoadIdentity();
            }

            // Otherwise, we need to compute A^T*A
            else
            {
                float j   = this[0, 0] * this[0, 0] + this[1, 0] * this[1, 0],
                      k   = this[0, 1] * this[0, 1] + this[1, 1] * this[1, 1],
                      v_c = this[0, 0] * this[0, 1] + this[1, 0] * this[1, 1];
                // Check to see if A^T*A is diagonal
                if (Mathf.Abs(v_c) < MATRIX_EPSILON)
                {
                    float s1 = Mathf.Sqrt(j), s2 = Mathf.Abs(j - k) < MATRIX_EPSILON ? s1 : Mathf.Sqrt(k);
                    e.SetValue(s1, s2);
                    v.LoadIdentity();
                    w.SetValue(this[0, 0] / s1, this[0, 1] / s2, this[1, 0] / s1, this[1, 1] / s2);
                }
                // Otherwise, solve quadratic for eigenvalues
                else
                {
                    float jmk  = j - k,
                          jpk  = j + k,
                          root = Mathf.Sqrt(jmk * jmk + 4 * v_c * v_c),
                          eig  = (jpk + root) / 2,
                          s1   = Mathf.Sqrt(eig),
                          s2   = Mathf.Abs(root) < MATRIX_EPSILON ? s1 : Mathf.Sqrt((jpk - root) / 2);

                    e.SetValue(s1, s2);

                    // Use eigenvectors of A^T*A as V
                    float v_s = eig - j, len = Mathf.Sqrt(v_s * v_s + v_c * v_c);
                    v_c /= len;
                    v_s /= len;
                    v.SetValue(v_c, -v_s, v_s, v_c);
                    // Compute w matrix as Av/s
                    w.SetValue(
                        (this[0, 0] * v_c + this[0, 1] * v_s) / s1,
                        (this[0, 1] * v_c - this[0, 0] * v_s) / s2,
                        (this[1, 0] * v_c + this[1, 1] * v_s) / s1,
                        (this[1, 1] * v_c - this[1, 0] * v_s) / s2
                    );
                }
            }
        }

        //DIAGONAL MATRIX OPERATIONS
        //Matrix * Matrix
        public void DiagProduct(Vector2 v)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                    this[i, j] *= v[i];
            }
        }

        //Matrix * Matrix^-1
        public void DiagProductInv(Vector2 v)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                    this[i, j] /= v[i];
            }
        }

        //Matrix - Matrix
        public void DiagDifference(float c)
        {
            for (int i = 0; i < 2; i++)
                this[i, i] -= c;
        }

        public void DiagDifference(Vector2 v)
        {
            for (int i = 0; i < 2; i++)
                this[i, i] -= v[i];
        }

        //Matrix + Matrix
        public void DiagSum(float c)
        {
            for (int i = 0; i < 2; i++)
                this[i, i] += c;
        }

        public void DiagSum(Vector2 v)
        {
            for (int i = 0; i < 2; i++)
                this[i, i] += v[i];
        }

        // Array subscripts
        public float this[int row, int column]
        {
            get => m_Matrix[column, row];
            set => m_Matrix[column, row] = value;
        }

        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => m00,
                    1 => m01,
                    2 => m10,
                    3 => m11,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m01 = value;
                        break;
                    case 2:
                        m10 = value;
                        break;
                    case 3:
                        m11 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // Matrix - Scalar overloads
        public static Matrix2x2 operator +(Matrix2x2 l, float r)
        {
            Matrix2x2 result = new Matrix2x2(l);
            for (int index = 0; index < 4; index++)
            {
                result[index] += r;
            }

            return result;
        }

        public static Matrix2x2 operator +(float l, Matrix2x2 r)
        {
            Matrix2x2 result = new Matrix2x2(r);
            for (int index = 0; index < 4; index++)
            {
                result[index] += l;
            }

            return result;
        }

        public static Matrix2x2 operator -(Matrix2x2 l, float r)
        {
            Matrix2x2 result = new Matrix2x2(l);
            for (int index = 0; index < 4; index++)
            {
                result[index] -= r;
            }

            return result;
        }

        public static Matrix2x2 operator *(Matrix2x2 l, float r)
        {
            Matrix2x2 result = new Matrix2x2(l);
            for (int index = 0; index < 4; index++)
            {
                result[index] *= r;
            }

            return result;
        }

        public static Matrix2x2 operator *(float l, Matrix2x2 r)
        {
            Matrix2x2 result = new Matrix2x2(r);
            for (int index = 0; index < 4; index++)
            {
                result[index] *= l;
            }

            return result;
        }
        
        public static Vector2 operator *(Vector2 point, Matrix2x2 r)
        {
            return new Vector2(point.x * r.m00 + point.y * r.m01, point.x * r.m10 + point.y * r.m11);
        }

        public static Matrix2x2 operator /(Matrix2x2 l, float r)
        {
            Matrix2x2 result = new Matrix2x2(l);
            for (int index = 0; index < 4; index++)
            {
                result[index] /= r;
            }

            return result;
        }

        // Matrix - Matrix overloads
        public static Matrix2x2 operator +(Matrix2x2 l, Matrix2x2 r)
        {
            Matrix2x2 result = new Matrix2x2(l);
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 2; column++)
                {
                    result[row, column] += r[row, column];
                }
            }

            return result;
        }

        public static Matrix2x2 operator -(Matrix2x2 l, Matrix2x2 r)
        {
            Matrix2x2 result = new Matrix2x2(l);
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 2; column++)
                {
                    result[row, column] -= r[row, column];
                }
            }

            return result;
        }

        public static Matrix2x2 operator *(Matrix2x2 l, Matrix2x2 r)
        {
            Matrix2x2 result = new Matrix2x2(l);
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 2; column++)
                {
                    result[row, column] = l[row, 0] * r[0, column];
                    for (int i = 1; i < 2; i++)
                    {
                        result[row, column] += l[row, i] * r[i, column];
                    }
                }
            }

            return result;
        }

        // Matrix - Vector Overloads
        public static Vector2 operator *(Matrix2x2 l, Vector2 r)
        {
            return new Vector2(
                l[0, 0] * r[0] + l[0, 1] * r[1],
                l[1, 0] * r[0] + l[1, 1] * r[1]
            );
        }

        public override string ToString()
        {
            string str = "[\n";
            for (int row = 0; row < 2; row++)
            {
                str += "[";
                for (int column = 0; column < 2; column++)
                {
                    str += this[row, column] + ", ";
                }

                str += "]\n";
            }

            str += "]";

            return str;
        }

        public static Matrix2x2 Identity { get; } = new Matrix2x2(1, 0, 0, 1);

        public static Matrix2x2 Rotation(float degree)
        {
            var rad = degree * Mathf.Deg2Rad;
            var sin = Mathf.Sin(rad);
            var cos = Mathf.Cos(rad);
            return new Matrix2x2(cos, -sin, sin, cos);
        }

        public static Matrix2x2 Scale(float scale)
        {
            return new Matrix2x2(scale, 0, 0, scale);
        }
        
        public static Matrix2x2 Scale(Vector2 scale)
        {
            return new Matrix2x2(scale.x, 0, 0, scale.y);
        }

        public static Matrix2x2 RotationScale(float degree, Vector2 scale)
        {
            var rad = degree * Mathf.Deg2Rad;
            var sin = Mathf.Sin(rad);
            var cos = Mathf.Cos(rad);
            return new Matrix2x2(cos * scale.y, -sin * scale.y, sin * scale.x, cos * scale.x);
        }
    }
}