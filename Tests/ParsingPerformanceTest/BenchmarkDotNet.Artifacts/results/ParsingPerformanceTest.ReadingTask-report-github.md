``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
Intel Core i7-4770K CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]               : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0             : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET Framework 4.7.2 : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT


```
|     Method |                  Job |              Runtime |     Mean |     Error |    StdDev |    Gen 0 |    Gen 1 |   Gen 2 | Allocated |
|----------- |--------------------- |--------------------- |---------:|----------:|----------:|---------:|---------:|--------:|----------:|
| LocalCoosu |             .NET 6.0 |             .NET 6.0 | 3.220 ms | 0.0622 ms | 0.0830 ms | 152.3438 |  70.3125 |       - |    815 KB |
| LocalCoosu | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 4.886 ms | 0.0614 ms | 0.0574 ms | 406.2500 | 164.0625 |       - |  2,150 KB |
| NugetCoosu |             .NET 6.0 |             .NET 6.0 | 6.040 ms | 0.1205 ms | 0.2932 ms | 476.5625 | 226.5625 |  7.8125 |  2,507 KB |
| NugetCoosu | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 8.988 ms | 0.0894 ms | 0.0793 ms | 906.2500 | 234.3750 | 62.5000 |  4,339 KB |
