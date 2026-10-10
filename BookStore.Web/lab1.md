Oto Twój uporządkowany, czysty tekst, idealnie przygotowany do wklejenia w pliku lab1.md:

Pytania Kontrolne

1. Czym różnią się metody GET i POST z punktu widzenia idempotentności i miejsca przekazywania danych?

Metody GET i POST:

Idempotentność: GET jest metodą idempotentną (wielokrotne wysłanie tego samego żądania daje ten sam efekt i nie zmienia stanu serwera). POST nie jest idempotentny (wielokrotne wywołanie może np. utworzyć kilka identycznych rekordów w bazie).

Miejsce przekazywania danych: GET przekazuje parametry jawnie, doklejając je do adresu URL (np. w postaci Query String). POST przesyła dane w sposób niewidoczny w pasku adresu, umieszczając je wewnątrz ciała żądania (Body).

2. Co oznaczają kody 200, 302, 400, 401, 403, 404, 405, 415, 500?

Kody statusu HTTP:

200 – OK (żądanie przetworzone poprawnie).

302 – Found (tymczasowe przekierowanie na inny adres).

400 – Bad Request (błędne żądanie klienta, np. niewłaściwy format danych wejściowych).

401 – Unauthorized (brak autoryzacji, użytkownik niezalogowany).

403 – Forbidden (użytkownik jest zalogowany, ale nie ma uprawnień do tego zasobu).

404 – Not Found (nie znaleziono strony lub zasobu).

405 – Method Not Allowed (użyto złej metody HTTP, np. próba wysłania GET na endpoint wymagający POST).

415 – Unsupported Media Type (serwer odrzuca żądanie ze względu na zły format danych, np. oczekiwano JSON, a wysłano zwykły tekst).

500 – Internal Server Error (krytyczny błąd serwera, najczęściej niespodziewany wyjątek w kodzie aplikacji).

3. Skąd model binding wie, czy id pochodzi z ścieżki, czy z query string?

Framework analizuje żądanie według określonej hierarchii. Najpierw sprawdza definicje tras routingu (domyślnie {controller}/{action}/{id?}). Jeśli adres URL zawiera odpowiedni segment pasujący do miejsca {id} w ścieżce, to stamtąd pobiera wartość. Jeśli tego segmentu brakuje w ścieżce, framework automatycznie przechodzi do szukania parametru o nazwie id w Query Stringu (po znaku ?).

4. Dlaczego pliki z wwwroot są dostępne pod adresem, a pliki z Controllers/ nie?

Wynika to z architektury i bezpieczeństwa aplikacji ASP.NET Core. Katalog wwwroot jest specjalnie oznaczony w konfiguracji (dzięki oprogramowaniu pośredniczącemu UseStaticFiles()) jako jedyny publiczny katalog przeznaczony do serwowania plików bezpośrednio do przeglądarki klienta. Pliki w folderze Controllers/ (i innych) stanowią kod źródłowy backendu, który ma być wykonywany wyłącznie na serwerze, a nie pobierany przez użytkowników. Dostęp do nich z poziomu przeglądarki jest domyślnie zablokowany.