-- Applied by the app on startup when the database has no tables (see Data/DatabaseInitializer.cs).

CREATE TABLE users (
    id            SERIAL PRIMARY KEY,
    username      VARCHAR(50)  NOT NULL UNIQUE,
    email         VARCHAR(100) NOT NULL CHECK (email <> ''),
    password_hash VARCHAR(255) NOT NULL,
    coins         INTEGER      NOT NULL DEFAULT 0
);
-- Emails are unique regardless of case (Test@Mail.ru = test@mail.ru)
CREATE UNIQUE INDEX ux_users_email ON users (LOWER(email));

CREATE TABLE authtokens (
    id         SERIAL PRIMARY KEY,
    user_id    INTEGER      NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token      VARCHAR(255) NOT NULL UNIQUE,
    -- timestamptz (not plain timestamp): the app writes DateTime.UtcNow, which Npgsql only accepts for timestamptz
    expires_at TIMESTAMPTZ  NOT NULL
);
CREATE INDEX ix_authtokens_user_id ON authtokens(user_id);

CREATE TABLE flowertypes (
    id         SERIAL PRIMARY KEY,
    name       VARCHAR(100) NOT NULL,
    difficulty INTEGER      NOT NULL DEFAULT 1,
    code       VARCHAR(50)  NOT NULL UNIQUE, -- folder name in wwwroot/images/flowers
    price_buy  INTEGER      NOT NULL DEFAULT 0 CHECK (price_buy >= 0),
    price_sell INTEGER      NOT NULL DEFAULT 0 CHECK (price_sell >= 0)
);

CREATE TABLE flowers (
    id       SERIAL PRIMARY KEY,
    user_id  INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    type_id  INTEGER NOT NULL REFERENCES flowertypes(id),
    hp       INTEGER NOT NULL DEFAULT 15,
    pot_slot INTEGER NOT NULL
);
CREATE INDEX ix_flowers_user_id ON flowers(user_id);

CREATE TABLE questions (
    id            SERIAL PRIMARY KEY,
    flowertype_id INTEGER NOT NULL REFERENCES flowertypes(id) ON DELETE CASCADE,
    content       TEXT    NOT NULL
);
CREATE INDEX ix_questions_flowertype_id ON questions(flowertype_id);

CREATE TABLE answers (
    id          SERIAL PRIMARY KEY,
    question_id INTEGER NOT NULL REFERENCES questions(id) ON DELETE CASCADE,
    content     TEXT    NOT NULL,
    is_correct  BOOLEAN NOT NULL DEFAULT FALSE
);
CREATE INDEX ix_answers_question_id ON answers(question_id);
