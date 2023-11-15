#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SwizlyPeasy.Gateway.API/SwizlyPeasy.Gateway.API.csproj", "SwizlyPeasy.Gateway.API/"]
RUN dotnet restore "SwizlyPeasy.Gateway.API/SwizlyPeasy.Gateway.API.csproj"
COPY . .
WORKDIR "/src/SwizlyPeasy.Gateway.API"
RUN dotnet build "SwizlyPeasy.Gateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SwizlyPeasy.Gateway.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SwizlyPeasy.Gateway.API.dll"]