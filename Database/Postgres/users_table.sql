CREATE TABLE IF NOT EXISTS users (
    user_id serial PRIMARY KEY,
    email VARCHAR (255) UNIQUE NOT NULL,
    password VARCHAR (50) NOT NULL,
    salt VARCHAR (50) NOT NULL,
    created_on TIMESTAMP NOT NULL,
    last_login TIMESTAMP,
    active BOOLEAN NOT NULL,
    disabled_on TIMESTAMP
);