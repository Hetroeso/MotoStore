MotoStore: Инструкция по запуску на другом компьютере

✅ 1. Скопируй весь проект

Перенеси папку с решением (.sln, MotoStore.Api/, MotoStoreWpf/) на другой компьютер любым удобным способом (GitHub, флешка, облако).

✅ 2. Установи необходимое ПО

Что нужно

Где взять

.NET SDK 7+

Для сборки и запуска API и WPF

Visual Studio

С рабочими нагрузками .NET и WPF

SQL Server / SQLite

Зависит от того, как хранится база

✅ 3. Проверь appsettings.json

В MotoStore.Api/appsettings.json убедись, что строка подключения к БД корректна. Если нет localdb — замени на обычный SQL Server.

✅ 4. Восстанови NuGet-пакеты

Visual Studio: Tools > NuGet Package Manager > Restore или Build > Restore NuGet Packages

✅ 5. Настрой запуск проектов

Right click по решению > Set Startup Projects > Multiple startup projects: Start для MotoStore.Api и MotoStoreWpf

✅ 6. Запуск

Нажми F5 или кнопку Start в Visual Studio

Убедись, что API запущен (обычно на https://localhost:PORT/api/...)

✅ 7. Если Visual Studio не нужна

cd MotoStore.Api
 dotnet run

cd ../MotoStoreWpf
 dotnet run

✅ Что ещё может потребоваться:

Проверь файл launchSettings.json (для настройки URL API)

Учетные записи в базу могут требовать миграции / инициализации