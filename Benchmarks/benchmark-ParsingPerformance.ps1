dotnet build ParsingPerformanceTest --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process .\ParsingPerformanceTest\bin\Release\net6.0\ParsingPerformanceTest.exe -WorkingDirectory .\ParsingPerformanceTest\bin\Release\net6.0 -NoNewWindow -Wait
}