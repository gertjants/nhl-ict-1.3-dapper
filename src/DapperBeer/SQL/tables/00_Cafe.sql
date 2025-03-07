drop table if exists `DapperBeer`.`Cafe`;

create table `DapperBeer`.`Cafe`
(
    CafeId int  not null
        primary key,
    Name        text null,
    Address     text null,
    City        text null
);