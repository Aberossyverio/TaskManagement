FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TaskApi.csproj", "."]
RUN dotnet restore "./TaskApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TaskApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskApi.dll"]
