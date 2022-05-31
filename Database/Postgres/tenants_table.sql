CREATE TABLE IF NOT EXISTS tenants (
    tenant_id serial PRIMARY KEY,
    creator_by INTEGER REFERENCES users (user_id),
    created_on TIMESTAMP NOT NULL,
    active BOOLEAN NOT NULL,
    disabled_on TIMESTAMP
);