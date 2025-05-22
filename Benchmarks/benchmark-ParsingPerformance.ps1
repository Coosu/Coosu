Push-Location ./ParsingPerformanceTest
try {
    dotnet build --configuration Release --framework "net8.0" 
    dotnet run --configuration Release --framework "net8.0" --no-build
}
finally {
    Pop-Location
}