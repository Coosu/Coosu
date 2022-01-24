dotnet build AnalyzeTypeBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process .\AnalyzeTypeBenchmark\bin\Release\net6.0\AnalyzeTypeBenchmark.exe -WorkingDirectory .\AnalyzeTypeBenchmark\bin\Release\net6.0 -NoNewWindow -Wait
}