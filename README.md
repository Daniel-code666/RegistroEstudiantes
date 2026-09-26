# Registro de estudiantes — backend

API de registro académico desarrollada con .NET 10, ASP.NET Core, Entity Framework Core, SQL Server y AutoMapper. El frontend asociado utiliza Angular 21.

## Estructura

| Proyecto o carpeta | Responsabilidad |
| --- | --- |
| RegistroEstudiante.Domain | Entidades y constantes del negocio. |
| RegistroEstudiante.Application | Servicios, DTO, validaciones, interfaces y perfiles de AutoMapper. |
| RegistroEstudiante.Infrastructure | Repositorios, DbContext, configuraciones y migraciones. |
| RegistroEstudianteBack | Controladores, autenticación JWT, Swagger, errores y salud. |
| RegistroEstudiante.Tests | Pruebas del modelo, auditoría, dependencias, paginación y mapeos. |
| database | Scripts SQL del esquema. |
| scripts | Preparación de variables para Docker Compose. |

Los servicios siguen un flujo directo: validar, mapear, modificar las entidades y guardar. No existen envoltorios personalizados de transacciones ni aislamiento serializable entre validación y guardado. EF Core conserva su comportamiento transaccional normal en SaveChanges; las validaciones previas no garantizan las reglas entre solicitudes simultáneas.

AutoMapper centraliza las conversiones en `RegistroEstudiante.Application/Mapping/ApplicationAppProfile.cs`. Los identificadores del usuario autenticado se obtienen del token.

## Funcionalidades y roles

- **Student:** registro público, perfil, cambio de contraseña, inscripción de materias, consulta de estudiantes y compañeros.
- **Professor:** gestión de sus materias, asignación de materias disponibles y consulta de alumnos.
- **Admin:** administración de usuarios, roles, materias e inscripciones de estudiantes.

Los roles adicionales no reciben automáticamente los permisos anteriores. Eliminar usuarios, roles o materias los desactiva; retirar una materia elimina la relación de inscripción.

La autenticación usa JWT y verifica vigencia, estado del usuario y rol y versión del token. Cambiar o restablecer una contraseña invalida los tokens anteriores.

## Modelo y reglas académicas

- User representa estudiantes, profesores y administradores, diferenciados por Role.
- Subject representa una materia y su profesor opcional.
- StudentSubject relaciona estudiantes con materias mediante una clave compuesta.
- Cada materia tiene **3 créditos**.
- Un estudiante puede guardar **entre 0 y 3 materias**, sin duplicados y con profesores diferentes.
- Las materias y profesores seleccionados deben estar activos. Una materia sin profesor no admite inscripciones.
- Un profesor puede tener **hasta 2 materias activas**.
- No se permite desactivar o desasignar materias con estudiantes inscritos.
- Los compañeros se consultan solo para materias del estudiante. La respuesta contiene nombres y excluye al propio estudiante.

Las migraciones incluyen **10 materias y 5 profesores iniciales**. La administración puede ampliar el catálogo; esas cantidades no son límites globales. Los profesores iniciales se convierten en usuarios con datos provisionales y sin contraseña utilizable: deben completarse sus datos y restablecerse sus contraseñas desde administración.

El enunciado indica que el estudiante selecciona tres materias. La implementación interpreta esta regla como un máximo de tres, con hasta nueve créditos, y permite guardar una inscripción vacía.

## Ejecutar con Docker Compose

Requiere Docker con soporte para contenedores Linux. Desde la raíz del backend:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\init-docker.ps1
docker compose up --build -d
docker compose ps
```

El script crea `.env` con credenciales aleatorias de SQL Server, clave JWT y datos del administrador inicial. Si existe, lo conserva. No publicar este archivo.

Antes de iniciar, configurar `FRONTEND_PATH` en `.env` si la ubicación del proyecto Angular difiere de la ruta local incluida en compose.yaml:

```dotenv
FRONTEND_PATH=C:/ruta/al/Front/RegistroEstudiante
```

| Servicio | Dirección predeterminada |
| --- | --- |
| Frontend | http://localhost:5089 |
| Swagger | http://localhost:5088/swagger |
| Salud | http://localhost:5088/health |
| SQL Server | localhost,14334 |

Compose aplica las migraciones al arrancar la API y conserva los datos en el volumen sqlserver_data. Las credenciales del administrador están en `BOOTSTRAP_ADMIN_EMAIL` y `BOOTSTRAP_ADMIN_PASSWORD` dentro de `.env`. Solo se crea si no existe ningún administrador; no reemplaza credenciales existentes.

## Ejecutar el backend sin Docker

Requiere SDK .NET 10 y una instancia accesible de SQL Server. Desde la raíz del backend:

```powershell
dotnet restore RegistroEstudianteBack.slnx
dotnet build RegistroEstudianteBack.slnx --no-restore

# Adaptar la conexión a la instancia local.
$env:ConnectionStrings__DefaultConnection = 'Server=localhost;Database=RegistroEstudiantes;Integrated Security=true;TrustServerCertificate=true'

# Generar una clave para esta sesión; mantenerla estable para conservar los tokens.
$jwtBytes = New-Object byte[] 48
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
try { $rng.GetBytes($jwtBytes) } finally { $rng.Dispose() }
$env:Jwt__SigningKey = [Convert]::ToBase64String($jwtBytes)

