$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\.."
mvn clean verify
