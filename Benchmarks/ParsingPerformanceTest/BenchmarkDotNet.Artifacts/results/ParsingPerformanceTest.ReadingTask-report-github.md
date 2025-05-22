```

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.5335/23H2/2023Update/SunValley3)
AMD Ryzen 7 6800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.300
  [Host]             : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  .NET 8.0           : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9300.0), X64 RyuJIT VectorSize=256


```
| Method     | Job                | Runtime            | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Gen2    | Allocated  | Alloc Ratio |
|----------- |------------------- |------------------- |----------:|----------:|----------:|------:|--------:|----------:|---------:|--------:|-----------:|------------:|
| Coosu      | .NET 8.0           | .NET 8.0           |  1.250 ms | 0.0215 ms | 0.0191 ms |  1.00 |    0.00 |   62.5000 |  39.0625 |       - |  555.55 KB |        1.00 |
| OsuParsers | .NET 8.0           | .NET 8.0           |  4.318 ms | 0.0855 ms | 0.1227 ms |  3.52 |    0.12 |  515.6250 | 375.0000 |       - | 4255.12 KB |        7.66 |
|            |                    |                    |           |           |           |       |         |           |          |         |            |             |
| Coosu      | .NET Framework 4.8 | .NET Framework 4.8 |  3.520 ms | 0.0678 ms | 0.0781 ms |  1.00 |    0.00 |   97.6563 |  46.8750 |       - |  614.53 KB |        1.00 |
| OsuParsers | .NET Framework 4.8 | .NET Framework 4.8 | 46.319 ms | 0.5352 ms | 0.4744 ms | 13.24 |    0.27 | 1545.4545 | 545.4545 | 90.9091 |  9612.8 KB |       15.64 |
