dotnet build WritingOsuBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process .\WritingOsuBenchmark\bin\Release\net6.0\WritingOsuBenchmark.exe -WorkingDirectory .\WritingOsuBenchmark\bin\Release\net6.0 -NoNewWindow -Wait
}