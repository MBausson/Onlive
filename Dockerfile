FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
EXPOSE 8001
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnliveServer/OnliveServer.csproj", "OnliveServer/"]
COPY ["OnliveConstants/OnliveConstants.csproj", "OnliveConstants/"]
RUN dotnet restore "OnliveServer/OnliveServer.csproj"
COPY . .
WORKDIR "/src/OnliveServer"
RUN dotnet build "./OnliveServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OnliveServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnliveServer.dll"]
CMD ["--ip 0.0.0.0", "--port 8001"]
