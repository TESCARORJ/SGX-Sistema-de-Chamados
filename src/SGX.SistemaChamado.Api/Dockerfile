FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY SGX.SistemaChamado.sln ./
COPY src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj src/SGX.SistemaChamado.Api/
COPY src/SGX.SistemaChamado.Application/SGX.SistemaChamado.Application.csproj src/SGX.SistemaChamado.Application/
COPY src/SGX.SistemaChamado.Domain/SGX.SistemaChamado.Domain.csproj src/SGX.SistemaChamado.Domain/
COPY src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj src/SGX.SistemaChamado.Infrastructure/

RUN dotnet restore src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj

COPY . .
RUN dotnet publish src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SGX.SistemaChamado.Api.dll"]
