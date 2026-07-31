# Changelog

Todos los cambios relevantes de EAC Foundation se documentan en este archivo.

El formato sigue Keep a Changelog y el producto utiliza versionado semántico.

## [Unreleased]

### Added

- estructura inicial del repositorio y solución .NET 10;
- proyecto empaquetable `EAC.Foundation`;
- baseline de pruebas arquitectónicas con xUnit v3 y Microsoft Testing Platform;
- manifest de capabilities y script Bash de CI.
- contrato SharedKernel con errores, resultados y eventos de dominio mínimos.
- contrato `IEntity<TId>` y base opcional `Entity<TId>` con igualdad por tipo concreto e identificador.
- `IAggregateRoot`, `IHasDomainEvents` y `AggregateRoot<TId>` con acumulación ordenada y extracción de snapshots de solo lectura.
- `ValueObject` con igualdad estructural por tipo concreto y componentes ordenados.
- `IUseCaseRequest<TUseCaseResponse>`, `ICommand`, `ICommand<TValue>` e `IQuery<TValue>` como primer incremento CQRS de Application.
- `IUseCase<TUseCaseRequest, TUseCaseResponse>`, `ICommandUseCase` e `IQueryUseCase` con respuesta tipada y cancelación explícita.
- nomenclatura alineada de solicitud y respuesta mediante `TUseCaseRequest` y `TUseCaseResponse`.
- `IUseCaseDispatcher` como contrato de despacho local, tipado y ajeno a transporte.
- `UseCaseContinuation<TUseCaseResponse>` e `IPipelineBehavior<TUseCaseRequest, TUseCaseResponse>` como contratos de pipeline tipado.
- `ValidationFailure` y `ValidationError` como contratos inmutables para fallos ordenados y errores esperados de validación.
- `ValidationOutcome` con estados válido e inválido explícitos y snapshot inmutable de fallos.
- `IUseCaseRequestValidator<TUseCaseRequest>` como contrato asíncrono, contravariante y neutral a proveedores.
- `PageRequest` como solicitud de paginación inmutable, base 1 y sin límite máximo específico de una aplicación.
- `Page<TItem>` con snapshot inmutable, total largo, cálculo exacto representable y navegación explícita.
- `IRepository<TAggregate, TId>` restringido a Aggregate Roots identificados y destinado exclusivamente al Command Side.
- `IUnitOfWork` y `CommitResult` para expresar una confirmación local con conteo no negativo.
- `IQueryService<TReadModel, TId>` para lecturas por identidad separadas del repositorio de agregados.
- auditoría ejecutable de Application con neutralidad integral, separación Query/Command y snapshot público trazado.
- documentación XML obligatoria para la API pública mediante el gate `CS1591`;
- objetivos ejecutables con `DisplayName` y trazabilidad `Trait("Rule", ...)` en las 177 pruebas;
- reporte compacto de CI que presenta estado, regla, objetivo y duración en inglés.