$env:Database__InitializeOnStartup = 'true'
$env:Http__UseHttpsRedirection = 'false'
dotnet run --project RegistroEstudianteBack --launch-profile http
```

El perfil HTTP utiliza **http://localhost:5010**, con Swagger en `/swagger`. La cadena de conexión y clave JWT son obligatorias; también pueden configurarse mediante User Secrets. La clave debe tener al menos 32 bytes. TrustServerCertificate en el ejemplo corresponde al desarrollo local.

La inicialización de base de datos está desactivada si no se configura Database:InitializeOnStartup. Como alternativa a migrar al arrancar, `database/RegistroEstudiantes.sql` contiene el historial completo del esquema. `database/InitialAuthentication.sql` contiene solo el esquema inicial de autenticación.

Para crear un administrador local, configurar antes del arranque `BootstrapAdmin__Enabled=true` y las variables `BootstrapAdmin__Email`, `BootstrapAdmin__Password`, `BootstrapAdmin__Name`, `BootstrapAdmin__LastName`, `BootstrapAdmin__IdentificationType` y `BootstrapAdmin__IdentificationNumber`, con valores que cumplan las validaciones de usuarios.

### Conexión con el frontend

El frontend utiliza rutas relativas `/api`. Su proxy de desarrollo apunta a **http://localhost:5088**, el puerto de Compose. Para ejecutar la API con el perfil local, cambiar el destino de `proxy.conf.json` del frontend a **http://localhost:5010**.

Los orígenes CORS predeterminados son http://localhost:4200 y http://localhost:5173; se configuran en Cors:AllowedOrigins.

## Endpoints consumidos por el frontend

Todas las rutas siguientes llevan el prefijo `/api`. Salvo login y registro, requieren autenticación y los permisos definidos en cada controlador.

| Ruta | Métodos y propósito |
| --- | --- |
| `/auth/login` | POST: iniciar sesión. |
| `/users/register` | POST: registrar estudiante. |
| `/users/me` | GET: recuperar sesión/perfil; PUT: editar perfil. |
| `/users/me/password` | PUT: cambiar contraseña propia. |
| `/users` | GET: listar usuarios y profesores; POST: crear usuario. |
| `/users/{id}` | GET: consultar estudiante para administrar inscripción; PUT: editar; DELETE: desactivar. |
| `/users/{id}/activate` | PATCH: reactivar usuario. |
| `/users/{id}/password` | PUT: restablecer contraseña. |
| `/Role` | GET: listar roles y llenar selectores; POST: crear rol. |
| `/Role/{id}` | PUT: editar; DELETE: desactivar. |
| `/Role/{id}/activate` | PATCH: reactivar rol. |
| `/Subject` | GET: listar catálogo; POST: crear materia. |
| `/Subject/{id}` | GET: consultar detalle; PUT: editar; DELETE: desactivar. |
| `/Subject/{id}/activate` | PATCH: reactivar materia. |
| `/students` | GET: consultar nombres de estudiantes y materias inscritas. |
| `/enrollments/me` | GET: consultar inscripción propia; PUT: guardar selección completa. |
| `/enrollments/{userId}` | GET y PUT: consultar y actualizar inscripción desde administración. |
| `/enrollments/me/{subjectId}` | DELETE: retirar materia propia. |
| `/enrollments/{userId}/{subjectId}` | DELETE: retirar materia desde administración. |
| `/enrollments/me/{subjectId}/classmates` | GET: consultar compañeros de materia propia. |
| `/enrollments/{userId}/{subjectId}/classmates` | GET: consultar compañeros desde administración. |
| `/professors/me/subjects` | GET: listar materias propias; POST: crear materia. |
| `/professors/me/subjects/available` | GET: listar materias disponibles. |
| `/professors/me/subjects/{id}` | PUT: editar materia propia; DELETE: desactivar. |
| `/professors/me/subjects/{id}/activate` | PATCH: reactivar materia propia. |
| `/professors/me/subjects/{id}/assignment` | PUT: asignarse materia; DELETE: desasignarse. |
| `/professors/me/subjects/{subjectId}/students` | GET: consultar nombres de alumnos. |

La inscripción se guarda con PUT y un cuerpo como `{"subjectIds":[1,3,5]}`. Una lista vacía retira todas las materias. No existen endpoints POST de inscripción individual ni GET de rol por ID.

Los listados paginados aceptan PageNumber y PageSize (entre 1 y 100). Roles devuelve una lista completa y el frontend la pagina localmente.

## Archivos HTTP para pruebas manuales

- `RegistroEstudianteBack/RegistroEstudianteBack.http`: Swagger, salud, registro, login, perfil y usuarios. Host inicial en el puerto 5010.
- `RegistroEstudianteBack/Academic.http`: roles, materias e inscripciones. Host inicial en el puerto 5088.
- `RegistroEstudianteBack/Professors.http`: operaciones del profesor.

Se ejecutan desde un editor compatible con solicitudes HTTP. Ajustar host, identificadores y tokens. Son ejemplos para pruebas manuales: no los consume el frontend ni son necesarios para ejecutar la aplicación.

`/health` comprueba acceso a la tabla de usuarios y migraciones pendientes; devuelve 503 si la base no está disponible o el esquema no está listo. Docker utiliza esta comprobación. `/` redirige a Swagger.

## Pruebas y alcance de la verificación

```powershell
dotnet build RegistroEstudianteBack.slnx --no-restore
dotnet test RegistroEstudianteBack.slnx --no-build --no-restore
```

Última verificación del código: compilación sin errores ni advertencias y **8 casos de prueba aprobados**. Cubren relaciones del modelo, auditoría síncrona y asíncrona, resolución de dependencias, límites de paginación y configuración de AutoMapper.

Estas pruebas no verifican persistencia real en SQL Server ni todos los flujos funcionales. La actualización de este documento no implica una nueva ejecución de Docker o una prueba completa desde el navegador.
