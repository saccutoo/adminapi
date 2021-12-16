FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Services/Auth.API/Auth.API.csproj", "src/Services/Auth.API/"]
COPY ["src/BuildingBlocks/Utils/Utils/Utils.csproj", "src/BuildingBlocks/Utils/Utils/"]
RUN dotnet restore "src/Services/Auth.API/Auth.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Auth.API"
RUN dotnet build "Auth.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Auth.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Auth.API.dll"]