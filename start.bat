@echo off

REM call C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe /win32icon:icon.ico /r:Mono.Nat.dll dang.cs executor.cs dang_server.cs
call C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /win32icon:icon.ico /r:Mono.Nat.dll dang.cs executor.cs dang_server.cs

echo compiled!
pause
cls
REM call dang.exe test.dang --debug
call dang.exe

pause