protoc -I=. --csharp_out=. message.proto
protoc -I=. --csharp_out=. NatsMessages.proto

copy Message.cs  ..\messages\Message.cs
copy NatsMessages.cs  ..\messages\NatsMessages.cs
pause
