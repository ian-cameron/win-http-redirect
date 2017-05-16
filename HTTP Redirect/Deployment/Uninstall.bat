echo off
net stop HttpRedirect
sc delete HttpRedirect
echo Uninstall script completed
pause