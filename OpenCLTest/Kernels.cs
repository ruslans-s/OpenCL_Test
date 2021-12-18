using Amplifier.OpenCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCLTest
{
    internal class Kernels : OpenCLFunctions
    {
        [OpenCLKernel]
        void MatMul([Global] float[] A, [Global] float[] B, [Global] float[] C, int M)
        {
            int x = get_global_id(0);
            int i = x / M;
            int j = x % M;

            float acc = 0.0f;
            for (int k = 0; k < M; k++)
            {
                acc += A[k + M * i] * B[k * M + j];
                //  acc += A[k * M + globalRow] * B[globalCol * K + k];
            }

            C[i * M + j] = acc;
        }

        [OpenCLKernel]
        void RSmulty([Global] float[] x, [Global] float[] y, float value)
        {
            int i = get_global_id(0);
            for (int j = 0; j < value; j++)
            {
                x[i] = x[i] + y[j];
            }
        }

        [OpenCLKernel]
        void Fill([Global] float[] x, float value)
        {
            int i = get_global_id(0);

            x[i] = value;
        }

        [OpenCLKernel]
        void FillMatrx(int value, int value2,  [Global] float[] x)
        {
            int i = get_global_id(0);
            int y = i % value;

            x[i] = y;
        }

    }
}
