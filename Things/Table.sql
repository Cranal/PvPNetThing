-- DB name pvpnetthing
create table pvpuser
(
  userkey      serial      not null
    constraint user_pkey
    primary key,
  username     varchar(50) not null,
  userpassword bytea       not null,
  hashalgor    integer
);

create unique index user_userkey_uindex
  on pvpuser (userkey);

create unique index user_username_uindex
  on pvpuser (username);

