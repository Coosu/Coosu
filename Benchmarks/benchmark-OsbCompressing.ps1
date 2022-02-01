dotnet build OsbCompressingBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process .\OsbCompressingBenchmark\bin\Release\net6.0\OsbCompressingBenchmark.exe -WorkingDirectory .\OsbCompressingBenchmark\bin\Release\net6.0 -NoNewWindow -Wait
}