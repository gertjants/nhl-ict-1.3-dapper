drop table if exists `DapperBeer`.`Address`;

create table `DapperBeer`.`Address` (
    AddressId INT PRIMARY KEY AUTO_INCREMENT,
    Street TEXT,
    City TEXT,
    Country TEXT
);