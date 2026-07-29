#!/usr/bin/env bash
# Обновление приложения на сервере: забрать свежий код из git и пересобрать контейнеры.
# Запуск:  bash deploy.sh
set -euo pipefail

cd "$(dirname "$0")"

echo "==> Забираем изменения из git"
git pull --ff-only

echo "==> Собираем образ и перезапускаем контейнеры"
docker compose up -d --build

echo "==> Удаляем старые образы"
docker image prune -f

echo "==> Статус"
docker compose ps

echo
echo "Готово. Логи приложения: docker compose logs -f webapp"
