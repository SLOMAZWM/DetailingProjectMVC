# Dokumentacja Projektu: System Detailingu Samochodowego

## Spis Treści

1. [Wstęp](#wstęp)
    - [1.1 Cel Dokumentacji](#11-cel-dokumentacji)
    - [1.2 Zakres Projektu](#12-zakres-projektu)
2. [Technologie i Narzędzia](#technologie-i-narzędzia)
    - [.NET Core MVC](#.net-core-mvc)
    - [Entity Framework](#entity-framework)
    - [Microsoft SQL Server](#microsoft-sql-server)
    - [Inne Technologie](#inne-technologie)
3. [Architektura Systemu](#architektura-systemu)
    - [3.1 Warstwy Aplikacji](#31-warstwy-aplikacji)
4. [Projekt Bazy Danych](#projekt-bazy-danych)
    - [4.1 Model Danych](#41-model-danych)
5. [Funkcjonalności Aplikacji](#funkcjonalności-aplikacji)
    - [5.1 Strona Główna](#51-strona-główna)
    - [5.2 Moduł Sklepu](#52-moduł-sklepu)
        - [5.2.1 Przegląd Asortymentu](#521-przegląd-asortymentu)
        - [5.2.2 Koszyk Zakupowy](#522-koszyk-zakupowy)
        - [5.2.3 Składanie Zamówień](#523-składanie-zamówień)
        - [5.2.4 Podgląd Zamówień](#524-podgląd-zamówień)
    - [5.3 Zarządzanie Zamówieniami dla Pracowników](#53-zarządzanie-zamówieniami-dla-pracowników)
    - [5.4 System Rejestracji i Logowania](#54-system-rejestracji-i-logowania)
        - [5.4.1 Rejestracja Klienta](#541-rejestracja-klienta)
        - [5.4.2 Rejestracja Pracownika](#542-rejestracja-pracownika)
        - [5.4.3 Zmiana Danych Konta](#543-zmiana-danych-konta)
    - [5.5 Formularz Kontaktowy](#55-formularz-kontaktowy)
        - [5.5.1 Integracja z E-mailem](#551-integracja-z-e-mailem)
6. [Interfejs Użytkownika](#interfejs-użytkownika)
    - [6.1 Projekt UI](#61-projekt-ui)
    - [6.2 Responsywność](#62-responsywność)
7. [Bezpieczeństwo](#bezpieczeństwo)
    - [7.1 Autoryzacja i Autentykacja](#71-autoryzacja-i-autentykacja)
    - [7.2 Ochrona Danych](#72-ochrona-danych)
8. [Kontakt](#kontakt)

---

## 1. Wstęp

### 1.1 Cel Dokumentacji

Celem niniejszej dokumentacji jest szczegółowe opisanie projektu systemu detailingu samochodowego opartego na technologii .NET Core MVC. Dokumentacja ma na celu ułatwienie zrozumienia architektury systemu, funkcjonalności, implementacji oraz zapewnienie wytycznych dla dalszego rozwoju i utrzymania aplikacji.

### 1.2 Zakres Projektu

Projekt obejmuje stworzenie pełnoprawnej aplikacji webowej dla firmy oferującej usługi detailingu samochodowego. System umożliwia klientom przeglądanie oferty, składanie zamówień, zarządzanie kontem oraz kontakt z firmą. Pracownicy mają dostęp do zarządzania zamówieniami oraz administracji systemu.

## 2. Technologie i Narzędzia

### .NET Core MVC

Aplikacja została zbudowana przy użyciu frameworka .NET Core MVC, który umożliwia tworzenie skalowalnych i wydajnych aplikacji webowych. MVC (Model-View-Controller) zapewnia separację logiki biznesowej, interfejsu użytkownika oraz zarządzania danymi.

### Entity Framework

Entity Framework (EF) jest używany jako ORM (Object-Relational Mapping), co umożliwia interakcję z bazą danych za pomocą obiektów .NET, eliminując potrzebę pisania skomplikowanych zapytań SQL.

### Microsoft SQL Server

Jako system zarządzania bazą danych (DBMS) wybrano Microsoft SQL Server. Baza danych jest lokalna, co zapewnia szybki dostęp i łatwą konfigurację w środowisku deweloperskim.

### Inne Technologie

- **HTML5, CSS3, JavaScript**: Do tworzenia responsywnego i interaktywnego interfejsu użytkownika.
- **Bootstrap**: Framework CSS ułatwiający tworzenie responsywnych stron.
- **ASP.NET Identity**: Do zarządzania autoryzacją i autentykacją użytkowników.
- **MailKit**: Biblioteka do obsługi wysyłania e-maili z formularza kontaktowego.

## 3. Architektura Systemu

### 3.1 Warstwy Aplikacji

System jest podzielony na następujące warstwy:

1. **Warstwa Prezentacji (UI)**: Odpowiada za interakcję z użytkownikiem. Zbudowana przy użyciu Razor Views w ASP.NET Core MVC.
2. **Warstwa Logiki Biznesowej**: Zawiera logikę aplikacji, zarządzanie zamówieniami, użytkownikami itd.
3. **Warstwa Dostępu do Danych**: Używa Entity Framework do komunikacji z bazą danych SQL Server.
4. **Warstwa Infrastruktury**: Zawiera komponenty wspierające, takie jak wysyłka e-maili, logowanie, itp.

## 4. Projekt Bazy Danych

### 4.1 Model Danych

Model danych obejmuje następujące główne encje:

- **User**: Reprezentuje użytkowników systemu (klientów i pracowników).
- **Product**: Przedmioty dostępne w sklepie.
- **Order**: Zamówienia składane przez klientów.
- **OrderItem**: Pozycje zamówienia, powiązane z produktami.
- **ContactMessage**: Wiadomości wysyłane przez formularz kontaktowy.

## 5. Funkcjonalności Aplikacji

### 5.1 Strona Główna

Strona główna pełni rolę wizytówki firmy, prezentując podstawowy asortyment sklepu oraz kluczowe informacje o usługach detailingu samochodowego. Zawiera:

- **Banner**: Zawierający logo i slogan firmy.
- **Przegląd Asortymentu**: Wybrane produkty z modułu sklepu.
- **Informacje o Usługach**: Krótkie opisy oferowanych usług.
- **Linki do Podstron**: Sklep, Kontakt, Rejestracja/Logowanie.

### 5.2 Moduł Sklepu

Moduł sklepu umożliwia klientom przeglądanie, wybieranie i zakup produktów związanych z detailingiem samochodowym.

#### 5.2.1 Przegląd Asortymentu

- **Katalog Produktów**: Lista dostępnych produktów z obrazkami, opisami i cenami.
- **Filtrowanie i Sortowanie**: Opcje filtrowania produktów według kategorii, ceny, popularności.
- **Szczegóły Produktu**: Strona z pełnym opisem, zdjęciami oraz opiniami klientów.

#### 5.2.2 Koszyk Zakupowy

- **Dodawanie do Koszyka**: Możliwość dodania wybranych produktów do koszyka.
- **Podgląd Koszyka**: Lista produktów w koszyku wraz z ilością, ceną jednostkową i całkowitą.
- **Edycja Koszyka**: Zmiana ilości produktów, usuwanie pozycji.
- **Podsumowanie Zamówienia**: Przedstawienie kosztów, podatków i łącznej kwoty do zapłaty.

#### 5.2.3 Składanie Zamówień

- **Formularz Zamówienia**: Wypełnienie danych do wysyłki, wybór metody płatności.
- **Potwierdzenie Zamówienia**: E-mail z potwierdzeniem oraz podsumowaniem zamówienia.
- **Przetwarzanie Zamówienia**: Automatyczne tworzenie rekordu zamówienia w bazie danych.

#### 5.2.4 Podgląd Zamówień

- **Historia Zamówień**: Klient może przeglądać swoje poprzednie zamówienia.
- **Szczegóły Zamówienia**: Widok szczegółowy wybranego zamówienia, statusu i historii zmian.

### 5.3 Zarządzanie Zamówieniami dla Pracowników

- **Panel Administracyjny**: Dostępny dla zalogowanych pracowników.
- **Przegląd Wszystkich Zamówień**: Lista wszystkich zamówień z możliwością filtrowania.
- **Aktualizacja Statusów**: Możliwość zmiany statusu zamówienia (np. w trakcie realizacji, zrealizowane).
- **Szczegóły Zamówienia**: Podgląd pełnych informacji o zamówieniu oraz danych klienta.

### 5.4 System Rejestracji i Logowania

System umożliwia użytkownikom tworzenie kont oraz logowanie się do systemu.

#### 5.4.1 Rejestracja Klienta

- **Formularz Rejestracyjny**: Wprowadzenie danych takich jak imię, nazwisko, email, hasło.
- **Weryfikacja Email**: Potwierdzenie rejestracji poprzez link wysłany na adres email.
- **Tworzenie Konta**: Automatyczne przypisanie roli "Klient" do nowego użytkownika.

#### 5.4.2 Rejestracja Pracownika

- **Proces Rejestracji**: Rejestracja pracowników może być ograniczona do administratora systemu.
- **Przypisanie Ról**: Użytkownicy rejestrujący się jako pracownicy otrzymują odpowiednie uprawnienia.

#### 5.4.3 Zmiana Danych Konta

- **Edycja Profilu**: Możliwość zmiany danych osobowych, hasła.
- **Bezpieczeństwo**: Wymagane jest ponowne uwierzytelnienie przy zmianie kluczowych danych.

### 5.5 Formularz Kontaktowy

Formularz umożliwia klientom kontakt z firmą w celu uzyskania informacji lub umówienia się na usługę.

#### 5.5.1 Integracja z E-mailem

- **Formularz Kontaktowy**: Pola takie jak imię, email, temat, wiadomość.
- **Wysyłka Wiadomości**: Po wypełnieniu formularza wiadomość jest wysyłana na firmowy adres email.
- **Potwierdzenie dla Użytkownika**: Wyświetlenie komunikatu o pomyślnym wysłaniu wiadomości.

## 6. Interfejs Użytkownika

### 6.1 Projekt UI

Interfejs użytkownika został zaprojektowany z myślą o intuicyjności i estetyce. Użyto nowoczesnych wzorców projektowych, zapewniając spójność wizualną na wszystkich podstronach.

- **Nawigacja**: Górne menu z odnośnikami do kluczowych sekcji (Sklep, Kontakt, Konto).
- **Responsywne Elementy**: Optymalizacja dla różnych urządzeń, w tym komputerów, tabletów i smartfonów.
- **Czytelna Typografia**: Użycie czytelnych fontów i odpowiedniego kontrastu kolorów.

### 6.2 Responsywność

Aplikacja jest w pełni responsywna, zapewniając optymalne doświadczenie użytkownika na różnych urządzeniach. Wykorzystano framework Bootstrap do tworzenia elastycznego układu strony.

## 7. Bezpieczeństwo

### 7.1 Autoryzacja i Autentykacja

- **ASP.NET Identity**: Używany do zarządzania użytkownikami, rejestracją, logowaniem oraz zarządzaniem rolami.
- **Haszowanie Haseł**: Hasła są przechowywane w formie zahaszowanej z użyciem algorytmów bezpiecznych.
- **Ograniczenia Dostępu**: Kontrolowanie dostępu do zasobów na podstawie ról użytkowników (Klient, Pracownik, Administrator).

### 7.2 Ochrona Danych

- **Walidacja Danych**: Wszystkie dane wejściowe są walidowane zarówno po stronie klienta, jak i serwera.
- **Ochrona Przed Atakami**: Implementacja zabezpieczeń przeciwko SQL Injection, Cross-Site Scripting (XSS), Cross-Site Request Forgery (CSRF).
- **SSL/TLS**: Szyfrowanie komunikacji między klientem a serwerem za pomocą protokołu HTTPS.

---

*Dokumentacja została przygotowana na czerwiec 2024 roku. Wszystkie prawa zastrzeżone.*
