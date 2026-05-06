$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\.."
mvn -pl GETI.SistemaChamado.Worker.Email spring-boot:run

