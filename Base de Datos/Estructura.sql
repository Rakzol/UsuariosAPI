drop database datos_usuario;
create database datos_usuario;
use datos_usuario;

create table rol(
	id int unsigned auto_increment primary key,
    nombre varchar(255) not null unique check ( nombre regexp '^[a-zA-ZñÑáéíóúÁÉÍÓÚ]+( [a-zA-ZñÑáéíóúÁÉÍÓÚ]+)*$' )
);

create table sucursal(
	id int unsigned auto_increment primary key,
    nombre varchar(255) not null unique check ( nombre regexp '^[a-zA-ZñÑáéíóúÁÉÍÓÚ]+( [a-zA-ZñÑáéíóúÁÉÍÓÚ]+)*$' )
);

create table usuario(
	id int unsigned auto_increment primary key,
    email varchar(255) not null unique check ( email regexp '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$' ),
    contraseña varchar(255) not null,
    nombres varchar(255) not null check ( nombres regexp '^[a-zA-ZñÑáéíóúÁÉÍÓÚ]+( [a-zA-ZñÑáéíóúÁÉÍÓÚ]+)*$'),
    apellidos varchar(255) not null check ( apellidos regexp '^[a-zA-ZñÑáéíóúÁÉÍÓÚ]+( [a-zA-ZñÑáéíóúÁÉÍÓÚ]+)*$'),
    rol int unsigned not null,
    sucursal int unsigned not null,
    foreign key (rol) references rol(id),
    foreign key (sucursal) references sucursal(id)
);