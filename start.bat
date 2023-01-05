@echo off

REM cd C:\Users\celin\Documents\JULES\DANG\Compiler

call C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /win32icon:icon.ico /r:Mono.Nat.dll dang.cs executor.cs dang_server.cs

echo compiled!
pause
cls
call dang.exe test.dang

pause