FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5672

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY Simulator/ Simulator/
RUN dotnet restore "Simulator/Simulator.csproj"

RUN dotnet build "Simulator/Simulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Simulator/Simulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Simulator.dll"]