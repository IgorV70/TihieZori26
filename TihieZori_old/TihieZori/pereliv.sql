SELECT 'insert into Documents(OrderId,Name,Title,Comment,Active) 
values ('+cast(OrderId as varchar(20))+','''+Name+''','''+Title+''','''+Comment+''','+cast(Active as varchar(20))+')' FROM zori2.dbo.Documents


SELECT 'insert into Feedbacks(Datm,Title,Comment,Active) 
values ('''+cast(Datm as varchar(20))+''','''+Title+''','''+cast(Comment as varchar(4000))+''','+cast(Active as varchar(20))+')' FROM zori2.dbo.Feedbacks

insert into Roles (Id,Name) values (0,'Администратор')
insert into Roles (Id,Name) values (1,'Пользователь')
insert into DbVersion (VersionNum,Comment) values (1,'20180315')


SELECT 'insert into Accrual(Name,AccDate,UserId,AccSum,PaymentDate) 
values ('''+cast(Name as varchar(20))+''','''
+cast(AccDate as varchar(50))+''','
+cast(UserId as varchar(50))+','
+cast(AccSum as varchar(50))+','
+cast(PaymentDate as varchar(50))+')' FROM dbo.Accrual