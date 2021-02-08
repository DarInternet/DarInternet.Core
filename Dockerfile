FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/DarInternet.Api/DarInternet.Api.csproj", "src/DarInternet.Api/"]
COPY ["src/DarInternet.Infrastructure/DarInternet.Infrastructure.csproj", "src/DarInternet.Infrastructure/"]
COPY ["src/DarInternet.Domain/DarInternet.Domain.csproj", "src/DarInternet.Domain/"]
COPY ["src/DarInternet.Application/DarInternet.Application.csproj", "src/DarInternet.Application/"]
RUN dotnet restore "src/DarInternet.Api/DarInternet.Api.csproj"
COPY . .
WORKDIR "/src/src/DarInternet.Api"
RUN dotnet build "DarInternet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DarInternet.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DarInternet.Api.dll"]