CREATE TABLE erc_20_transfers
(
    network          TEXT    NOT NULL,
    block_hash       TEXT    NOT NULL,
    transaction_hash TEXT    NOT NULL,
    log_index        TEXT    NOT NULL,
    update_time_ms   INTEGER NOT NULL,
    block_number     TEXT    NOT NULL,
    token_address    TEXT    NOT NULL,
    from_address     TEXT    NOT NULL,
    to_address       TEXT    NOT NULL,
    value            TEXT    NOT NULL,
    PRIMARY KEY (network, block_hash, transaction_hash, log_index)
);

CREATE TABLE blockchain_listen_progresses
(
    network              TEXT NOT NULL,
    current_block_number TEXT NOT NULL,
    PRIMARY KEY (network)
);
