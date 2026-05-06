$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\.."
mvn -pl GETI.SistemaChamado.Api spring-boot:run

