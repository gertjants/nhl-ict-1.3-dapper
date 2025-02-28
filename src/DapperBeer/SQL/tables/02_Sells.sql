create table Sells
(
    CafeId int not null,
    BeerId  int not null,
    primary key (CafeId, BeerId),
    constraint Sells_Beer_BeerId_fk
        foreign key (BeerId) references Beer (BeerId),
    constraint Sells_Cafe_CafeId_fk
        foreign key (CafeId) references Cafe (CafeId)
);