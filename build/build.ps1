Write-Host $Env:appveyor_build_version

Get-Content .\src\JsonTransformation\JsonTransformation.csproj

dotnet build .\src -c Release -o "..\..\Output"
dotnet test .\src -o "..\..\Output"
dotnet pack ".\src\JsonTransformation\JsonTransformation.csproj" -c Release -o "..\..\Output"