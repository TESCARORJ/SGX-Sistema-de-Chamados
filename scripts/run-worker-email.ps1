$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\.."
dotnet run --project src/SGX.SistemaChamado.Worker.Email/SGX.SistemaChamado.Worker.Email.csproj

