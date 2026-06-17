# Visit Service

## Descrizione

Il **Visit Service** è un microservizio responsabile della gestione delle visite agli immobili in vendita.

Consente agli utenti di prenotare visite per gli immobili disponibili e ai proprietari di confermare o rifiutare le richieste ricevute.

## Architettura

Il servizio fa parte di un'architettura a microservizi:

* Espone API REST
* Tutte le operazioni sono protette tramite autenticazione JWT
* Comunica con il **Property Service** per verificare l'esistenza, la disponibilità e il proprietario degli immobili

## Avvio del servizio

Per avviare il servizio in locale:

```bash
# esempio
dotnet run
```

Oppure con Docker:

```bash
docker-compose up
```

Il servizio sarà disponibile su:

```
http://localhost:7804
```

## API

Documentazione Swagger disponibile qui:

```
http://localhost:7804/swagger/index.html
```

## Autenticazione e autorizzazione

Il servizio utilizza **JWT (JSON Web Token)** per proteggere tutte le operazioni.

### Accesso autenticato

Tutte le operazioni richiedono un token JWT valido.

Il client deve includere il token nell'header HTTP:

```http
Authorization: Bearer <token>
```

### Regole di autorizzazione

#### Prenotazione di una visita

Un utente autenticato può prenotare una visita soltanto se:

* l'immobile esiste
* l'immobile non è stato venduto
* non è il proprietario dell'immobile
* la data della visita è futura

#### Modifica di una visita

Una visita può essere modificata solo se:

* l'utente autenticato è il visitatore che ha effettuato la prenotazione
* la visita è ancora in stato **Pending**
* la nuova data è futura

#### Eliminazione di una visita

Una visita può essere eliminata solo se:

* l'utente autenticato è il visitatore che ha effettuato la prenotazione
* la visita è ancora in stato **Pending**

#### Conferma di una visita

Una visita può essere confermata solo se:

* l'utente autenticato è il proprietario dell'immobile
* la visita è in stato **Pending**
* l'immobile è ancora disponibile

#### Rifiuto di una visita

Una visita può essere rifiutata solo se:

* l'utente autenticato è il proprietario dell'immobile
* la visita è in stato **Pending**

#### Consultazione delle visite

* Un utente può visualizzare solo le visite che lo coinvolgono come proprietario o come visitatore.
* L'elenco delle visite restituisce esclusivamente le visite associate all'utente autenticato.

### Stati di una visita

Una visita può assumere i seguenti stati:

| Stato     | Descrizione                                             |
| --------- | ------------------------------------------------------- |
| Pending   | Visita richiesta e in attesa di approvazione            |
| Confirmed | Visita confermata dal proprietario                      |
| Cancelled | Visita rifiutata dal proprietario                       |
| Completed | Visita completata automaticamente dopo la data prevista |

Le visite vengono automaticamente marcate come **Completed** quando la data della visita è trascorsa.

## Endpoints principali

| Metodo | Endpoint                 | Autenticazione | Descrizione                                |
| ------ | ------------------------ | -------------- | ------------------------------------------ |
| POST   | /api/visits              | ✅ Sì           | Prenota una nuova visita                   |
| GET    | /api/visits              | ✅ Sì           | Recupera le visite dell'utente autenticato |
| GET    | /api/visits/{id}         | ✅ Sì           | Recupera una visita specifica              |
| PUT    | /api/visits/{id}         | ✅ Sì           | Modifica una visita                        |
| DELETE | /api/visits/{id}         | ✅ Sì           | Elimina una visita                         |
| PATCH  | /api/visits/{id}/confirm | ✅ Sì           | Conferma una visita                        |
| PATCH  | /api/visits/{id}/reject  | ✅ Sì           | Rifiuta una visita                         |

## Integrazioni

### Property Service

Il Visit Service utilizza il Property Service per:

* verificare l'esistenza dell'immobile
* verificare che l'immobile sia disponibile
* recuperare il proprietario dell'immobile
* impedire la prenotazione di visite su immobili già venduti

### Controlli automatici

* Le visite scadute vengono automaticamente marcate come **Completed**.
* Tutti i controlli di autorizzazione si basano sull'utente autenticato contenuto nel JWT.
