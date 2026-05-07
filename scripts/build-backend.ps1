$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\.."
dotnet restore SGX.SistemaChamado.sln
dotnet build SGX.SistemaChamado.sln
dotnet test SGX.SistemaChamado.sln
