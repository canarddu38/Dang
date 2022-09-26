@echo off

cd C:\Users\celin\Documents\JULES\DANG\Compiler

call C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe /win32icon:icon.ico dang.cs executor.cs

echo compiled!
pause
cls
call dang.exe test.dang

pause