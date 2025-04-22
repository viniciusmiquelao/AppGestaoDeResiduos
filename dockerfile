# Imagem base do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Imagem para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AppGestaoDeResiduos/AppGestaoDeResiduos.csproj", "AppGestaoDeResiduos/"]
RUN dotnet restore "AppGestaoDeResiduos/AppGestaoDeResiduos.csproj"
COPY . .
WORKDIR "/src/AppGestaoDeResiduos"
RUN dotnet build "AppGestaoDeResiduos.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppGestaoDeResiduos.csproj" -c Release -o /app/publish

# Imagem final com aplicação pronta
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppGestaoDeResiduos.dll"]
