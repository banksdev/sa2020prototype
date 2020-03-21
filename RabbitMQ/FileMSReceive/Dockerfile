FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5672

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY FileMS/ FileMS/
COPY FileMSReceive/ FileMSReceive/
RUN dotnet restore "FileMSReceive/FileMSReceive.csproj"

RUN dotnet build "FileMSReceive/FileMSReceive.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileMSReceive/FileMSReceive.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileMSReceive.dll"]