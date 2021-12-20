using OpenCLTest;

Console.WriteLine("Start");
Benchmark benchmark = new Benchmark();

int startSize = 32;
int maxFloatSize = 4096;

benchmark.Initialization(maxFloatSize);

string topTable = "|Size\t|CPU time\t|GPU time\t|Errors\t|ratio\t|";
Console.WriteLine(topTable);


benchmark.Start(startSize);
for (int i = startSize; i < maxFloatSize; i*=2)
{
    
    benchmark.Start(i);
}

Console.WriteLine("End");

Console.ReadKey();
Console.ReadKey();
