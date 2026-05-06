FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY SGX.SistemaChamado.sln ./
COPY src/SGX.SistemaChamado.Worker.Email/SGX.SistemaChamado.Worker.Email.csproj src/SGX.SistemaChamado.Worker.Email/
COPY src/SGX.SistemaChamado.Application/SGX.SistemaChamado.Application.csproj src/SGX.SistemaChamado.Application/
COPY src/SGX.SistemaChamado.Domain/SGX.SistemaChamado.Domain.csproj src/SGX.SistemaChamado.Domain/
COPY src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj src/SGX.SistemaChamado.Infrastructure/

RUN dotnet restore src/SGX.SistemaChamado.Worker.Email/SGX.SistemaChamado.Worker.Email.csproj

COPY . .
RUN dotnet publish src/SGX.SistemaChamado.Worker.Email/SGX.SistemaChamado.Worker.Email.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SGX.SistemaChamado.Worker.Email.dll"]
