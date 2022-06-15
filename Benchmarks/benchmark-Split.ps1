dotnet build SplitBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process .\SplitBenchmark\bin\Release\net6.0\SplitBenchmark.exe -WorkingDirectory .\SplitBenchmark\bin\Release\net6.0 -NoNewWindow -Wait
}