$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\..\src\SGX.SistemaChamado.Web"
npm.cmd install
npm.cmd run build

