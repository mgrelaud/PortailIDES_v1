Stop-Process -Name "msedgewebview2", "PortailMetier.Frontend", "dotnet" -ErrorAction SilentlyContinue; Remove-Item -Path "Frontend\bin", "Frontend\obj" -Recurse -Force -ErrorAction SilentlyContinue; Remove-Item -Path "$env:LOCALAPPDATA\PortailMetier.Frontend" -Recurse -Force -ErrorAction SilentlyContinue

dotnet run --project Frontend/PortailMetier.Frontend.csproj -f net9.0-windows10.0.19041.0
