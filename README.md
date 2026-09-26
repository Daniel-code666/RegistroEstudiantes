# Registro de estudiantes — backend

Solución .NET 10 con ASP.NET Core, Entity Framework Core y SQL Server, organizada en Domain, Application, Infrastructure y API.

## Estado de la revisión

Se revisó el documento `PRUEBA TECNICA APLICACION WEB (1) (1) (1) (1) (2) (1).md` de la carpeta de la prueba técnica de Interrapidísimo, el 25 de septiembre de 2026.

El proyecto es una estructura inicial: compila y permite abrir Swagger, pero **aún no implementa operaciones de negocio**. Los controladores, interfaces, servicios y repositorios existentes son esqueletos. Que Swagger abra o las pruebas pasen no significa que el CRUD esté implementado.

| Requisito del documento | Estado actual / trabajo pendiente |
| --- | --- |
| CRUD para registro en línea | Existe `User`; faltan contratos de entrada/salida, validaciones y operaciones HTTP con persistencia. |
| Adhesión a un programa de créditos | Falta definir y representar la matrícula; el documento no define varios programas académicos. |
| 10 materias, cada una de 3 créditos | Existe `Subject`, pero faltan datos iniciales y restricciones que garanticen esos valores. |
| Selección de 3 materias | Existe relación muchos a muchos; falta validar cantidad, duplicados y materias existentes/activas. |
| 5 profesores, cada uno dicta 2 materias | Faltan entidad profesor, relación con materias y catálogo inicial. |
| No repetir profesor en las materias elegidas | Falta validación de matrícula, tanto al crear como al actualizar. |
| Ver registros de otros estudiantes | Falta consulta pública con respuesta limitada a los datos permitidos. |
| Ver solo nombres de compañeros por clase | Falta consulta por materia, filtrada por matrícula del estudiante y excluyendo al propio estudiante; no exponer documentos ni correos. |
| Base de datos o scripts MySQL / SQL | Se configuró el proveedor SQL Server; faltan migraciones, datos iniciales y script entregable. |
| Aplicación web o cliente-servidor y entregables adjuntos | El frontend y el empaquetado de entrega quedan fuera de esta revisión del backend. |

### Decisiones que conviene fijar al implementar

- “Sólo podrá seleccionar 3 materias” puede significar máximo tres o exactamente tres. Propuesta: permitir hasta tres durante la selección y exigir tres al confirmar la matrícula (9 créditos). El documento no especifica un flujo de borrador.
- La consulta de otros registros no debe devolver directamente la entidad `User`. Usar DTO específicos; para compañeros, únicamente nombres.
- El documento no exige login explícitamente. Si se requiere que cada estudiante solo modifique su registro y consulte sus propias clases, debe existir una identidad verificada; un ID enviado por el cliente no basta.
- No se solicitan CRUD de roles ni administración libre de catálogos. Las clases actuales de roles no cubren ningún requisito obligatorio por sí solas.

## Correcciones realizadas

- Referencias entre proyectos y dependencias NuGet explícitas; incorporación de todas las capas y pruebas a la solución.
- Uso del enum existente `TipoIdentificacion` e inicialización válida de `User.Subjects` sin elementos nulos.
- Relación muchos a muchos mediante `StudentSubjects`, evitando que una materia pertenezca a un solo estudiante.
- Auditoría mediante propiedades de EF, conservando los setters privados y la fecha de creación al actualizar; aplicación efectiva de las configuraciones de fechas a las tres entidades.
- Registro en inyección de dependencias de los servicios requeridos por los controladores.
- Corrección del arranque y Swagger: imports, paquete, generación XML, eliminación de `MapOpenApi` sin registro de servicios y textos ajenos al proyecto.
- Protección al habilitar inicialización de BD sin migraciones: se informa el problema en vez de anunciar una base lista sin tablas.
- Dockerfile actualizado para copiar los proyectos referenciados antes de restaurar.
- Archivo HTTP actualizado a rutas existentes y `.gitignore` para artefactos locales. Esta carpeta no tenía repositorio Git al revisarla.

## Ejecutar

Requiere SDK .NET 10. Para persistencia se necesita una instancia accesible de SQL Server. No se incluyen credenciales.

```powershell
dotnet restore RegistroEstudianteBack.slnx
dotnet build RegistroEstudianteBack.slnx -c Release --no-restore
dotnet test RegistroEstudianteBack.slnx -c Release --no-restore

# Adaptar servidor/autenticación al entorno real.
$env:ConnectionStrings__DefaultConnection = 'Server=localhost;Database=RegistroEstudiantes;Integrated Security=true;TrustServerCertificate=true'
dotnet run --project RegistroEstudianteBack --launch-profile http
```

Swagger: `http://localhost:5010/swagger`. La cadena de conexión es obligatoria; se puede suministrar también mediante User Secrets en desarrollo. `TrustServerCertificate=true` en el ejemplo es para desarrollo local.

`Database:InitializeOnStartup` permanece desactivado por defecto. No activarlo hasta crear las migraciones. `/health` comprueba acceso a la tabla de usuarios y migraciones pendientes: responde 503 si la BD no está disponible o no tiene el esquema requerido. Abrir Swagger no requiere conectarse a la BD.

## Verificación y límites

- Compilación Release: cero errores y advertencias.
- Siete pruebas automatizadas: modelo muchos a muchos, auditoría síncrona/asíncrona, resolución de servicios y validación de paginación incluyendo desbordamiento.
- Arranque HTTP y documento/UI Swagger comprobados localmente. El documento no contiene operaciones de negocio porque los controladores todavía no tienen acciones.
- Las pruebas de auditoría interceptan la escritura: no validan persistencia real en SQL Server. No se ejecutaron migraciones, pruebas contra una BD real ni una compilación de imagen Docker.

## Orden propuesto para completar el backend

1. Modelar profesor y matrícula, fijar reglas y crear catálogo de 10 materias/5 profesores.
2. Implementar CRUD con DTO, validaciones y manejo de recursos inexistentes/conflictos.
3. Implementar inscripción atómica y validar cantidad de materias y profesores distintos también en actualizaciones.
4. Implementar consultas de registros y compañeros con las restricciones de información.
5. Crear migraciones/script SQL y probar CRUD, reglas de matrícula y privacidad contra SQL Server.
