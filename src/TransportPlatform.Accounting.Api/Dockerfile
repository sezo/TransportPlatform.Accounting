FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5101

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["NuGet.config", "."]
RUN sed -i 's/localhost:5555/host.docker.internal:5555/g' NuGet.config
COPY ["src/TransportPlatform.Accounting.Api/TransportPlatform.Accounting.Api.csproj", "TransportPlatform.Accounting.Api/"]
COPY ["src/TransportPlatform.Accounting.Infrastructure/TransportPlatform.Accounting.Infrastructure.csproj", "TransportPlatform.Accounting.Infrastructure/"]
COPY ["src/TransportPlatform.Accounting.Application/TransportPlatform.Accounting.Application.csproj", "TransportPlatform.Accounting.Application/"]
COPY ["src/TransportPlatform.Accounting.Domain/TransportPlatform.Accounting.Domain.csproj", "TransportPlatform.Accounting.Domain/"]
RUN dotnet restore "TransportPlatform.Accounting.Api/TransportPlatform.Accounting.Api.csproj"

COPY src/ .
RUN dotnet build "TransportPlatform.Accounting.Api/TransportPlatform.Accounting.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransportPlatform.Accounting.Api/TransportPlatform.Accounting.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransportPlatform.Accounting.Api.dll"]
