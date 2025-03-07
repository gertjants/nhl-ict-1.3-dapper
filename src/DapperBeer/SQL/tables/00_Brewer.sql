drop table if exists `DapperBeer`.`Brewer`;

create table `DapperBeer`.`Brewer`
(
    BrewerId int  not null primary key,
    Name text null,
    Country text null
);