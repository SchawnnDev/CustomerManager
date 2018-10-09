use master;

go
alter database CustomerManager
set single_user with
rollback immediate;

go
drop database CustomerManager