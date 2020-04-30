/****** Object:  StoredProcedure [Book].[uspLockBook]    Script Date: 30-04-2020 13:51:31 ******/

Create procedure [Book].[uspLockBook]
(
@ISBN nvarchar(50),
@Lock int
)

as

update [Book].[Book]
Set [Lock] = @Lock

where [ISBN] = @ISBN
GO

/****** Object:  StoredProcedure [Member].[uspRetrieveBorrowStatus]    Script Date: 30-04-2020 13:51:51 ******/

Create Procedure [Member].[uspRetrieveBorrowStatus]
(
	@MemberID int,
	@CanBorrow bit out
)

as 
select @CanBorrow = [CanBorrow] from [Member].[Member]
where
[ID] = @MemberID

GO


/****** Object:  StoredProcedure [Member].[uspRetrieveMemberDetails]    Script Date: 30-04-2020 13:52:13 ******/
Create procedure [Member].[uspRetrieveMemberDetails]
(
	@MemberID int,
	@Name nvarchar(max) out,
	@Email nvarchar(max) out
)
AS
Select @Name = [Name], @Email = [Email] 
from [Member].[Member]
Where [ID] = @MemberID
GO


/****** Object:  StoredProcedure [Member].[uspUpdateBorrowedBook]    Script Date: 30-04-2020 13:52:42 ******/
create procedure [Member].[uspUpdateBorrowedBook]
(@MemberID  int,
@CanBorrow bit,
@ISBN nvarchar(50)

)
as
update [Member].[Member]
set [CanBorrow] = @CanBorrow, [ISBN] = @ISBN
where [ID] = @MemberID

GO


/****** Object:  StoredProcedure [Reservation].[uspCreateReservation]    Script Date: 30-04-2020 13:53:01 ******/
create procedure [Reservation].[uspCreateReservation]

(
	@CorrelationID uniqueidentifier,
	@MemberID int,
	@ISBN nvarchar(50),
	@Status nvarchar(20)

)

as 
insert into [Reservation].[Reservation]
(
[CorrelationID],
	[MemberID],
	[ISBN],
	[Status]

)
values
(
	@CorrelationID, @MemberID, @ISBN, @Status
)
GO


/****** Object:  StoredProcedure [Reservation].[uspUpdateReservation]    Script Date: 30-04-2020 13:53:26 ******/
create procedure [Reservation].[uspUpdateReservation]

(
	@CorrelationID uniqueidentifier,
	@Status nvarchar(20)

)

as 
update [Reservation].[Reservation]
set
	[Status] = @Status

where [CorrelationID] = @CorrelationID
GO



