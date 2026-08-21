# Guía de consumo y configuración de EAC.Foundation

> **Orden documental:** DOC-107 · **Etapa:** Consumo · **Versión demostrada:** `0.1.0-rc.3` · [Índice](../INDICE_DOCUMENTAL.md)

## 1. Cuándo usarlo

Use `EAC.Foundation` para primitivas compartidas, modelado de dominio y
contratos neutrales de Application: resultados, errores, entidades, agregados,
eventos, Commands, Queries, paginación y puertos CQRS. No lo use para hosting,
DI, transporte, seguridad, observabilidad ni acceso físico a datos.

## 2. Instalación

```xml
<PackageReference Include="EAC.Foundation" Version="0.1.0-rc.3" />
```

El paquete tiene como target `net10.0` y no necesita registro en DI.

## 3. Consumo mínimo

```csharp
public sealed record IssueOrderCommand(Guid OrderId) : ICommand;

public sealed class Order : AggregateRoot<Guid>
{
    public Order(Guid id) : base(id) { }
}
```

Los contratos se seleccionan explícitamente en código. Instalar el paquete no
crea servicios, workers, conexiones ni recursos.

## 4. Configuración

Foundation no posee configuración ambiental. No existe sección
`Eac:Foundation`, variables de entorno, secretos ni configuración programática
equivalente. Los tipos CLR, reglas de dominio y contratos de casos de uso no
son settings operativos.

La persistencia, mensajería, seguridad y observabilidad se configuran en sus
componentes propietarios. Cambiar contratos o actualizar el paquete requiere
recompilar y desplegar el consumidor.

## 5. Comportamientos y errores esperados

- un `Result` fallido conserva errores explícitos y no lanza por representar un fallo funcional;
- un agregado conserva sus eventos en orden hasta que el consumidor los confirma;
- `PageRequest` usa índices base 1 y rechaza rangos inválidos;
- los puertos de Application no realizan DI ni acceso a infraestructura.

## 6. Compatibilidad y verificación

La API pública se valida mediante Architecture, Contract y Unit Tests, con
documentación XML obligatoria. El paquete es compatible con trimming y Native
AOT dentro de los contratos declarados y no usa descubrimiento por reflexión.

- [Identidad y límites](../architecture/EAC_FOUNDATION.md)
- [SharedKernel](../architecture/EAC_FOUNDATION_SHARED_KERNEL.md)
- [Domain](../architecture/EAC_FOUNDATION_DOMAIN.md)
- [Application](../architecture/EAC_FOUNDATION_APPLICATION.md)
- [Plan y evidencia](../planning/PLAN_DE_IMPLEMENTACION.md)
