# Apuração de Ponto Simples

## Backend

### Requisitos
- .NET 8+
- PostgreSQL

### Configuração
Edite `D:\Programacao\ApuracaoPontoSimples\src\ApuracaoPontoSimples.Api\appsettings.json`:
- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`

### Bootstrap do admin
Use `POST /api/auth/bootstrap` com:
```json
{
  "email": "admin@local",
  "password": "Senha@123",
  "fullName": "Administrador"
}
```

### Comandos sugeridos
```bash
$env:DOTNET_CLI_HOME='D:\Programacao\ApuracaoPontoSimples\.dotnet'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT='1'
$env:NUGET_CONFIG_FILE='D:\Programacao\ApuracaoPontoSimples\NuGet.Config'
$env:NUGET_PACKAGES='D:\Programacao\ApuracaoPontoSimples\.nuget\packages'

dotnet restore D:\Programacao\ApuracaoPontoSimples\ApuracaoPontoSimples.slnx

dotnet build D:\Programacao\ApuracaoPontoSimples\ApuracaoPontoSimples.slnx
```

## Frontend

### Requisitos
- Node.js 18+

```bash
cd D:\Programacao\ApuracaoPontoSimples\frontend
npm install
npm run dev
```
