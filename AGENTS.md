# Estilo de C#

- Usar clases normales, DTO con propiedades `get; set;` y constructores explícitos.
- En métodos con cuerpo de expresión, colocar `=>` en la línea siguiente a la firma,
  con una indentación adicional. La expresión comienza en la misma línea que `=>`.

```csharp
public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    => Ok(await _auth.LoginAsync(request, cancellationToken));
```

- Esta regla corresponde a métodos y propiedades con cuerpo de expresión; no exige
  separar las lambdas de consultas como `Where(x => x.Active)`.
