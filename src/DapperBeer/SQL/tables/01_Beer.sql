create table `Beer`
(
    BeerId  int    not null primary key,
    Name      text   null,
    Type      text   null,
    Style     text   null,
    Alcohol   double null,
    BrewerId int    null,
    constraint Beer_Brewer_BrewerId_fk
        foreign key (BrewerId) references Brewer (BrewerId)
);