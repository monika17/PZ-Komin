/* dodanie nowej bazy */
create database KominServerDatabase
go

/* przejscie do bazy o zadanej nazwie */
use KominServerDatabase
go

/* definicja nowego typu danych */
sp_addtype string, 'varchar(250)'
go
sp_addtype string1000, 'varchar(1000)'
go


/* dodanie tabel */
/* tabela Konta */
create table konta
	(id_konta int not null primary key,
	nazwa string not null,
	haslo varchar(15) not null,
	status_konta int not null,
	lista_kontaktow string)
go

/* tabeli kontakty */
/*
create table kontakty
	(id_kontaktu int not null foreign key references konta(id_konta))
go
*/

/* tabeli grupy */
create table grupy
	(id_grupy int not null primary key,
	nazwa_grupy string,
	id_zalozyciela int foreign key references konta(id_konta),
	rodzaj_komunikacji int not null,
	kontakty_nazwa string not null)	
go

create table pliki_grup
	(id_pliku int not null primary key,
	œcie¿ka string not null,
	rozmiar int,
	id_nadawcy int foreign key references konta(id_konta),
	id_grupy int foreign key references grupy(id_grupy),
	czas_wyslania int,
	data_wyslania int,
	data_konca datetime)
go

create table oczekuj¹ce_wiadomoœci
	(id_wiadomosci int not null primary key,
	id_nadawcy int foreign key references konta(id_konta),
	id_docelowy int,
	czy_grupowy bit,
	tresc_wiadomoci string1000 not null,
	data_czas_wyslania datetime)
go



/* uzupe³nienie tabel danymi */
/* tabela konta */
/*
insert into konta values (1, 'Monika', 'ja12345','dostepny')
insert into konta values (2, 'Monia', '12345','dostepny')
insert into konta values (3, 'Jakub', 'blabla','dostepny')
insert into konta values (4, 'Marta', 'aaa','zaraz wracam')
insert into konta values (5, 'Rybka', 'rybka','dostepny')
insert into konta values (6, 'bolek', 'nie/tak','dostepny')
insert into konta values (7, 'bibi', 'ibib','niewidoczny')
insert into konta values (8, 'lala134', '4','niedostepny')
go

select * from konta
go*/

/*tabela kontakty*/
/*
insert into  kontakty values(1, 'Monika')
insert into  kontakty values(2, 'Monia')
insert into  kontakty values(3, 'Jakub')
insert into  kontakty values(4, 'Marta')
insert into  kontakty values(5, 'Rybka')
insert into  kontakty values(6, 'bolek')
insert into  kontakty values(7, 'bibi')
insert into  kontakty values(8, 'lala134')
go

select * from kontakty
go*/
 

/*tabela grupy*/
/*insert into grupy values(1,'praca' ,1, 'Monika')
insert into grupy values(2,'projekt', 3,'Jakub')
insert into grupy values(3,'ploty', 7, 'bibi')
go

select * from grupy
go*/


