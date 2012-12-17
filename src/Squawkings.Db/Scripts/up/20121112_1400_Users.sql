create table dbo.Followers (
                UserId int,
                FollowerUserId int 
)
go

create clustered index followers_ix1 on Followers (UserId)
go

create index followers_ix2 on Followers (FollowerUserId)
go

alter table dbo.Users
                add bio nvarchar(500)
go
