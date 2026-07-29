FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Сначала копируем только csproj-файлы, чтобы кэшировать restore
COPY Domain/Domain.csproj Domain/
COPY Data/Data.csproj Data/
COPY WebApp/WebApp.csproj WebApp/
RUN dotnet restore WebApp/WebApp.csproj --disable-parallel

COPY Domain/ Domain/
COPY Data/ Data/
COPY WebApp/ WebApp/
# /m:1 и отключение shared compilation снижают пик потребления памяти:
# сборка идет прямо на VPS, где всего 2 ГБ RAM
RUN dotnet publish WebApp/WebApp.csproj -c Release -o /app/publish \
    /p:UseAppHost=false /p:UseSharedCompilation=false /m:1

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Каталоги для аудит-логов и ключей DataProtection должны быть доступны непривилегированному пользователю
RUN mkdir -p /app/logs /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /app/logs /home/app/.aspnet
USER app

EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApp.dll"]
