FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 1337/udp
EXPOSE 1338/udp

ENV DOTNET_EnableDiagnostics=0

FROM mcr.microsoft.com/dotnet/sdk:6.0  AS build

WORKDIR /src
COPY ["MainServer/MainServer.csproj", "MainServer/"]
COPY ["/Libs/", "./Libs/"]
COPY ["Mmogf.Servers/Mmogf.Servers.csproj", "Mmogf.Servers/"]
COPY ["Mmogf/Mmogf.csproj", "Mmogf/"]
RUN dotnet restore "MainServer/MainServer.csproj"
COPY . .
WORKDIR "/src/MainServer"

FROM build AS publish
RUN dotnet publish "MainServer.csproj" -a x64 -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["./MmogfMainServer"]