$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\.."
dotnet run --project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj

