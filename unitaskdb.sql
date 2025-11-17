-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: unitask
-- ------------------------------------------------------
-- Server version	8.0.36

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `etiqueta`
--

DROP TABLE IF EXISTS `etiqueta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `etiqueta` (
  `id` char(36) NOT NULL,
  `usuario_id` char(36) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `color_hex` char(7) DEFAULT NULL,
  `creado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `actualizado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_etiqueta_usuario_nombre` (`usuario_id`,`nombre`),
  KEY `idx_etiqueta_usuario` (`usuario_id`,`nombre`),
  CONSTRAINT `fk_etiqueta_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `etiqueta`
--

LOCK TABLES `etiqueta` WRITE;
/*!40000 ALTER TABLE `etiqueta` DISABLE KEYS */;
/*!40000 ALTER TABLE `etiqueta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `materia`
--

DROP TABLE IF EXISTS `materia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `materia` (
  `id` char(36) NOT NULL,
  `usuario_id` char(36) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `estado` enum('activo','eliminada') DEFAULT 'activo',
  `creado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `actualizado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_materia_usuario_nombre` (`usuario_id`,`nombre`),
  KEY `idx_materia_estado` (`usuario_id`,`estado`),
  CONSTRAINT `fk_materia_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `materia`
--

LOCK TABLES `materia` WRITE;
/*!40000 ALTER TABLE `materia` DISABLE KEYS */;
INSERT INTO `materia` VALUES ('11b1c4c1-7018-4565-b111-e7783f807765','7664e240-462a-45d2-8533-4b7b6fa92396','Matematica','eliminada','2025-11-16 12:09:54','2025-11-16 12:15:04'),('1f609f72-952e-474e-aaee-2e5c36728495','9792a30a-23b4-4eac-914c-0ac5c77a0d7e','Matematica','activo','2025-10-13 23:39:53','2025-10-13 23:39:53'),('210cead6-765b-4f83-a9ce-958abb73776b','7664e240-462a-45d2-8533-4b7b6fa92396','Calculo 1','eliminada','2025-11-16 13:02:02','2025-11-16 13:04:02'),('22caab53-80c3-4d8f-8f1b-dbec3dfe8153','9792a30a-23b4-4eac-914c-0ac5c77a0d7e','Historia de Nicaragua','activo','2025-10-13 23:40:14','2025-10-13 23:40:14'),('24ecd14b-76d5-4316-8842-7ec7aa72dd1f','9792a30a-23b4-4eac-914c-0ac5c77a0d7e','Mamitas Puebla','activo','2025-10-13 23:40:22','2025-10-13 23:40:22'),('2b0f8020-9d79-42a4-898b-a2d555532487','9792a30a-23b4-4eac-914c-0ac5c77a0d7e','Lengua y Literatura','activo','2025-10-13 23:39:57','2025-10-13 23:39:57'),('2f703562-86cc-4151-91ef-10876fb25d06','4852474a-fb6f-43ca-a3b6-fa2f530c29cc','ASdasdasd','eliminada','2025-11-18 00:35:03','2025-11-18 01:00:44'),('3a47e71e-802e-4b01-ab2c-f304d8cee986','9792a30a-23b4-4eac-914c-0ac5c77a0d7e','Estudios Sociales','activo','2025-10-13 23:40:04','2025-10-13 23:40:04'),('411f8568-d2f7-45df-859a-f2c380454f75','7664e240-462a-45d2-8533-4b7b6fa92396','caa','eliminada','2025-11-16 12:30:22','2025-11-16 12:30:43'),('71731466-7d17-4840-bc6d-1ead79ede0f2','9792a30a-23b4-4eac-914c-0ac5c77a0d7e','Biologia general','activo','2025-10-13 23:40:09','2025-10-13 23:40:09'),('7697c93a-6855-4906-b7b8-b9ca2c864a58','9792a30a-23b4-4eac-914c-0ac5c77a0d7e','JAJAJAJAJ','activo','2025-10-13 23:40:25','2025-10-13 23:40:25'),('8d7ec254-57d5-4bd8-8d42-cee6cf133797','4852474a-fb6f-43ca-a3b6-fa2f530c29cc','matematica','activo','2025-10-09 12:20:58','2025-10-09 12:20:58'),('9e691b50-0fcc-4369-a3ed-6d1da77fe2bd','4852474a-fb6f-43ca-a3b6-fa2f530c29cc','MateriaJuajuajua','eliminada','2025-10-09 23:06:47','2025-11-16 13:14:13'),('b6657a20-bd78-46da-8082-3bbbc194ef30','4852474a-fb6f-43ca-a3b6-fa2f530c29cc','Español','activo','2025-11-18 01:02:12','2025-11-18 01:02:12'),('bfab901c-83cd-40b2-b73b-795e3d984bcd','4852474a-fb6f-43ca-a3b6-fa2f530c29cc','Lengua y Literatura','eliminada','2025-10-09 12:40:28','2025-11-18 00:34:56'),('cbc69098-448d-46f8-a930-06401d7604a3','7664e240-462a-45d2-8533-4b7b6fa92396','ca','eliminada','2025-11-16 12:32:35','2025-11-16 12:32:38'),('ef667b46-9211-4431-a5e1-a40568ae2d7f','7664e240-462a-45d2-8533-4b7b6fa92396','e','eliminada','2025-11-16 12:46:58','2025-11-16 12:47:02'),('f91a3748-2308-47c5-ae59-be69d3d3832a','de13876d-b56f-4ff7-ad91-27f54e0bfc7b','matematicas','activo','2025-10-18 07:00:00','2025-10-18 07:00:00');
/*!40000 ALTER TABLE `materia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recordatorio_tarea`
--

DROP TABLE IF EXISTS `recordatorio_tarea`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recordatorio_tarea` (
  `id` char(36) NOT NULL,
  `tarea_id` char(36) NOT NULL,
  `minutos_antes` int NOT NULL,
  `activo` tinyint(1) DEFAULT '1',
  `enviado_en` datetime DEFAULT NULL,
  `creado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_recordatorio_tarea` (`tarea_id`),
  CONSTRAINT `fk_recordatorio_tarea` FOREIGN KEY (`tarea_id`) REFERENCES `tarea` (`id`) ON DELETE CASCADE,
  CONSTRAINT `recordatorio_tarea_chk_1` CHECK ((`minutos_antes` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recordatorio_tarea`
--

LOCK TABLES `recordatorio_tarea` WRITE;
/*!40000 ALTER TABLE `recordatorio_tarea` DISABLE KEYS */;
INSERT INTO `recordatorio_tarea` VALUES ('453665e5-febb-45f1-8680-f64e14bb6d89','0a826e72-0a9e-4039-8327-834670251cd1',30,1,NULL,'2025-11-18 00:36:06');
/*!40000 ALTER TABLE `recordatorio_tarea` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tarea`
--

DROP TABLE IF EXISTS `tarea`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tarea` (
  `id` char(36) NOT NULL,
  `materia_id` char(36) NOT NULL,
  `titulo` varchar(150) NOT NULL,
  `descripcion` text,
  `vence_en` datetime NOT NULL,
  `prioridad` enum('Alta','Media','Baja') NOT NULL DEFAULT 'Media',
  `completada` tinyint(1) DEFAULT '0',
  `silenciada` tinyint(1) DEFAULT '0',
  `eliminada` tinyint(1) DEFAULT '0',
  `creado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `actualizado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_tarea_filtros` (`materia_id`,`completada`,`prioridad`,`vence_en`),
  KEY `idx_tarea_vence_en` (`vence_en`),
  FULLTEXT KEY `ft_tarea_busqueda` (`titulo`,`descripcion`),
  CONSTRAINT `fk_tarea_materia` FOREIGN KEY (`materia_id`) REFERENCES `materia` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tarea`
--

LOCK TABLES `tarea` WRITE;
/*!40000 ALTER TABLE `tarea` DISABLE KEYS */;
INSERT INTO `tarea` VALUES ('090e062c-5368-441e-a2e6-b8db28d3b2e6','f91a3748-2308-47c5-ae59-be69d3d3832a','asdasdasd','asdasdasd','2025-10-17 19:02:00','Alta',1,0,0,'2025-10-18 07:03:00','2025-10-18 07:03:03'),('0a826e72-0a9e-4039-8327-834670251cd1','2f703562-86cc-4151-91ef-10876fb25d06','Tarea adsasdasd','tarea','2025-11-18 12:35:00','Alta',1,0,1,'2025-11-18 00:35:59','2025-11-17 18:36:13'),('1d163a6f-21d0-42b9-9677-e57ca6d676c7','bfab901c-83cd-40b2-b73b-795e3d984bcd','Tarea de mañana','Tarea','2025-11-13 07:00:00','Alta',0,0,1,'2025-11-13 02:54:01','2025-11-16 06:30:16'),('48468b3a-f0f8-40c6-943f-dd882a82a998','bfab901c-83cd-40b2-b73b-795e3d984bcd','asd','asd','2025-10-09 01:24:00','Media',0,0,1,'2025-10-09 13:22:21','2025-10-09 07:32:58'),('4bde613a-3231-444d-91b8-0372e1f69186','bfab901c-83cd-40b2-b73b-795e3d984bcd','Exposicion','asdasdasd','2025-10-04 00:42:00','Baja',0,0,1,'2025-10-09 12:42:25','2025-10-09 07:32:48'),('4c3b9dac-5b66-4042-9d19-7d1da2f2b777','bfab901c-83cd-40b2-b73b-795e3d984bcd','juan','asd','2025-11-16 01:21:00','Media',0,0,1,'2025-11-16 13:21:57','2025-11-17 18:35:30'),('7cb3c0f1-46f5-4aa4-92c5-23bd11f0bdf3','8d7ec254-57d5-4bd8-8d42-cee6cf133797','asdasd','asdasd','2025-11-01 00:28:00','Alta',0,0,1,'2025-10-09 12:29:03','2025-10-09 07:33:28'),('85dc656f-333f-4114-af09-4d9a599f5b88','b6657a20-bd78-46da-8082-3bbbc194ef30','Tarea','tarea','2025-11-17 13:05:00','Media',0,0,0,'2025-11-18 01:05:52','2025-11-18 01:06:45'),('a62a7b72-8dd9-4f82-b90b-f9d7807aae8c','f91a3748-2308-47c5-ae59-be69d3d3832a','tarea de mañana','descripcion','2025-10-18 11:00:00','Baja',1,0,0,'2025-10-18 07:00:24','2025-10-18 07:03:04'),('bb229b29-acdf-473c-9fe7-698f228ac183','9e691b50-0fcc-4369-a3ed-6d1da77fe2bd','tareaXD','asdasdasd','2025-10-10 01:07:00','Media',1,0,1,'2025-10-09 23:07:09','2025-10-18 00:58:04'),('c0aad80b-76a0-4720-b464-747f1e5d6a6c','8d7ec254-57d5-4bd8-8d42-cee6cf133797','tarea de manana','asdasdasd','2025-10-17 03:25:00','Media',0,0,1,'2025-10-09 12:25:12','2025-10-09 07:33:03'),('dc6692d2-3fba-4d53-957b-657fda1f2eea','bfab901c-83cd-40b2-b73b-795e3d984bcd','prueba','asdasd','2025-10-09 01:18:00','Alta',0,0,1,'2025-10-09 13:16:22','2025-10-09 07:32:55'),('faffd594-24c1-4676-a821-146ff7e44e33','bfab901c-83cd-40b2-b73b-795e3d984bcd','tarea','','2025-11-16 01:23:00','Media',0,0,1,'2025-11-16 13:21:41','2025-11-17 18:35:32');
/*!40000 ALTER TABLE `tarea` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tarea_etiqueta`
--

DROP TABLE IF EXISTS `tarea_etiqueta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tarea_etiqueta` (
  `tarea_id` char(36) NOT NULL,
  `etiqueta_id` char(36) NOT NULL,
  `creado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`tarea_id`,`etiqueta_id`),
  KEY `idx_te_relacion` (`etiqueta_id`,`tarea_id`),
  CONSTRAINT `fk_te_etiqueta` FOREIGN KEY (`etiqueta_id`) REFERENCES `etiqueta` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_te_tarea` FOREIGN KEY (`tarea_id`) REFERENCES `tarea` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tarea_etiqueta`
--

LOCK TABLES `tarea_etiqueta` WRITE;
/*!40000 ALTER TABLE `tarea_etiqueta` DISABLE KEYS */;
/*!40000 ALTER TABLE `tarea_etiqueta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `id` char(36) NOT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `email` varchar(150) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `timezone` varchar(50) DEFAULT 'America/Managua',
  `locale` varchar(10) DEFAULT 'es',
  `creado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `actualizado_en` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `auth_provider` enum('local','google') NOT NULL DEFAULT 'local',
  `google_sub` varchar(255) DEFAULT NULL,
  `email_verified` tinyint(1) NOT NULL DEFAULT '0',
  `rol` enum('usuario','admin') NOT NULL DEFAULT 'usuario',
  `last_login` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`),
  UNIQUE KEY `google_sub` (`google_sub`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES ('1c7d8ad4-673c-4fc5-b350-3c31d27d79ea','Juan Andres Martinez Melendez','johnantonycina4@gmail.com','$2a$11$UyKGzGn.UHr2EFh4wdrWv.q9G6OspllT1QHzlBAMedvPmd5.qcTUC','America/Managua','es','2025-11-18 00:38:24','2025-11-18 00:38:24','local',NULL,0,'usuario','2025-11-17 18:38:24'),('4852474a-fb6f-43ca-a3b6-fa2f530c29cc','Diego Mairena','diegorafaelmairena04@gmail.com','','America/Managua','es','2025-10-09 11:42:48','2025-11-18 01:00:40','google','104488736549637388496',1,'admin','2025-11-17 19:00:40'),('7664e240-462a-45d2-8533-4b7b6fa92396','Juan Andres Perez Miranda','johnantonycena4@gmail.com','$2a$11$SxB.okZKHMrlvrdXiaMoZeeDn0xR/BqeMVEDXKH8EcirXSRrRveWW','America/Managua','es','2025-11-16 12:00:09','2025-11-16 13:15:25','google','111477748510871115292',1,'usuario','2025-11-16 07:15:25'),('9792a30a-23b4-4eac-914c-0ac5c77a0d7e','Diego Juejuejue','diegojuejuejue@gmail.com','','America/Managua','es','2025-10-09 13:33:42','2025-11-16 13:14:42','google','108823500773292819109',1,'admin','2025-11-16 07:14:42'),('de13876d-b56f-4ff7-ad91-27f54e0bfc7b','Fabian Gonzales','pe290626@gmail.com','','America/Managua','es','2025-10-18 06:59:43','2025-10-18 06:59:43','google','109388248268346204347',1,'usuario','2025-10-18 00:59:43');
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-17 14:08:15
