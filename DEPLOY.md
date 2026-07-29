# Деплой на VPS

Стек: nginx (порт 80) → ASP.NET Core (порт 8080, внутри сети compose) → PostgreSQL 17.
Код доставляется на сервер через **git**, образ собирается прямо на сервере.
Миграции применяются автоматически при старте приложения.

## Что и где настраивается

| Что | Где | Когда применяется |
|---|---|---|
| Фермы, логины и пароли админов | `WebApp/appsettings.json`, секция `Farms` | при старте приложения |
| Пароль Postgres, часовой пояс | `.env` рядом с `docker-compose.yml` (**необязателен**) | при старте контейнеров |

Файла `.env` может не быть — тогда возьмутся значения по умолчанию из `docker-compose.yml`.
Порт Postgres наружу не проброшен, база доступна только контейнеру приложения.

## Первый запуск на новом сервере

1. Установить Docker и git:

   ```bash
   curl -fsSL https://get.docker.com | sh && apt-get install -y git
   ```

2. Добавить swap — на 2 ГБ RAM сборка .NET без него может упасть по памяти
   (если swap уже есть, шаг пропустить, проверка: `swapon --show`):

   ```bash
   fallocate -l 2G /swapfile && chmod 600 /swapfile && mkswap /swapfile && swapon /swapfile && echo '/swapfile none swap sw 0 0' >> /etc/fstab
   ```

3. Склонировать репозиторий **именно в `/opt/salary-site`** (имя каталога задаёт
   имя compose-проекта, от которого зависят имена volume'ов с базой):

   ```bash
   git clone https://github.com/vinwap07/AutomatedSalaryReportSite.git /opt/salary-site
   ```

4. Запустить:

   ```bash
   cd /opt/salary-site && docker compose up -d --build
   ```

Приложение будет доступно на `http://<IP сервера>/`. Первая сборка идёт
несколько минут — качается .NET SDK (~1 ГБ), дальше он берётся из кэша.

## Обновление

```bash
cd /opt/salary-site && bash deploy.sh
```

Скрипт делает `git pull`, пересобирает образ, перезапускает контейнеры и чистит
старые образы. То же самое вручную:

```bash
cd /opt/salary-site && git pull && docker compose up -d --build
```

Локально перед этим: закоммитить и запушить изменения (`git push`).

## Переезд текущего сервера на git

Сейчас файлы в `/opt/salary-site` залиты по SFTP и не являются git-репозиторием.
Один раз выполнить на сервере (данные в volume'ах не пострадают — они привязаны
к имени каталога `salary-site`, которое сохраняется):

```bash
cd /opt && mv salary-site salary-site.bak && git clone https://github.com/vinwap07/AutomatedSalaryReportSite.git salary-site && cp salary-site.bak/.env salary-site/ 2>/dev/null; cd salary-site && docker compose up -d --build
```

После проверки, что сайт работает и данные на месте, удалить `rm -rf /opt/salary-site.bak`.

## Если репозиторий приватный

`git clone` по HTTPS попросит логин. Проще выдать серверу read-only доступ по SSH-ключу:

```bash
ssh-keygen -t ed25519 -N "" -f ~/.ssh/id_ed25519 && cat ~/.ssh/id_ed25519.pub
```

Вывод добавить в GitHub: репозиторий → Settings → Deploy keys → Add deploy key
(галочку «Allow write access» не ставить). Дальше клонировать по SSH:

```bash
git clone git@github.com:vinwap07/AutomatedSalaryReportSite.git /opt/salary-site
```

## Фермы и админы

Каждая ферма — независимое хозяйство. Разделение идёт через учётные записи
администраторов: админ видит и меняет только своих работников, технику,
справочники, журнал работ и учётные записи. Всё, что он создаёт, автоматически
относится к его ферме. Логины уникальны в пределах всей системы: при входе
ферма определяется по учётной записи.

Список ферм — в `WebApp/appsettings.json`:

```json
"Farms": [
  { "Id": "11111111-...", "Name": "Ферма №1", "AdminLogin": "admin",  "AdminPassword": "..." },
  { "Id": "22222222-...", "Name": "Ферма №2", "AdminLogin": "admin2", "AdminPassword": "..." }
]
```

- **Новая ферма**: дописать блок с новым уникальным `Id`, запушить, `bash deploy.sh`.
- **`Id` менять нельзя** после первого запуска — к нему привязаны все данные фермы.
  Все данные, существовавшие до разделения по фермам, отнесены к первой ферме.
- **`AdminPassword` работает только при создании учётной записи.** Если админ с
  таким логином уже есть, пароль в конфиге игнорируется — менять пароль нужно в
  интерфейсе: «Справочники» → «Учётные записи» → «Сменить пароль».
- `Name` наоборот обновляется из конфига при каждом запуске.

Пароли лежат в репозитории, поэтому он должен быть приватным.

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
- Выгрузка дампа на странице «Сервис» содержит данные только текущей фермы.

## HTTPS

Сейчас приложение отдаётся по HTTP (для доступа по IP сертификат выпустить нельзя).
Когда появится домен: направить A-запись на сервер, добавить в `nginx/nginx.conf`
server-блок с сертификатом (проще всего — certbot) и включить `Cookie:Secure=true`
через переменную окружения `Cookie__Secure=true` у сервиса `webapp`.
