using Amplifier;
using System.Diagnostics;
using System.Management;

namespace OpenCLTest
{
    internal class Benchmark
    {
        private OpenCLCompiler compiler;
        private dynamic exec;

        float[,] x3;
        float[,] y3;
        float[,] r3;
        //Для CPu
        float[,] x4;
        float[,] y4;
        float[,] r4;

        float[] x3_temp ;
        float[] y3_temp ;
        float[] r3_temp ;
        int maxFloatSize;
        public void Start(float size)
        {

            int intSize = (int)size;

            //Создаем массивы
            //Для GPU
            if(x3 != null)
            {
                Array.Clear(x3);
                Array.Clear(y3);
                Array.Clear(r3);

                Array.Clear(x4);
                Array.Clear(y4);
                Array.Clear(r4);

                Array.Clear(x3_temp);
                Array.Clear(y3_temp);
                Array.Clear(r3_temp);
            } 
            else
            {
                x3 = new float[maxFloatSize, maxFloatSize];
                y3 = new float[maxFloatSize, maxFloatSize];
                r3 = new float[maxFloatSize, maxFloatSize];
                r4 = new float[maxFloatSize, maxFloatSize];

                //Временные массивы 
                x3_temp = new float[maxFloatSize * maxFloatSize];
                y3_temp = new float[maxFloatSize * maxFloatSize];
                r3_temp = new float[maxFloatSize * maxFloatSize];
            }

        

            //Создаем рандом
            Random rnd = new Random();

            //Заполнение массивов
            for (int i = 0; i < intSize; i++)
            {
                for (int j = 0; j < intSize; j++)
                {
                    x3[i, j] = (float)rnd.NextDouble();
                    y3[i, j] = (float)rnd.NextDouble();
                }
            }

            //Клонируем массивы для CPU
            x4 = (float[,])x3.Clone();
            y4 = (float[,])y3.Clone();

            //Запуск таймера для GPU
            Stopwatch stopwatch = new Stopwatch();  
            stopwatch.Start();
          
         

            //Переводим массивы из 2D в 1D
            int count = 0;
            for (int i = 0; i < intSize; i++)
            {
                for (int j = 0; j < intSize; j++)
                {
                    x3_temp[count] = x3[i, j];
                    y3_temp[count] = y3[i, j];
                    count++;
                }
            }

            //Вычисление
            exec.MatMul(x3_temp, y3_temp, r3_temp, intSize);

            //Перевод из 1D в 2D
            count = 0;
            for (int i = 0; i < intSize; i++)
            {
                for (int j = 0; j < intSize; j++)
                {
                    r3[i, j] = r3_temp[count];
                    count++;
                }
            }

            //Конец вычисленй и стоп таймеру
            stopwatch.Stop();
            
            //Начало вычислений на CPU
            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            
            /*
            //Вычисления
            for (int i = 0; i < intSize; i++)
            {
                for (int j = 0; j < intSize; j++)
                {
                    float s = 0.0f;

                    float[] temps = new float[intSize];
                    for(int k = 0; k < intSize; k++)
                        temps[k] = y3[k, j];

                    for (int k = 0; k < intSize; k++)
                        s += x4[i, k] * temps[k];

                    r4[i, j] = s;
                }
            }*/
        
            stopwatch2.Stop();
        
            //Расчет ошибки
            int errors = 0;
            count = 0;
            for (int i = 0; i < intSize; i++)
            {
                for (int j = 0; j < intSize; j++)
                {
                    if ((r3[i, j] - r4[i, j]) > 0.001)
                    {
                        errors++;
                       
                    }

                }
            }

            
            //Вывод
            float ratio = (float)stopwatch2.ElapsedMilliseconds / (float)stopwatch.ElapsedMilliseconds;
            Console.WriteLine("|{0}\t|{1}\t\t|{2}\t\t|{3}\t|{4}", size, stopwatch2.ElapsedMilliseconds, stopwatch.ElapsedMilliseconds, errors, ratio);

        }

        public void Initialization(int maxFloatSize2)
        {
            maxFloatSize = maxFloatSize2;

            compiler = new OpenCLCompiler();
            Console.WriteLine("\nList Devices----");

            string CPU = GetHardwareInfo("Win32_Processor", "Name")[0];
            Console.WriteLine("CPU: " + CPU);
            Console.WriteLine("OpenCL supported device:");
            foreach (var item in compiler.Devices)
            {
                string GPU = GetHardwareInfo("Win32_VideoController", "Name")[item.ID];
                Console.WriteLine("|"+item+"\t|"+ GPU);
            }

            compiler.UseDevice(0);
            compiler.CompileKernel(typeof(Kernels));



            exec = compiler.GetExec();
        }

        static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    result.Add(obj[ClassItemField].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
