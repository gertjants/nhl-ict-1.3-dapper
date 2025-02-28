create table IF NOT EXISTS Review
(
    ReviewId int auto_increment primary key,
    BeerId   int           null,
    Score    decimal(4, 2) null,
    constraint Review_ibfk_1
        foreign key (BeerId) references DapperBeer.Beer (BeerId)
);