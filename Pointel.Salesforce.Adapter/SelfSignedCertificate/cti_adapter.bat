@echo off
echo Installing Certificate Authority...
rem Define the file locations as environment variables
set CERTPWD=pointel
set CERTHASH=3c942783728d7f5c0dac30c27bc78b32d2838f0c
set APPID={9D06D291-3882-4D7B-A6E5-5DD6EEE2486F}
set PORT=4040
rem Install certificate Authority
certutil -f -addstore root "PointelAuth.cer"
rem Install PFX
certutil -f -p %CERTPWD% -importpfx "Localhost.pfx"
rem bind the certificate to a port
netsh http add sslcert ipport=0.0.0.0:%PORT% certhash=%CERTHASH% appid=%APPID%
PAUSE