``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Core i7-3610QM CPU 2.30GHz (Ivy Bridge), ProcessorCount=8
Frequency=2241064 Hz, Resolution=446.2166 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|      Method |     N |      Mean |      Error |     StdDev |    Median | Scaled | ScaledSD |
|------------ |------ |----------:|-----------:|-----------:|----------:|-------:|---------:|
| **IEnumerable** |  **1000** |  **61.52 us** |  **0.5802 us** |  **0.5143 us** |  **61.25 us** |   **1.00** |     **0.00** |
|        List |  1000 |  53.71 us |  1.2556 us |  1.1745 us |  53.05 us |   0.87 |     0.02 |
|       Array |  1000 |  46.42 us |  0.8306 us |  0.7770 us |  46.45 us |   0.75 |     0.01 |
| **IEnumerable** | **10000** | **631.51 us** |  **2.0497 us** |  **1.7116 us** | **630.91 us** |   **1.00** |     **0.00** |
|        List | 10000 | 538.04 us |  0.5200 us |  0.4342 us | 538.05 us |   0.85 |     0.00 |
|       Array | 10000 | 486.82 us | 39.3617 us | 73.9308 us | 447.74 us |   0.77 |     0.12 |
