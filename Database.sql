-- =====================================
-- SCRIPT SQL - MODELO ER (MySQL 8+)
-- =====================================

-- ======================
-- USUARIO
-- ======================
CREATE TABLE usuario (
  id CHAR(36) PRIMARY KEY,
  nombre VARCHAR(100),
  email VARCHAR(150) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  timezone VARCHAR(50) DEFAULT 'America/Managua',
  locale VARCHAR(10) DEFAULT 'es',
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  actualizado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- ======================
-- MATERIA
-- ======================
CREATE TABLE materia (
  id CHAR(36) PRIMARY KEY,
  usuario_id CHAR(36) NOT NULL,
  nombre VARCHAR(100) NOT NULL,
  estado ENUM('activo','eliminada') DEFAULT 'activo',
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  actualizado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_materia_usuario_nombre (usuario_id, nombre),
  CONSTRAINT fk_materia_usuario FOREIGN KEY (usuario_id)
    REFERENCES usuario(id) ON DELETE CASCADE
);

-- ======================
-- TAREA
-- ======================
CREATE TABLE tarea (
  id CHAR(36) PRIMARY KEY,
  materia_id CHAR(36) NOT NULL,
  titulo VARCHAR(150) NOT NULL,
  descripcion TEXT,
  vence_en DATETIME NOT NULL,
  prioridad ENUM('Alta','Media','Baja') NOT NULL DEFAULT 'Media',
  completada BOOLEAN DEFAULT FALSE,
  silenciada BOOLEAN DEFAULT FALSE,
  eliminada BOOLEAN DEFAULT FALSE,
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  actualizado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_tarea_materia FOREIGN KEY (materia_id)
    REFERENCES materia(id) ON DELETE CASCADE
);

-- ======================
-- RECORDATORIO_TAREA
-- ======================
CREATE TABLE recordatorio_tarea (
  id CHAR(36) PRIMARY KEY,
  tarea_id CHAR(36) NOT NULL,
  minutos_antes INT NOT NULL CHECK (minutos_antes > 0),
  activo BOOLEAN DEFAULT TRUE,
  enviado_en DATETIME NULL,
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_recordatorio_tarea FOREIGN KEY (tarea_id)
    REFERENCES tarea(id) ON DELETE CASCADE
);

-- ======================
-- ETIQUETA
-- ======================
CREATE TABLE etiqueta (
  id CHAR(36) PRIMARY KEY,
  usuario_id CHAR(36) NOT NULL,
  nombre VARCHAR(100) NOT NULL,
  color_hex CHAR(7) DEFAULT NULL,
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  actualizado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_etiqueta_usuario_nombre (usuario_id, nombre),
  CONSTRAINT fk_etiqueta_usuario FOREIGN KEY (usuario_id)
    REFERENCES usuario(id) ON DELETE CASCADE
);

-- ======================
-- TAREA_ETIQUETA (N:M)
-- ======================
CREATE TABLE tarea_etiqueta (
  tarea_id CHAR(36) NOT NULL,
  etiqueta_id CHAR(36) NOT NULL,
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (tarea_id, etiqueta_id),
  CONSTRAINT fk_te_tarea FOREIGN KEY (tarea_id)
    REFERENCES tarea(id) ON DELETE CASCADE,
  CONSTRAINT fk_te_etiqueta FOREIGN KEY (etiqueta_id)
    REFERENCES etiqueta(id) ON DELETE CASCADE
);

-- ======================
-- ÍNDICES OPCIONALES
-- ======================
CREATE INDEX idx_materia_estado ON materia (usuario_id, estado);
CREATE INDEX idx_tarea_filtros ON tarea (materia_id, completada, prioridad, vence_en);
CREATE INDEX idx_tarea_vence_en ON tarea (vence_en);
CREATE INDEX idx_etiqueta_usuario ON etiqueta (usuario_id, nombre);
CREATE INDEX idx_te_relacion ON tarea_etiqueta (etiqueta_id, tarea_id);

-- ======================
-- OPCIONAL: FULLTEXT (MySQL >= 8.0.17)
-- ======================
-- Permite búsqueda por texto (RF5.1)
CREATE FULLTEXT INDEX ft_tarea_busqueda ON tarea (titulo, descripcion);

