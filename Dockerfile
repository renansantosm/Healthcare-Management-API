FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
USER app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/HealthcareManagement.API/HealthcareManagement.API.csproj", "src/HealthcareManagement.API/"]
RUN dotnet restore "src/HealthcareManagement.API/HealthcareManagement.API.csproj"
COPY . .
WORKDIR "/src/src/HealthcareManagement.API"
RUN dotnet build "HealthcareManagement.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "HealthcareManagement.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthcareManagement.API.dll"]