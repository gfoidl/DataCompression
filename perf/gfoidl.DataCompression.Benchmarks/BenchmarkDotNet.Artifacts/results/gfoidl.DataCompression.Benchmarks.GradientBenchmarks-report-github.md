``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Core i7-3610QM CPU 2.30GHz (Ivy Bridge), ProcessorCount=8
Frequency=2241064 Hz, Resolution=446.2166 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
| Method |      Mean |     Error |    StdDev | Scaled | ScaledSD |
|------- |----------:|----------:|----------:|-------:|---------:|
| Normal |  2.307 ns | 0.0088 ns | 0.0082 ns |   1.00 |     0.00 |
|  Simd1 | 13.771 ns | 0.0411 ns | 0.0364 ns |   5.97 |     0.03 |
|  Simd2 | 15.372 ns | 0.0195 ns | 0.0183 ns |   6.66 |     0.02 |
