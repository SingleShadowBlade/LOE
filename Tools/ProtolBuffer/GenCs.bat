@echo off
set tool=ProtoGen
 
set proto=test.proto
 
%tool%\protogen.exe -i:%proto% -o:%proto%.cs -ns:Proto
 
pause