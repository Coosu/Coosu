# Coosu.Storyboard
Use Coosu.Storyboard (osu!storyboard coding Library) to code and generate SBC(.osb code)

Support using in StoryBrew and use ExecuteBrew().

## Performance improvement

Since version v2.1.1, the library's performance was optimized. 
### Parsing benchmark
Run `benchmark-OsbParsing.ps1`:

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
Intel Core i7-4770K CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]             : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0           : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET Core 3.1      : .NET Core 3.1.22 (CoreCLR 4.700.21.56803, CoreFX 4.700.21.57101), X64 RyuJIT
  .NET Framework 4.8 : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT


```
|      Method |       Runtime |      Mean |     Error |    StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 | Allocated |
|------------ |-------------- |----------:|----------:|----------:|------:|--------:|---------:|---------:|----------:|
| CoosuLatest |      .NET 6.0 |  1.771 ms | 0.0105 ms | 0.0099 ms |  1.00 |    0.00 |  87.8906 |  42.9688 |    544 KB |
|    CoosuOld |      .NET 6.0 |  8.215 ms | 0.0653 ms | 0.0611 ms |  4.64 |    0.05 | 265.6250 | 125.0000 |  1,626 KB |
|  OsuParsers |      .NET 6.0 | 11.893 ms | 0.1751 ms | 0.1638 ms |  6.72 |    0.10 | 203.1250 |  93.7500 |  1,258 KB |
|             |               |           |           |           |       |         |          |          |           |
| CoosuLatest | .NET Core 3.1 |  2.023 ms | 0.0162 ms | 0.0152 ms |  1.00 |    0.00 |  89.8438 |  42.9688 |    543 KB |
|  OsuParsers | .NET Core 3.1 |  3.388 ms | 0.0267 ms | 0.0250 ms |  1.67 |    0.01 | 207.0313 | 101.5625 |  1,258 KB |
|    CoosuOld | .NET Core 3.1 |  3.697 ms | 0.0736 ms | 0.0723 ms |  1.83 |    0.04 | 265.6250 | 132.8125 |  1,626 KB |
|             |               |           |           |           |       |         |          |          |           |
| CoosuLatest |    .NETFX 4.8 |  3.392 ms | 0.0309 ms | 0.0289 ms |  1.00 |    0.00 | 152.3438 |  74.2188 |    922 KB |
|  OsuParsers |    .NETFX 4.8 |  4.584 ms | 0.0669 ms | 0.0625 ms |  1.35 |    0.02 | 273.4375 | 132.8125 |  1,669 KB |
|    CoosuOld |    .NETFX 4.8 |  4.935 ms | 0.0769 ms | 0.0719 ms |  1.45 |    0.02 | 343.7500 | 171.8750 |  2,102 KB |
