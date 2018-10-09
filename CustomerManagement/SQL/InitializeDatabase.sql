if not exists (select * 
               from   sys.databases 
               where  name = 'CustomerManager') 
  begin 
      create database CustomerManager 
  end 

go 

use CustomerManager 

if not exists (select * 
               from   information_schema.tables 
               where  table_name = 'Customers') 
  begin 
      create table Customers 
        ( 
           id          int not null identity, 
           firstname   nvarchar(max), 
           name        nvarchar(max), 
           dateofbirth date, 
           phonenumber nvarchar(max), 
           email       nvarchar(max), 
           primary key(id) 
        ) 
  end 

if not exists (select * 
               from   information_schema.tables 
               where  table_name = 'ShippingAddresses') 
  begin 
      create table ShippingAddresses 
        ( 
           id         int not null identity, 
           customerid int, 
           address    nvarchar(max), 
           postalcode nvarchar(max), 
           primary key(id), 
           foreign key (customerid) references Customers(id) 
        ) 
  end 