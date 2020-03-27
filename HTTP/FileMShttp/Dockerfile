FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY FileMShttp/ FileMShttp/
COPY FileMS/ FileMS/
WORKDIR "/src/FileMShttp"
RUN dotnet restore "FileMShttp.csproj"
RUN dotnet build "FileMShttp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileMShttp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileMShttp.dll"]