# Деплой на VPS

Стек: nginx (порт 80) → ASP.NET Core (порт 8080, внутри сети compose) → PostgreSQL 17.
Миграции применяются автоматически при старте приложения; при пустой базе создаётся
администратор из `DEFAULT_ADMIN_LOGIN` / `DEFAULT_ADMIN_PASSWORD`.

## Первый запуск на сервере

1. Установить Docker (включает compose-плагин):

   ```bash
   curl -fsSL https://get.docker.com | sh
   ```

2. Скопировать проект на сервер (например, в `/opt/salary-site`) любым способом:
   `git clone`, `scp` или rsync.

3. Создать файл `.env` рядом с `docker-compose.yml` по образцу `.env.example`
   и задать свои пароли.

4. Запустить:

   ```bash
   docker compose up -d --build
   ```

Приложение будет доступно на `http://<IP сервера>/`.

## Обновление

```bash
cd /opt/salary-site
git pull            # или заново скопировать файлы
docker compose up -d --build
```

## Полезные команды

```bash
docker compose ps                # статус контейнеров
docker compose logs -f webapp    # логи приложения
docker compose logs -f nginx     # логи nginx
docker compose down              # остановить (данные БД сохраняются в volume pgdata)
```

## Данные

- База данных: volume `pgdata` (переживает пересборку и перезапуск).
- Аудит-логи приложения: volume `applogs` (`/app/logs` внутри контейнера).

## HTTPS

Сейчас приложение отдаётся по HTTP (для доступа по IP сертификат выпустить нельзя).
Когда появится домен: направить A-запись на сервер, добавить в `nginx/nginx.conf`
server-блок с сертификатом (проще всего — certbot) и включить `Cookie:Secure=true`
через переменную окружения `Cookie__Secure=true` у сервиса `webapp`.
