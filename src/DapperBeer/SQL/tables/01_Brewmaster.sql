drop table if exists `DapperBeer`.`Brewmaster`;

create table `DapperBeer`.`Brewmaster` (
    BrewmasterId INT PRIMARY KEY AUTO_INCREMENT,
    Name TEXT,
    BrewerId INT REFERENCES Brewer(BrewerId),
    AddressId INT REFERENCES Address(AddressId)
);