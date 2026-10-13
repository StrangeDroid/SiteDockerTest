# 🌸 Flower Quiz — Оранжерея знаний

A gamified quiz web app: grow a home greenhouse by answering questions.
Every flower asks questions on its own subject. Correct answers "water" it and earn coins; mistakes make it wilt.

Built with **Blazor Server (.NET 7)** and **PostgreSQL**, shipped as Docker containers.

> Created as a group project practice at Perm State University (ПГНИУ), 2025.

## Features

- **Registration and login.** Accounts with unique usernames and emails, plus a login token kept in a cookie for 7 days.
- **Quiz gameplay.** Flowers in your pots periodically ask a question with 4 answers:
  - ✅ correct answer: **+5 coins**, flower gains 1 HP and grows
  - ❌ wrong answer: flower loses 1 HP and wilts (0 HP = dead)
- **Shop.** Buy seedlings of new flower types (up to 6 pots) and sell grown ones for coins.
- **12 flower types, 12 subjects.** Medicine, physics, biology, ecology, chemistry, poetry, climate, classic literature, zoology, history, chemical reactions and music.
- **Localization.** Russian and English UI.
- **Sound.** Background music and sound effects for answers.

| Flower | Subject |
|---|---|
| Алоэ медицины | Медицина |
| Кактус физики | Физика |
| Папоротник биологии | Биология |
| Мандариновое дерево экологии | Экология |
| Лук химии | Химия |
| Орхидея поэзии | Поэзия |
| Пальма климата | Климат |
| Фикус классики | Классическая литература |
| Зелёный хищник | Зоология |
| Роза бессмертия | История |
| Спатифиллум реакций | Химические реакции |
| Фиалка волны | Музыка |

## Quick start (Docker)

