## Instruction

```
npm install
dotnet restore
set ASPNETCORE_ENVIRONMENT=Development
set ConnectionStrings:DefaultConnection="Server=DESKTOP-Q1FD1OK\\MSSQLSERVER08;Database=ShopDB;Trusted_Connection=True;TrustServerCertificate=True;"
npm i webpack -g
npx webpack --config webpack.config.js
npm run build
dotnet ef migrations add InitialCreate
dotnet build
dotnet ef database update
dotnet run
```
