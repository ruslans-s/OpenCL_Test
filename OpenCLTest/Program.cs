﻿using OpenCLTest;

Benchmark benchmark = new Benchmark();
benchmark.Initialization();

string topTable = "|Size\t|CPU time\t|GPU time\t|Errors\t|ratio\t|";
Console.WriteLine(topTable);

int startSize = 32;
benchmark.Start(startSize);
for (int i = 1; i < 512; i*=2)
{
    benchmark.Start(i * startSize);
}

Console.WriteLine("End");
