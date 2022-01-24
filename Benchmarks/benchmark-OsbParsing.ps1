dotnet build OsbParsingBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process .\OsbParsingBenchmark\bin\Release\net6.0\OsbParsingBenchmark.exe -WorkingDirectory .\OsbParsingBenchmark\bin\Release\net6.0 -NoNewWindow -Wait
}