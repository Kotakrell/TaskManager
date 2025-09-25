CREATE TABLE notification_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT
);
CREATE TABLE notifications (
    id SERIAL PRIMARY KEY,
    id_user INT NOT NULL,
    id_type INT REFERENCES notification_types(id) ON DELETE CASCADE ON UPDATE CASCADE,
	id_task INT NOT NULL,
    message TEXT NOT NULL,
    create_time TIMESTAMP DEFAULT NOW()
);
CREATE TABLE notification_read_status (
    id SERIAL PRIMARY KEY,
    notification_id INT NOT NULL REFERENCES notifications(id) ON DELETE CASCADE ON UPDATE CASCADE,
    read_at TIMESTAMP
);
INSERT INTO notification_types(name)
VALUES ('Новая задача'),
	   ('Задача изменена'),
	   ('Задача удалена');