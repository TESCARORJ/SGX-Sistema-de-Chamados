@echo off
setlocal

set "REPO_ROOT=%~dp0"
set "MAVEN_CMD=%REPO_ROOT%.tools\apache-maven-3.9.9\bin\mvn.cmd"
set "LOCAL_JAVA_HOME=%REPO_ROOT%.tools\jdk-21"

if not exist "%MAVEN_CMD%" (
  echo [ERROR] Maven local nao encontrado em "%MAVEN_CMD%".
  exit /b 1
)

if exist "%LOCAL_JAVA_HOME%\bin\java.exe" (
  set "JAVA_HOME=%LOCAL_JAVA_HOME%"
  set "Path=%JAVA_HOME%\bin;%Path%"
)

call "%MAVEN_CMD%" %*
exit /b %ERRORLEVEL%
