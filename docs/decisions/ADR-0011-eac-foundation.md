# ADR-0011: crear EAC Foundation como producto genérico

> **Orden documental:** DOC-026 · **Etapa:** Decisión · [Índice maestro](../INDICE_DOCUMENTAL.md)

- **Estado:** Accepted
- **Fecha:** 2026-07-16
- **Ámbito:** capacidades transversales y plataforma de desarrollo

## Contexto

Las aplicaciones necesitan capacidades repetidas de hosting, configuración,
errores, seguridad, observabilidad, persistencia, mensajería, resiliencia y
pruebas. Copiar estas implementaciones dentro de cada repositorio provocaría
divergencia. Concentrarlas en una única librería general introduciría
acoplamiento, dependencias innecesarias y ciclos de publicación globales.

El Service Starter necesita consumir capacidades runtime estables, pero debe continuar siendo un generador de código y no convertirse en una biblioteca runtime implícita.

## Decisión

Crear `EAC Foundation` como producto genérico, independiente de cualquier solución consumidora.

La identidad de gobierno será:

```text
Producto: EAC Foundation
Repositorio: eac-foundation
Namespace: EAC
Versionado: SemVer
```

EAC Foundation:

- publica un único NuGet cohesionado `EAC.Foundation`;
- contiene SharedKernel, Domain y Application como namespaces del mismo
  ensamblado;
- mantiene fuera del núcleo los adaptadores físicos y capacidades opcionales;
- participa en una matriz de compatibilidad entre productos independientes;
- carece de lógica, contratos y nombres de dominios consumidores;
- se consume con versiones explícitas;
- declara su versión preliminar en el archivo `VERSION`; build, package y
  candidato deben usar exactamente ese valor;
- admite únicamente versiones `alpha.N` o `beta.N` hasta que una decisión
  posterior habilite release candidates o versiones finales;
- no exige herencia desde clases base universales;
- consume Configuration y Options nativos e incorpora solo capacidades adicionales de seguridad, observabilidad y pruebas;
- es consumido por el starter y puede ser consumido directamente por aplicaciones autorizadas.
- no presupone microservicios: admite monolitos modulares, APIs, workers,
  procesos por lotes, gateways y aplicaciones de consola.

## Relación con EAC Service Starter

- `eac-foundation` proporciona comportamiento transversal runtime.
- `eac-service-starter` genera estructura, referencias, configuración, pruebas y automatización.
- una solución consumidora aporta dominio y decide qué capacidades activa.
- los tres productos tienen repositorios, versiones, pipelines y roadmaps independientes.

## Alternativas consideradas

### Copiar capacidades en cada aplicación

Descartada porque multiplica correcciones, dificulta parches de seguridad y genera comportamientos incompatibles.

### Un único paquete con todas las capacidades

Descartada porque obliga a instalar proveedores y dependencias no utilizadas y aumenta el impacto de cada cambio.

### Incorporar EAC Foundation dentro del Starter

Descartada porque mezclaría generación y runtime, impediría actualizar paquetes independientemente y ocultaría dependencias reales.

### Incluir extensiones específicas de cada solución

Descartada porque rompe la identidad genérica. Una solución puede crear paquetes propios sobre puertos públicos sin incorporarlos al catálogo general.

## Consecuencias positivas

- Capacidades transversales coherentes y parcheables.
- Starter más pequeño y declarativo.
- Aplicaciones instalan solo lo necesario.
- Proveedores físicos sustituibles.
- Versiones y compatibilidad visibles.
- Seguridad y observabilidad evolucionan como productos mantenidos.

## Consecuencias negativas

- Se necesita gobierno de paquetes y matriz de compatibilidad.
- Los cambios incompatibles requieren guías de migración.
- Más repositorios y pipelines aumentan operación de plataforma.
- Una abstracción mal diseñada puede afectar muchos consumidores.
- EAC Foundation necesita pruebas de integración con varios proveedores.

## Criterios de revisión

Crear un ADR sustituto o complementario si:

- una familia de paquetes adquiere ownership o ciclo completamente independiente;
- la matriz de compatibilidad no escala;
- se adopta una plataforma organizativa obligatoria;
- una capacidad debe retirarse del catálogo por baja reutilización;
- se soportan simultáneamente varias versiones mayores de .NET;
- un proveedor impide mantener una abstracción estable.

## Referencias

- [EAC Foundation](https://github.com/eac-architecture/eac-foundation/blob/main/docs/architecture/EAC_FOUNDATION.md)
- [Service Starter](https://github.com/eac-architecture/eac-service-starter/blob/main/docs/architecture/SERVICE_STARTER.md)
- [Catálogo de capacidades](https://github.com/eac-architecture/eac-engineering-governance/blob/main/docs/catalog/CATALOGO_DE_COMPONENTES.md)
- [Auditoría de cohesión](https://github.com/eac-architecture/eac-engineering-governance/blob/main/docs/governance/PACKAGE_COHESION_AUDIT.md)
- [ADR-0010](https://github.com/eac-architecture/eac-engineering-governance/blob/main/docs/decisions/ADR-0010-parameterized-service-starter.md)
