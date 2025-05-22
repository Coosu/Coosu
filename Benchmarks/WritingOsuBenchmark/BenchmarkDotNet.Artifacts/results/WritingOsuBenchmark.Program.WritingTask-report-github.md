```

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.5335/23H2/2023Update/SunValley3)
AMD Ryzen 7 6800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.300
  [Host]             : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  .NET 8.0           : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9300.0), X64 RyuJIT VectorSize=256


```
| Method     | Job                | Runtime            | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Allocated   | Alloc Ratio |
|----------- |------------------- |------------------- |-----------:|----------:|----------:|------:|--------:|----------:|---------:|------------:|------------:|
| Coosu      | .NET 8.0           | .NET 8.0           |   2.377 ms | 0.0261 ms | 0.0218 ms |  1.00 |    0.00 |   70.3125 |        - |   579.64 KB |        1.00 |
| JsonDotNet | .NET 8.0           | .NET 8.0           |   8.564 ms | 0.1682 ms | 0.1573 ms |  3.61 |    0.08 |  312.5000 |        - |  2598.81 KB |        4.48 |
| OsuParsers | .NET 8.0           | .NET 8.0           | 163.567 ms | 2.3641 ms | 2.2114 ms | 68.79 |    0.96 | 1333.3333 | 333.3333 | 13210.64 KB |       22.79 |
|            |                    |                    |            |           |           |       |         |           |          |             |             |
| Coosu      | .NET Framework 4.8 | .NET Framework 4.8 |   5.692 ms | 0.1133 ms | 0.1212 ms |  1.00 |    0.00 |  195.3125 |        - |  1225.48 KB |        1.00 |
| OsuParsers | .NET Framework 4.8 | .NET Framework 4.8 |  11.000 ms | 0.1697 ms | 0.1588 ms |  1.93 |    0.05 | 2515.6250 | 734.3750 | 15497.97 KB |       12.65 |
| JsonDotNet | .NET Framework 4.8 | .NET Framework 4.8 |  18.181 ms | 0.3568 ms | 0.5862 ms |  3.17 |    0.13 |  437.5000 |        - |  2751.28 KB |        2.25 |
