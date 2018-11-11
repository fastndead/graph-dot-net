--главный скрипт для таблиц и базы данных на postgres

CREATE SCHEMA IF NOT EXISTS graph_scheme;
  
CREATE TABLE IF NOT EXISTS graph_scheme.t_nodes(
  c_id int primary key,
  c_name varchar(18) unique ,
  c_group int
);

CREATE TABLE IF NOT EXISTS graph_scheme.t_links(
  c_id int primary key,
  c_source varchar(18),
  c_target varchar(18),
  c_value int
);

ALTER TABLE graph_scheme.t_links
  ADD CONSTRAINT t_links_t_nodes_c_source_fk
FOREIGN KEY (c_source) REFERENCES graph_scheme.t_nodes (c_name);

ALTER TABLE graph_scheme.t_links
  ADD CONSTRAINT t_links_t_nodes_c_target_fk
FOREIGN KEY (c_target) REFERENCES graph_scheme.t_nodes (c_name);

CREATE SEQUENCE IF NOT EXISTS graph_scheme.nodes_seq START 1;
CREATE SEQUENCE IF NOT EXISTS graph_scheme.links_seq START 1;
