FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 1234
EXPOSE 3000
EXPOSE 1337/udp
EXPOSE 1338/udp

ENV DOTNET_EnableDiagnostics=0

FROM mcr.microsoft.com/dotnet/sdk:6.0  AS build

WORKDIR /src
COPY ["Mmo Game Framework/", "Mmo Game Framework/"]
COPY ["Worker/UnityMmo/Packages/games.cookiedragon.mmogf.core/MmogfMessages/Core/", "Worker/UnityMmo/Packages/games.cookiedragon.mmogf.core/MmogfMessages/Core/"]
RUN dotnet restore "Mmo Game Framework/MainServer/MainServer.csproj"
WORKDIR "/src/Mmo Game Framework/MainServer"

FROM build AS publish
RUN dotnet publish "MainServer.csproj" -a x64 -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY ["Worker/UnityMmo/worlds/", "worlds/"]
ENTRYPOINT ["./MmogfMainServer"]
