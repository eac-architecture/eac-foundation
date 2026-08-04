# EAC Foundation

EAC Foundation es el núcleo de un ecosistema de productos técnicos
reutilizables para .NET 10. Su diseño no presupone un estilo de despliegue:
puede utilizarse en microservicios, monolitos modulares, APIs, workers,
procesos por lotes, gateways y aplicaciones de consola.

El paquete `EAC.Foundation` proporciona primitivas y contratos de Domain y
Application. No contiene lógica funcional, infraestructura, transporte ni
dependencias de proveedores.

## Ownership

Este repositorio posee exclusivamente el código, pruebas, documentación y
release del NuGet `EAC.Foundation`.

No pertenece a una solución funcional ni a un curso. Los consumidores utilizan
versiones publicadas; no copian el código fuente ni cambian los contratos del
producto por necesidades particulares.

La arquitectura transversal, el catálogo y los estándares pertenecen a
[`eac-engineering-governance`](https://github.com/eac-architecture/eac-engineering-governance).
Los demás productos reutilizables tienen repositorios, documentación, paquetes,
versiones y pipelines propios.

## Documentación

- [Índice documental](docs/INDICE_DOCUMENTAL.md)
- [Plan de implementación](docs/planning/PLAN_DE_IMPLEMENTACION.md)
- [Integración continua](docs/operations/CONTINUOUS_INTEGRATION.md)
- [Candidato de release](docs/operations/RELEASE_CANDIDATE.md)
- [Identidad y límites](docs/architecture/EAC_FOUNDATION.md)
- [SharedKernel](docs/architecture/EAC_FOUNDATION_SHARED_KERNEL.md)
- [Domain](docs/architecture/EAC_FOUNDATION_DOMAIN.md)
- [Application](docs/architecture/EAC_FOUNDATION_APPLICATION.md)
- [ADR del componente](docs/decisions/ADR-0011-eac-foundation.md)

## Estado

El producto se encuentra en implementación inicial. `VERSION` declara
`0.1.0-rc.1` como versión preliminar vigente; todavía no representa una
publicación final ni estable.

El primer incremento establece:

- un repositorio independiente;
- un único proyecto empaquetable y un único NuGet `EAC.Foundation`;
- un único ensamblado preparado para alojar `SharedKernel`, `Domain` y `Application` como namespaces internos;
- compilación determinista para `net10.0`;
- xUnit v3 sobre Microsoft Testing Platform;
- manifest de capabilities y entrada única de CI.
- SharedKernel con `Error`, `Result`, `Result<TValue>` e `IDomainEvent`.
- Domain validado con Entity, AggregateRoot, gestión ordenada de eventos y ValueObject.
- solicitudes de caso de uso mediante `IUseCaseRequest<TUseCaseResponse>`, `ICommand`, `ICommand<TValue>` e `IQuery<TValue>`;
- Use Cases tipados mediante `IUseCase`, `ICommandUseCase` e `IQueryUseCase`, con cancelación explícita;
- dispatcher local abstracto mediante `IUseCaseDispatcher`;
- pipeline tipado mediante `UseCaseContinuation<TUseCaseResponse>` e `IPipelineBehavior<TUseCaseRequest, TUseCaseResponse>`;
- fallos y errores de validación inmutables mediante `ValidationFailure` y `ValidationError`;
- resultado de validación explícito mediante `ValidationOutcome`;
- validator asíncrono neutral a proveedores mediante `IUseCaseRequestValidator<TUseCaseRequest>`;
- solicitud de página inmutable y base 1 mediante `PageRequest`;
- página inmutable con total, cálculo seguro y navegación mediante `Page<TItem>`;
- puertos de persistencia CQRS mediante Repository de Aggregate Roots, Unit of Work local, resultado de commit y QueryService;
- documentación XML obligatoria para toda la API pública del paquete;
- objetivos ejecutables y trazabilidad de reglas en todas las pruebas.
- auditoría ejecutable de Application con reglas cubiertas o delegadas explícitamente.

## Estructura

```text
eac-foundation/
├── docs/
│   ├── architecture/
│   ├── decisions/
│   ├── planning/
│   ├── governance/
│   ├── operations/
│   └── INDICE_DOCUMENTAL.md
├── .config/
│   └── dotnet-tools.json
├── .tekton/
│   └── continuous-integration.yaml
├── eng/
│   └── capabilities.yml
├── scripts/
│   ├── ci.sh
│   ├── validate.sh
│   ├── build.sh
│   ├── test.sh
│   ├── pack.sh
│   ├── release-candidate.sh
│   └── version.sh
├── src/
│   └── EAC.Foundation/
├── tests/
│   ├── EAC.Foundation.ArchitectureTests/
│   ├── EAC.Foundation.ContractTests/
│   └── EAC.Foundation.UnitTests/
├── Directory.Build.props
├── Directory.Packages.props
├── EAC.Foundation.sln
├── NuGet.Config
├── VERSION
└── global.json
```

El incremento vigente madura la misma línea `release/*` mediante versiones
`alpha.N`, `beta.N` y `rc.N`. La versión estable solo se publicará después de
integrar el release aprobado en `main`.

## Validación local

```bash
./scripts/ci.sh
bash ./scripts/validate-docs.sh
```

El script valida gobierno y documentación, restaura dependencias bloqueadas,
verifica formato, compila y ejecuta las pruebas. El paquete se genera de forma
explícita mediante `./scripts/pack.sh`; CI-001 no mezcla verificación con
empaquetado o publicación. Por defecto las pruebas utilizan `Table`: muestran
estado, regla de conformidad, objetivo y duración en una fila compacta, sin
rutas físicas, y terminan con el resumen global.

Los modos disponibles son:

```bash
TEST_OUTPUT=Table ./scripts/ci.sh
TEST_OUTPUT=Detailed ./scripts/ci.sh
TEST_OUTPUT=Normal ./scripts/ci.sh
```

- `Table`: tabla compacta predeterminada;
- `Detailed`: salida completa para diagnóstico;
- `Normal`: resumen nativo de Microsoft Testing Platform.

Cualquier otro valor falla antes de ejecutar el pipeline.

El color se controla de forma independiente:

```bash
TEST_COLOR=Auto ./scripts/ci.sh
TEST_COLOR=Always ./scripts/ci.sh
TEST_COLOR=Never ./scripts/ci.sh
```

`Auto` es el valor predeterminado: colorea `PASS` en verde, `FAIL` en rojo, `SKIP` en amarillo y la regla en cian cuando la salida es una terminal. La variable estándar `NO_COLOR` desactiva el color en modo `Auto`.

## Convenciones de documentación ejecutable

El código, los comentarios XML, los nombres de pruebas y la salida técnica se escriben en inglés. La documentación explicativa del repositorio se mantiene en español.

Cada prueba declara:

- un `DisplayName` en inglés que expresa el comportamiento observable esperado;
- un `Trait("Rule", "<rule-id>")` que la enlaza con una regla de conformidad;
- assertions que contienen el detalle verificable del escenario.

El nombre del método conserva una identidad técnica estable para navegación y diagnóstico. El cuadro de `ci.sh` presenta el objetivo de `DisplayName` junto con la regla correspondiente.

La API pública utiliza comentarios XML en inglés. El warning `CS1591` se trata como error en el proyecto empaquetable, por lo que una API pública sin documentación no puede superar el build. El archivo XML generado acompaña al ensamblado y al paquete NuGet.
