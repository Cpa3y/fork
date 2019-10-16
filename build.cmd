@powershell -NoProfile -ExecutionPolicy unrestricted "%~dp0build.ps1" %*

:: Publish: call build -Publish Dev
