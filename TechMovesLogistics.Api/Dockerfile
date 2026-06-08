# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TechMovesLogistics.Api/TechMovesLogistics.Api.csproj", "TechMovesLogistics.Api/"]
COPY ["TechMovesLogistics.Core/TechMovesLogistics.Core.csproj", "TechMovesLogistics.Core/"]

RUN dotnet restore "TechMovesLogistics.Api/TechMovesLogistics.Api.csproj"

COPY TechMovesLogistics.Api/ TechMovesLogistics.Api/
COPY TechMovesLogistics.Core/ TechMovesLogistics.Core/

RUN dotnet publish "TechMovesLogistics.Api/TechMovesLogistics.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

RUN mkdir -p /app/wwwroot/uploads

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TechMovesLogistics.Api.dll"]
