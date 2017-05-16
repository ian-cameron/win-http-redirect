echo off
mkdir "%programfiles%\HttpRedirect"
xcopy /y "WinHttpRedirect.exe*" "%programfiles%\HttpRedirect\"
%systemroot%\system32\sc.exe create HttpRedirect binPath= "%programfiles%\HttpRedirect\WinHttpRedirect.exe" displayname= "Helpdesk HTTP Redirect Service" obj= "NT AUTHORITY\NetworkService"
%systemroot%\system32\sc.exe description HttpRedirect "Tiny HTTP Server to redirect all requests."
net start HttpRedirect
echo Install script completed.
pause