# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TechMoves Logistics/TechMovesLogistics.csproj", "TechMoves Logistics/"]
COPY ["TechMovesLogistics.Core/TechMovesLogistics.Core.csproj", "TechMovesLogistics.Core/"]

RUN dotnet restore "TechMoves Logistics/TechMovesLogistics.csproj"

COPY ["TechMoves Logistics/", "TechMoves Logistics/"]
COPY TechMovesLogistics.Core/ TechMovesLogistics.Core/

RUN dotnet publish "TechMoves Logistics/TechMovesLogistics.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TechMovesLogistics.dll"]
