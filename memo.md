dotnet build -t:Run -f net8.0-windows10.0.19041.0 src\Presentation\IDES.Portail.MAUI\IDES.Portail.MAUI.csproj

dotnet run --project src\Presentation\IDES.Portail.MAUI\IDES.Portail.MAUI.csproj -f net8.0-windows10.0.19041.0


git add .
git commit -m "MAJ"
git push -u origin main