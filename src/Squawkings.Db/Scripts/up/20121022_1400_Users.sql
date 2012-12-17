create table dbo.Users (
	UserId int identity(1,1) primary key,
    UserName nvarchar(50),
	FirstName nvarchar(50),
	LastName nvarchar(50),
	Email nvarchar(300),
    AvatarUrl nvarchar(300)
)
go

create table dbo.UserSecurityInfo (
	UserId int primary key,
	Password nvarchar(100),
	foreign key (UserId) references Users(UserId)
)
go

create table dbo.Squawks (
    SquawkId int identity(1,1) primary key,
    UserId int not null,
    CreatedAt datetime not null,
    Content nvarchar(200)
)
go
