CREATE DATABASE  IF NOT EXISTS `test_books` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `test_books`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: test_books
-- ------------------------------------------------------
-- Server version	5.7.19-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `book_relations`
--

DROP TABLE IF EXISTS `book_relations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `book_relations` (
  `idbook_relations` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `idbook1_relations` int(10) unsigned NOT NULL,
  `idbook2_relations` int(10) unsigned NOT NULL,
  PRIMARY KEY (`idbook_relations`),
  UNIQUE KEY `idbook_relations_UNIQUE` (`idbook_relations`),
  KEY `book1_relations_idx` (`idbook1_relations`),
  KEY `book2_relations_idx` (`idbook2_relations`),
  CONSTRAINT `book1_relations` FOREIGN KEY (`idbook1_relations`) REFERENCES `books` (`idbooks`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `book2_relations` FOREIGN KEY (`idbook2_relations`) REFERENCES `books` (`idbooks`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `book_relations`
--

LOCK TABLES `book_relations` WRITE;
/*!40000 ALTER TABLE `book_relations` DISABLE KEYS */;
/*!40000 ALTER TABLE `book_relations` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-09-25 13:51:35
