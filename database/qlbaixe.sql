CREATE DATABASE QLBAIXE
go
USE QLBAIXE

CREATE TABLE EMPLOYEE( Id int not null IDENTITY(1,1) primary key,
					   UserName VARCHAR(50),
					   Password VARCHAR(50),
					   DisplayName nvarchar(50),
					   IdRole char(1) )

CREATE TABLE CUSTOMER( Id int not null IDENTITY(1,1)primary key,
					   Code char (12) not null,
					   DisplayName nvarchar(50),
					   Phone char(10))

CREATE TABLE INFOPARKING(Type int primary key,
						Name nvarchar(50)not null,
						Count int not null)

CREATE TABLE INFOCAR(Id int not null IDENTITY(1,1) primary key,
					 LicensePlate char(9)not null,
					 Type int not null,
					 IdEMPLOYEE int not null CONSTRAINT fk_ifcar_employee FOREIGN KEY (IdEMPLOYEE) REFERENCES EMPLOYEE(Id) ,
					 IdCUSTOMER int not null CONSTRAINT fk_ifcar_customer FOREIGN KEY (IdCUSTOMER)REFERENCES CUSTOMER(Id),
					 CheckInTime Datetime,
					 CheckOutTime Datetime)

CREATE TABLE PARKING(id int IDENTITY primary key,
					Type int not null CONSTRAINT fk_PARKING_infoPARKING FOREIGN KEY (Type) REFERENCES INFOPARKING(Type),
					IdINFOCAR int not null CONSTRAINT fk_PARKING_infocar FOREIGN KEY (IdINFOCAR) REFERENCES INFOCAR(Id))

CREATE TABLE Bill(Id int not null IDENTITY(1,1) primary key,
				  IdEMPLOYEE int not null CONSTRAINT fk_bill_employee FOREIGN KEY (IdEMPLOYEE) REFERENCES EMPLOYEE(Id) ,
				  IdINFOCAR int not null CONSTRAINT fk_bill_infocar FOREIGN KEY (IdINFOCAR) REFERENCES INFOCAR(Id),
				  Price Money)



go
INSERT INTO EMPLOYEE (UserName,Password,DisplayName,IdRole) VALUES
							('admin1','db69fc039dcbd2962cb4d28f5891aae1',N'Tạ Xuân Kiên',0),
							('staff1','978aae9bb6bee8fb75de3e4830a1be46',N'Hồ Thị Cảm Ly',1),
							('admin2','db69fc039dcbd2962cb4d28f5891aae1',N'Phùng Thị Thùy',0),
							('staff2','978aae9bb6bee8fb75de3e4830a1be46',N'Ngô Hồng Huy',1)


Create View VIEWPARKING as
Select  CT.Code, IC.LicensePlate
from PARKING as PK, INFOCAR as IC, CUSTOMER as CT
Where PK.IdINFOCAR = IC.Id and IC.IdCUSTOMER = CT.Id
