Тестовое задание - система управления задачами

Как запустить:
1. Клонировать проект или распаковать архив.
2. Открыть .sln в Visual Studio.
3. Создать в PostgreSql две базы данных task_service и notification_service. Запустить запросы из файлов dbTaskService и dbNotificationService соответственно.
4. В файлах NotificationServiceContext и TaskServiceContext в 26 и 30 строчке соответственно изменить данные о базах данных на ваши (port, host, username, password).
5. Запустить проект (F5 или `dotnet run`).

Как проверить:
1. В архиве приложен swagger.json. Можно открыть через Swagger Editor или импортировать в Postman.
2. Или же открыть в браузере https://localhost:5001/swagger (для TaskService) или http://localhost:5000/swagger (для NotificationService).