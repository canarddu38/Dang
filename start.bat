@echo off

cd C:\Users\celin\Documents\JULES\DANG\Compiler

call C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe "C:\Users\celin\Documents\JULES\DANG\Compiler\program.cs" "C:\Users\celin\Documents\JULES\DANG\Compiler\executor.cs"

echo compiled!
call "C:\Users\celin\Documents\JULES\DANG\Compiler\program.exe" C:\Users\celin\Documents\JULES\DANG\Compiler\test.dang

pause