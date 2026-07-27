FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Сначала копируем только csproj-файлы, чтобы кэшировать restore
COPY Domain/Domain.csproj Domain/
COPY Data/Data.csproj Data/
COPY WebApp/WebApp.csproj WebApp/
RUN dotnet restore WebApp/WebApp.csproj

COPY Domain/ Domain/
COPY Data/ Data/
COPY WebApp/ WebApp/
RUN dotnet publish WebApp/WebApp.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Каталоги для аудит-логов и ключей DataProtection должны быть доступны непривилегированному пользователю
RUN mkdir -p /app/logs /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /app/logs /home/app/.aspnet
USER app

EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApp.dll"]
