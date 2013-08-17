x64\certmgr.exe -add testcert.cer -s -r localMachine root
md "C:\Program Files\Freestyle"
copy freestyle.exe "C:\Program Files\Freestyle"
xcopy profiles "C:\Program Files\Freestyle" /s /i