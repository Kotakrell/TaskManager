CREATE TABLE users (
	id SERIAL PRIMARY KEY,
	name VARCHAR(100) NOT NULL,
	creation_date TIMESTAMP DEFAULT NOW()
);
CREATE TABLE task_statuses (
	id SERIAL PRIMARY KEY,
	name VARCHAR(30) NOT NULL UNIQUE
);
CREATE TABLE tasks (
	id SERIAL PRIMARY KEY,
	title VARCHAR(200) NOT NULL,
	description TEXT,
	id_status INT NOT NULL REFERENCES task_statuses(id) ON DELETE CASCADE ON UPDATE CASCADE,
	create_time TIMESTAMP DEFAULT NOW()
);
CREATE TABLE task_history (
	id SERIAL PRIMARY KEY,
	id_task INT NOT NULL REFERENCES tasks(id) ON DELETE CASCADE ON UPDATE CASCADE,
	id_old_status INT REFERENCES task_statuses(id),
	id_new_status INT REFERENCES task_statuses(id),
	change_time TIMESTAMP DEFAULT NOW()
);
CREATE TABLE task_assignments (
	id SERIAL PRIMARY KEY,
	id_task INT NOT NULL REFERENCES tasks(id) ON DELETE CASCADE ON UPDATE CASCADE,
	id_user INT NOT NULL REFERENCES users(id) ON DELETE CASCADE ON UPDATE CASCADE,
	assigne_time TIMESTAMP DEFAULT NOW()
);

INSERT INTO users(name)
VALUES ('Пользователь1'),
	   ('Пользователь2'),
	   ('Пользователь3');
INSERT INTO task_statuses(name)
VALUES ('Активен'),
	   ('Удален');