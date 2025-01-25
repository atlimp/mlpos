ALTER TABLE POSCLIENT ADD IF NOT EXISTS logincode varchar(64) NOT NULL DEFAULT '';

ALTER TABLE POSCLIENT DROP CONSTRAINT IF EXISTS C_POSCLIENT_UNIQUE_LOGINCODE;
ALTER TABLE POSCLIENT ADD CONSTRAINT C_POSCLIENT_UNIQUE_LOGINCODE UNIQUE(logincode);

CREATE TABLE IF NOT EXISTS MLUSER(
    id SERIAL PRIMARY KEY,
    username varchar(100),
    hashed_password varchar(1000),
    email varchar(200),
    image varchar(500) DEFAULT '',
    date_inserted timestamp DEFAULT CURRENT_TIMESTAMP,
    date_updated timestamp DEFAULT CURRENT_TIMESTAMP
);

CREATE OR REPLACE TRIGGER t_MLUSER_UPDATE BEFORE UPDATE ON MLUSER FOR EACH ROW EXECUTE PROCEDURE fn_update_date_updated();

ALTER TABLE MLUSER DROP CONSTRAINT IF EXISTS C_MLUSER_UNIQUE_USERNAME;
ALTER TABLE MLUSER ADD CONSTRAINT C_MLUSER_UNIQUE_USERNAME UNIQUE(username);

ALTER TABLE PRODUCT ADD IF NOT EXISTS date_deleted timestamp DEFAULT null;
ALTER TABLE CUSTOMER ADD IF NOT EXISTS date_deleted timestamp DEFAULT null;
ALTER TABLE PAYMENTMETHOD ADD IF NOT EXISTS date_deleted timestamp DEFAULT null;
ALTER TABLE POSCLIENT ADD IF NOT EXISTS date_deleted timestamp DEFAULT null;
ALTER TABLE MLUSER ADD IF NOT EXISTS date_deleted timestamp DEFAULT null;

ALTER TABLE PRODUCT ADD IF NOT EXISTS visible_on_pos boolean DEFAULT TRUE;
ALTER TABLE CUSTOMER ADD IF NOT EXISTS visible_on_pos boolean DEFAULT TRUE;
ALTER TABLE PAYMENTMETHOD ADD IF NOT EXISTS visible_on_pos boolean DEFAULT TRUE;
ALTER TABLE POSCLIENT ADD IF NOT EXISTS visible_on_pos boolean DEFAULT TRUE;
ALTER TABLE MLUSER ADD IF NOT EXISTS visible_on_pos boolean DEFAULT TRUE;

CREATE TABLE IF NOT EXISTS INVENTORYTRANSACTION(
    type int DEFAULT 0,
    transaction_id bigint NULL,
    posclient_id bigint NULL,
    line_id bigint NULL,
    product_id bigint REFERENCES PRODUCT(id),
    quantity decimal,
    date_inserted timestamp DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(transaction_id, posclient_id, line_id) REFERENCES POSTEDTRANSACTIONLINE(transaction_id, posclient_id, id)
);

CREATE OR REPLACE VIEW INVENTORYBALANCES AS (
    WITH LastKnownBalance AS (
    SELECT t1.quantity last_balance, t1.product_id, t1.date_inserted last_balance_date FROM inventorytransaction t1 JOIN (
	    SELECT t2.product_id, max(t2.date_inserted) timestamp
		    FROM inventorytransaction t2
		    WHERE type = 1 GROUP BY t2.product_id) max
	    ON t1.product_id = max.product_id AND t1.date_inserted = max.timestamp
	    WHERE type = 1
    ),
    TransactionsAfterReset AS (
        SELECT
            t.product_id,
            SUM(t.quantity) AS transaction_sum
        FROM 
            INVENTORYTRANSACTION t
        JOIN 
            LastKnownBalance lkb
            ON t.product_id = lkb.product_id
            AND t.date_inserted > lkb.last_balance_date
        GROUP BY
            t.product_id
    )
    SELECT
        lkb.product_id,
        (lkb.last_balance - COALESCE(ta.transaction_sum, 0)) AS current_balance
    FROM 
        LastKnownBalance lkb
    LEFT JOIN 
        TransactionsAfterReset ta
        ON lkb.product_id = ta.product_id
);

INSERT INTO INVENTORYTRANSACTION(type, product_id, quantity)
    SELECT 1 type, id product_id, 0 quantity FROM product WHERE type = 0 AND id NOT IN (SELECT DISTINCT product_Id FROM INVENTORYTRANSACTION);

CREATE OR REPLACE FUNCTION fn_create_inventorytransaction()
    RETURNS TRIGGER AS $$
BEGIN
    IF NEW.type = 0 THEN
        INSERT INTO INVENTORYTRANSACTION(type, product_id, quantity) VALUES(1, NEW.id, 0);
    END IF;
RETURN NEW;
END;
$$ language 'plpgsql';

CREATE OR REPLACE TRIGGER t_PRODUCT_INVENTORY_TRANSACTION_INSERT AFTER INSERT ON PRODUCT FOR EACH ROW EXECUTE PROCEDURE fn_create_inventorytransaction();

CREATE TABLE IF NOT EXISTS INVOICEHEADER(
    id SERIAL PRIMARY KEY,
    status int DEFAULT 0,
    customer_id bigint REFERENCES CUSTOMER(id),
    paymentmethod_id bigint REFERENCES PAYMENTMETHOD(id),
    period_from timestamp,
    period_to timestamp,
    date_inserted timestamp DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS INVOICELINE(
    id SERIAL,
    invoice_id bigint REFERENCES INVOICEHEADER(id),
    product_id bigint REFERENCES PRODUCT(id),
    quantity decimal DEFAULT 0,
    amount decimal DEFAULT 0,
    date_inserted timestamp DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (invoice_id, id)
);

ALTER TABLE POSTEDTRANSACTIONHEADER ADD IF NOT EXISTS invoice_id bigint NULL REFERENCES INVOICEHEADER(id);

ALTER TABLE PAYMENTMETHOD ADD IF NOT EXISTS invoice_on_post boolean DEFAULT FALSE;
