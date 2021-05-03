FROM mcr.microsoft.com/dotnet/aspnet:2.1.25-alpine3.12 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:2.1.813-alpine3.12 AS build
WORKDIR /src
COPY K8S.UI.csproj /src
RUN dotnet restore "K8S.UI.csproj"
COPY . .
RUN dotnet build "K8S.UI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "K8S.UI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENV ASPNETCORE_URLS http://*:8081

ENTRYPOINT ["dotnet", "K8S.UI.dll"]