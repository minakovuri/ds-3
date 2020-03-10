# Задание #3 Key-value хранилище, очередь сообщений, паттерн publish-subscribe.

Задание делается на базе задания #2.

Компонент **BackendApi** должен помещать принятые данные в **Redis**-хранилище по сгенерированному идентификатору в качестве ключа. Также, после сохранения данных в хранилище, необходимо публиковать событие **TextCreated** (в сообщении содержится ид-р текста). События публикуются через брокер сообщений **NATS**. Название шины сообщений - **events**.

Добавляется новый компонент - консольное приложение **TextListener**. 

Создание косольного приложения:
```
  dotnet new console
```
При запуске компонент **TextListener** подписывается на события в шине сообщений **events**. Получив сообщение **TextCreated**, компонент извлекает из **Redis**-хранилища текст по идентификатору, полученному в сообщении, и выводит в стандартный вывод пару значений *(ид-р, текст)*, т.е. пишет лог событий в консоль.

Скрипты автоматизации должны быть модифицированы для разворачивания системы с учётом нового компонента.

Библиотека для работы с Redis: https://www.nuget.org/packages/StackExchange.Redis/
Библиотека для работы с NATS: https://github.com/nats-io/nats.net
Примеры NATS для .Net: https://github.com/nats-io/nats.net/tree/master/src/Samples

Документация: https://stackexchange.github.io/StackExchange.Redis/Basics

Подключение библиотек:
```
  dotnet add package StackExchange.Redis
  dotnet add package nats.client
```
