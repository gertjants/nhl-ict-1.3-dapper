create table `Brewmaster` (
    BrewmasterId INT PRIMARY KEY AUTO_INCREMENT,
    Name TEXT,
    BrewerId INT REFERENCES Brewer(BrewerId),
    AddressId INT REFERENCES Address(AddressId)
);