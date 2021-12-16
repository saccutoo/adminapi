#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["src/Services/Lounge.API/Lounge.API.csproj", "src/Services/Lounge.API/"]
COPY ["src/BuildingBlocks/Utils/Utils/Utils.csproj", "src/BuildingBlocks/Utils/Utils/"]
RUN dotnet restore "src/Services/Lounge.API/Lounge.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Lounge.API"
RUN dotnet build "Lounge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lounge.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lounge.API.dll"]