Requirements: [Docker](https://docs.docker.com/get-docker/) with Docker Compose.

The server only needs `docker-compose.yml` and a `.env` file:

```bash
mkdir flowers && cd flowers
curl -O https://raw.githubusercontent.com/StrangeDroid/SiteDockerTest/master/docker-compose.yml
curl -o .env https://raw.githubusercontent.com/StrangeDroid/SiteDockerTest/master/.env.example
nano .env                   # set your own DB password
docker compose pull         # download the published app image
docker compose up -d
```

(Or clone the whole repository, which also lets you build the image yourself.)

Open **http://localhost:5273**.

The app image is built by GitHub Actions on every push to `master` and published publicly to the GitHub Container Registry, so no login is needed:

```
ghcr.io/strangedroid/sitedockertest:latest
```

To build the image yourself from a clone of the repository instead, run `docker compose up -d --build`.

This starts two containers:

| Service | Container | Image | Purpose |
|---|---|---|---|
| `app` | `flowers-app` | `ghcr.io/strangedroid/sitedockertest` | The Blazor web app (ports `5273`, `7041`) |
| `db` | `flowers-db` | `postgres:16-alpine` | Database, stored in the `db-data` volume |

Useful commands:

```bash
docker compose ps                 # status of both containers
docker logs -f flowers-app        # app logs
docker exec -it flowers-db psql -U flowers -d flowers_db   # database console
```

### Updating to a new version

After new changes land on `master` and the GitHub Actions run finishes:

```bash
docker compose pull         # newest app image
docker compose up -d        # recreate the app container; DB data is kept
```

To pin a specific build instead of `latest`, set `APP_TAG` in `.env` to a commit SHA (every image is also tagged with the commit it was built from).

On startup the app checks the database, and if it has no tables yet it creates them and fills in the starting data. The SQL scripts are built into the app image from `BlazorApp1/Database/`:

| File | Contents |
|---|---|
| `01-schema.sql` | Tables, keys and constraints |
| `02-seed.sql` | The 12 flower types |
| `03-questions.sql` | Test questions in Russian (3 per flower) |

> The scripts only run when the database is empty; existing data is never touched. To recreate the database from scratch (**deletes all users and progress**):
> ```bash
> docker compose down -v && docker compose up -d
> ```

### Configuration

Set in `.env` (see `.env.example`):

| Variable | Default | Description |
|---|---|---|
| `POSTGRES_DB` | `flowers_db` | Database name |
| `POSTGRES_USER` | `flowers` | Database user |
| `POSTGRES_PASSWORD` | `flowers` | Database password, **change it for production** |
| `APP_TAG` | `latest` | App image tag on ghcr.io: `latest` or a commit SHA |

The app reads its connection string from `ConnectionStrings:Default`. Compose passes it as the `ConnectionStrings__Default` environment variable.

## Local development

Requirements: [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) and Docker (for the database).

1. Expose the DB port by uncommenting `ports: - "5432:5432"` under `db` in `docker-compose.yml`, then start only the database:
   ```bash
   docker compose up -d db
   ```
2. Run the app (it uses the `localhost` connection string from `appsettings.Development.json` and creates the tables on first start):
   ```bash
   cd BlazorApp1
   dotnet run
   ```

## Adding questions

Add lines to `BlazorApp1/Database/03-questions.sql` (or create a new `04-*.sql` file next to it):

```sql
-- add_q(flower_type_id, question, correct answer, wrong 1, wrong 2, wrong 3)
SELECT pg_temp.add_q(2, 'В каких единицах измеряется сила?', 'Ньютон', 'Джоуль', 'Ватт', 'Паскаль');
```

Answers are stored in random order, so the correct one isn't always first. New questions are only added to a fresh database, so rebuild the app and recreate the database (see above).

## Database schema

```mermaid
erDiagram
    users ||--o{ authtokens : "has"
    users ||--o{ flowers : "owns"
    flowertypes ||--o{ flowers : "type of"
    flowertypes ||--o{ questions : "asks"
    questions ||--|{ answers : "has"

    users {
        int id PK
        varchar username UK
        varchar email UK
        varchar password_hash
        int coins
    }
    authtokens {
        int id PK
        int user_id FK
        varchar token UK
        timestamptz expires_at
    }
    flowertypes {
        int id PK
        varchar name
        int difficulty
        varchar code UK
        int price_buy
        int price_sell
    }
    flowers {
        int id PK
        int user_id FK
        int type_id FK
        int hp
        int pot_slot
    }
    questions {
        int id PK
        int flowertype_id FK
        text content
    }
    answers {
        int id PK
        int question_id FK
        text content
        boolean is_correct
    }
```

## Project structure

```
├── BlazorApp1/
│   ├── Data/            # Models, DB connection, schema initializer, user repository
│   ├── Database/        # SQL schema + seed data, applied to an empty DB on startup
│   ├── Pages/           # Index (auto-login), RegistrationWindow, Game
│   ├── Resources/       # ru / en localization (.resx)
│   └── wwwroot/         # CSS, JS (cookies, audio), images, sounds
├── Dockerfile           # Multi-stage build of the app image
├── docker-compose.yml   # app + PostgreSQL
└── .github/workflows/   # Builds and pushes the image to GHCR
```

## CI

GitHub Actions (`.github/workflows/docker-build.yml`) runs on every push and pull request:

- **build-and-push** builds the Docker image. On pushes to `master` it also publishes it as `ghcr.io/strangedroid/sitedockertest:latest` and `:<commit-sha>`.
- **smoke-test** starts the app and database with docker compose, checks that the database was seeded and that the site responds.

## Tech stack

- C# / .NET 7, Blazor Server
- PostgreSQL 16 via [Npgsql](https://www.npgsql.org/)
- HTML, CSS, JavaScript
- Docker, Docker Compose, GitHub Actions

## Authors

Students of group ИТ/3 ФИТ-22-НБ, Faculty of Information Technologies, Perm State University:

- Юлия Голотина
- Кирилл Вешторт
- Юлия Худанина
