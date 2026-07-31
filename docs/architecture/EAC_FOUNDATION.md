# EAC Foundation

> **Orden documental:** DOC-021 · **Etapa:** Foundation · [Índice maestro](../INDICE_DOCUMENTAL.md)

> Catálogo modular de NuGet para construir y operar aplicaciones .NET con
> capacidades transversales coherentes, versionadas y sustituibles.

## 1. Identidad

| Propiedad | Valor |
|---|---|
| Producto | EAC Foundation |
| Repositorio y NuGet de núcleo | `eac-foundation` / `EAC.Foundation` |
| Namespace raíz | `EAC` |
| Versionado | SemVer propio y matriz de compatibilidad del catálogo |
| Pipeline | `eac-foundation-ci`, conforme al [estándar Tekton](https://github.com/eac-architecture/eac-architecture-docs/blob/main/docs/design/CI_CD_PORTABLE.md) |
| Consumidores | starter y aplicaciones .NET autorizadas |
| Runtime objetivo | .NET 10 LTS (`net10.0`) |

EAC Foundation es independiente de cualquier solución consumidora. No contiene entidades, contratos, roles, tópicos, bases de datos ni reglas funcionales de un producto.

Las soluciones y los materiales pedagógicos consumen versiones publicadas.
Ningún consumidor posee el código fuente ni modifica los contratos de
Foundation para resolver necesidades particulares.

Foundation no determina una topología de despliegue. El mismo núcleo puede
usarse en microservicios, monolitos modulares, APIs, workers, procesos por
lotes, gateways y aplicaciones de consola.

## 2. Relación con el starter

```text
EAC Foundation
└── paquetes runtime versionados
        ↑ consumidos por
EAC Service Starter
└── genera estructura y configuración
        ↑ utilizado por
Soluciones consumidoras
└── servicios con dominio propio
```

| Producto | Responsabilidad |
|---|---|
| Foundation | comportamiento transversal reutilizable en runtime |
| Starter | generación de código, estructura, pruebas y automatización |
| Solución consumidora | dominio, contratos, configuración y decisiones de producto |

Actualizar el Starter no actualiza aplicaciones existentes. Actualizar un paquete de Foundation es una decisión explícita del repositorio consumidor.

## 3. Principios

1. Paquetes cohesionados; no se separan capacidades que siempre se consumen juntas.
2. Ningún paquete contiene lógica de negocio.
3. Las abstracciones no dependen de proveedores concretos.
4. Los adaptadores físicos implementan abstracciones estables.
5. Una aplicación instala únicamente las capacidades que utiliza.
6. Configuración tipada y validada al arrancar.
7. Valores seguros por defecto y fallo temprano ante configuración inválida.
8. Observabilidad integrada sin registrar datos sensibles.
9. Compatibilidad, migraciones y retirada de APIs documentadas.
10. Ningún paquete accede directamente al dominio de la aplicación.

## 4. Familias de paquetes

El ecosistema EAC se organiza en capacidades de seguridad, HTTP, persistencia políglota, mensajería, Kafka, observabilidad y primitivas compartidas. La configuración utiliza las APIs nativas de .NET y una composición externa gobernada; no constituye un paquete Foundation.

### 4.1 Núcleo

| Paquete | Responsabilidad |
|---|---|
| `EAC.Foundation` | SharedKernel, Domain y Application mediante namespaces internos |

`SharedKernel` no incluirá entidades base, repositorios genéricos universales, DTO de negocio ni tipos que fuercen herencia.

El contrato público propuesto está documentado en [Diseño de EAC.Foundation.SharedKernel](EAC_FOUNDATION_SHARED_KERNEL.md).

Las bases opcionales de modelado se definen en
[Diseño de EAC.Foundation.Domain](EAC_FOUNDATION_DOMAIN.md). Las entidades y
agregados concretos permanecen en el Domain de cada aplicación consumidora.

Los contratos CQRS locales se definen en [Diseño de EAC.Foundation.Application](EAC_FOUNDATION_APPLICATION.md).

Hosting, Application Runtime y Testing forman parte del ecosistema EAC, pero no del núcleo Foundation. Configuration y Options se consumen directamente desde .NET:

| Paquete | Capa y responsabilidad |
|---|---|
| [Configuration y Options de .NET](https://github.com/eac-architecture/eac-architecture-docs/blob/main/docs/standards/configuration/CONFIGURATION_STANDARD.md) | uso directo; no se publica un wrapper EAC y el provider externo se agrega en Hosting |
| [`EAC.Application.Runtime`](https://github.com/eac-architecture/eac-application-runtime/blob/main/docs/architecture/EAC_APPLICATION_RUNTIME.md) | dispatcher, behaviors y Validation de Application |
| [`EAC.Hosting`](https://github.com/eac-architecture/eac-hosting/blob/main/docs/architecture/EAC_HOSTING.md) | composición raíz, ciclo de vida y health |
| [`EAC.Hosting.AspNetCore`](https://github.com/eac-architecture/eac-hosting-aspnetcore/blob/main/docs/architecture/EAC_HOSTING_ASPNETCORE.md) | adaptación del ciclo de vida y bootstrap para servicios HTTP |
| [`EAC.Testing`](https://github.com/eac-architecture/eac-testing/blob/main/docs/architecture/EAC_TESTING.md) | soporte de pruebas y verificadores arquitectónicos |

`IError`, `Error` y `Result` pertenecen a SharedKernel. La representación HTTP seleccionada es JSON:API mediante `EAC.Infrastructure.Api.JsonApi`; no se crea un paquete general de errores HTTP.

### 4.2 Observabilidad

| Paquete | Responsabilidad |
|---|---|
| [`EAC.Infrastructure.Observability`](https://github.com/eac-architecture/eac-infrastructure-observability/blob/main/docs/architecture/EAC_INFRASTRUCTURE_OBSERVABILITY.md) | behavior de Application, correlación y política de datos sobre OpenTelemetry |
| `EAC.Infrastructure.Auditing` | puertos y persistencia de auditoría técnica |

### 4.3 Seguridad

| Paquete | Responsabilidad |
|---|---|
| [`EAC.Infrastructure.Security`](https://github.com/eac-architecture/eac-infrastructure-security/blob/main/docs/architecture/EAC_INFRASTRUCTURE_SECURITY.md) | identidad neutral y autorización por capacidades; autenticación mediante handlers oficiales |
| `EAC.Infrastructure.Secrets` | integración abstracta con proveedores de secretos |

### 4.4 Persistencia

Todas las familias aplican el [estándar de conexiones y stores de datos](https://github.com/eac-architecture/eac-architecture-docs/blob/main/docs/standards/data/EAC_DATA_STORES.md): `ConnectionProfile → LogicalStore tipado → Mapping técnico → acceso tipado`. Tablas, colecciones, documentos, streams, aliases e índices no son conexiones ni parámetros de Repository, QueryService o Projection Writer.

| Paquete | Responsabilidad |
|---|---|
| [`EAC.Infrastructure.Persistence`](https://github.com/eac-architecture/eac-infrastructure-persistence/blob/main/docs/architecture/EAC_INFRASTRUCTURE_PERSISTENCE.md) | núcleo neutral, transacción local e integración con Runtime |
| [`EAC.Infrastructure.Persistence.EntityFrameworkCore`](https://github.com/eac-architecture/eac-infrastructure-persistence-efcore/blob/main/docs/architecture/EAC_INFRASTRUCTURE_PERSISTENCE_EFCORE.md) | Repository, Unit of Work, QueryService, eventos y Outbox/Inbox sobre EF Core |
| [`EAC.Infrastructure.Persistence.MongoDB`](https://github.com/eac-architecture/eac-infrastructure-persistence-mongodb/blob/main/docs/architecture/EAC_INFRASTRUCTURE_PERSISTENCE_MONGODB.md) | Aggregate Repository, QueryService, atomicidad documental y scope transaccional opcional |
| [`EAC.Infrastructure.Persistence.Marten`](https://github.com/eac-architecture/eac-infrastructure-persistence-marten/blob/main/docs/architecture/EAC_INFRASTRUCTURE_PERSISTENCE_MARTEN.md) | Documents, Event Store, sesión, proyecciones y Outbox/Inbox Marten |
| [`EAC.Infrastructure.Search.Elasticsearch`](https://github.com/eac-architecture/eac-infrastructure-search-elasticsearch/blob/main/docs/architecture/EAC_INFRASTRUCTURE_SEARCH_ELASTICSEARCH.md) | QueryService y Projection Writer mediante bindings tipados; Schema Manager para schema, aliases e índices físicos |
| módulos Outbox/Inbox de cada adaptador Persistence | almacenamiento transaccional que implementa contratos de Messaging |

Cada adaptador tiene dependencias opcionales y ciclo de versión propios. Los providers relacionales de SQL Server, PostgreSQL y MySQL/MariaDB se consumen directamente; no se publican wrappers EAC por motor. Elasticsearch pertenece a Search y no publica Repository ni Unit of Work.

Outbox e Inbox permanecen separados del broker, pero su almacenamiento participa en la transacción local del Command Store mediante un adaptador de Messaging.

### 4.5 Comunicación

| Paquete | Responsabilidad |
|---|---|
| APIs HTTP de .NET | clientes tipados mediante `IHttpClientFactory`, handlers y propagación oficial |
| [`EAC.Infrastructure.Messaging`](https://github.com/eac-architecture/eac-infrastructure-messaging/blob/main/docs/architecture/EAC_INFRASTRUCTURE_MESSAGING.md) | catálogo, envelopes, pipeline, Outbox/Inbox y operación neutral al broker |
| [`EAC.Infrastructure.Messaging.Kafka`](https://github.com/eac-architecture/eac-infrastructure-messaging-kafka/blob/main/docs/architecture/EAC_INFRASTRUCTURE_MESSAGING_KAFKA.md) | adaptador Kafka inicial con topics, partitions, offsets y consumer groups |
| `EAC.Infrastructure.Messaging.*` | adaptadores futuros creados únicamente cuando exista necesidad real |
| Resiliencia de .NET | políticas mediante `Microsoft.Extensions.Http.Resilience` |

EAC Foundation no promete entrega exactamente una vez. Proporciona mecanismos para entrega al menos una vez, idempotencia, Outbox e Inbox.

Messaging no depende de un broker concreto. Los adaptadores Persistence implementan almacenamiento Outbox/Inbox mediante contratos de Messaging, sin publicar directamente. La composición de capacidades corresponde al host consumidor. HTTP y resiliencia utilizan directamente las APIs oficiales mientras no exista una carencia específica demostrada.

### 4.6 API

| Paquete | Responsabilidad |
|---|---|
| [`EAC.Infrastructure.Api.JsonApi`](https://github.com/eac-architecture/eac-infrastructure-api-jsonapi/blob/main/docs/architecture/EAC_INFRASTRUCTURE_API_JSONAPI.md) | documentos, errores, recursos, consultas y mapeo de validación JSON:API 1.1 |
| [`EAC.Infrastructure.Api.JsonApi.OpenApi`](https://github.com/eac-architecture/eac-infrastructure-api-jsonapi-openapi/blob/main/docs/architecture/EAC_INFRASTRUCTURE_API_JSONAPI_OPENAPI.md) | transformers OpenAPI 3.1 de contratos JSON:API |
| [Validation de Application Runtime](https://github.com/eac-architecture/eac-application-runtime/blob/main/docs/architecture/EAC_APPLICATION_VALIDATION.md) | capacidad interna de Runtime, documentada separadamente |
| [Integración JSON:API Validation](https://github.com/eac-architecture/eac-infrastructure-api-jsonapi/blob/main/docs/architecture/EAC_INFRASTRUCTURE_API_JSONAPI_VALIDATION.md) | capacidad interna del paquete JsonApi, documentada separadamente |

## 5. Repositorios

El catálogo tiene identidad de producto, pero sus paquetes pueden evolucionar mediante repositorios independientes:

```text
eac-foundation                         # EAC.Foundation y catálogo
eac-application-runtime                # EAC.Application.Runtime
eac-hosting                            # EAC.Hosting
eac-hosting-aspnetcore                 # EAC.Hosting.AspNetCore
eac-infrastructure-api-jsonapi         # EAC.Infrastructure.Api.JsonApi
eac-infrastructure-api-jsonapi-openapi # EAC.Infrastructure.Api.JsonApi.OpenApi
eac-infrastructure-observability       # EAC.Infrastructure.Observability
eac-infrastructure-security            # EAC.Infrastructure.Security
eac-infrastructure-persistence         # EAC.Infrastructure.Persistence
eac-infrastructure-persistence-efcore  # EAC.Infrastructure.Persistence.EntityFrameworkCore
eac-infrastructure-persistence-mongodb # EAC.Infrastructure.Persistence.MongoDB
eac-infrastructure-persistence-marten  # EAC.Infrastructure.Persistence.Marten
eac-infrastructure-search-elasticsearch # EAC.Infrastructure.Search.Elasticsearch
eac-infrastructure-messaging           # EAC.Infrastructure.Messaging
eac-infrastructure-messaging-kafka     # EAC.Infrastructure.Messaging.Kafka
eac-testing                            # EAC.Testing
```

La regla de artefactos es **un repositorio, un NuGet y un pipeline**. `eac-foundation` publica un único NuGet `EAC.Foundation` y mantiene también su catálogo.

Una capacidad pequeña que solo tiene sentido dentro de otra se implementa como namespace o carpeta interna, no como un segundo paquete. Si necesita convertirse en NuGet, obtiene su propio repositorio y pipeline.

### 5.1 Consolidación recomendada

| Capacidad | Decisión física | Motivo |
|---|---|---|
| SharedKernel, Domain y Application | un repositorio y un NuGet `EAC.Foundation` | no introducen proveedores ni dependencias opcionales; los límites se conservan mediante namespaces |
| Hosting y Hosting.AspNetCore | dos repositorios y dos NuGet | ASP.NET Core no se impone a workers |
| Validation | namespace interno de Application.Runtime | siempre se ejecuta dentro del pipeline del runtime |
| JsonApi Validation | namespace interno de JsonApi | no tiene consumidor independiente |
| JsonApi OpenApi | repositorio y NuGet propios | OpenAPI es opcional |
| núcleo de persistencia | repositorio y NuGet `EAC.Infrastructure.Persistence` | contratos técnicos y behavior sin drivers |
| adaptadores de persistencia | repositorio y NuGet por familia: EF Core, MongoDB y Marten | evitan imponer dependencias opcionales y contienen comportamiento reusable real |
| motores relacionales | sin wrapper EAC por motor | cada servicio consume directamente el provider sobre el adaptador EF Core |
| búsqueda Elasticsearch | repositorio y NuGet `EAC.Infrastructure.Search.Elasticsearch` | pertenece al Query Side y no comparte semántica transaccional |
| núcleo de mensajería | repositorio y NuGet `EAC.Infrastructure.Messaging` | catálogo, Outbox/Inbox y pipeline sin cliente físico |
| brokers de mensajería | repositorio y NuGet por broker implementado | cada broker mantiene dependencias, topología y semántica propias |
| adaptadores de observabilidad | repositorio y NuGet por runtime | dependencias opcionales no contaminan el núcleo |
| adaptadores de seguridad | repositorio y NuGet por proveedor | autenticación concreta permanece opcional |

La consolidación ocurre dentro del NuGet cuando las piezas siempre se consumen juntas. Nunca se utiliza un repositorio con varios `.csproj` empaquetables.

## 6. Versionado y compatibilidad

- cada paquete usa SemVer independiente;
- una matriz publicada declara combinaciones compatibles;
- el producto publica una versión de catálogo que referencia versiones de paquetes, sin obligar a un ensamblado monolítico;
- cambios incompatibles incrementan `MAJOR`;
- APIs obsoletas incluyen alternativa y fecha prevista de retirada;
- un paquete publicado es inmutable;
- símbolos, fuentes, SBOM y procedencia acompañan el artefacto;
- el starter fija versiones compatibles en `Directory.Packages.props`.

Ejemplo de catálogo:

```yaml
foundationVersion: 1.0.0
dotnet: 10.0
packages:
  EAC.Hosting: 1.2.0
  EAC.Infrastructure.Observability: 2.0.1
  EAC.Infrastructure.Security: 1.4.0
  EAC.Infrastructure.Persistence: 1.0.0
  EAC.Infrastructure.Persistence.EntityFrameworkCore: 1.0.0
  EAC.Infrastructure.Messaging: 1.3.0
  EAC.Infrastructure.Messaging.Kafka: 1.0.0
```

## 7. Extensibilidad

Cada capacidad expone:

- opciones tipadas;
- validación de opciones;
- extensiones de registro explícitas;
- interfaces para sustituir adaptadores;
- health checks;
- telemetría mínima;
- pruebas de contrato del adaptador;
- documentación de configuración y fallo.

Una aplicación puede reemplazar un adaptador sin modificar Domain o Application.

## 8. Seguridad de EAC Foundation

- análisis de código, dependencias, secretos y licencias;
- SBOM y firma de paquetes;
- publicación desde pipelines protegidos;
- mínimo privilegio para feeds y repositorios;
- sin credenciales o endpoints de entornos en paquetes;
- threat model por familia sensible;
- respuesta y publicación de parches documentadas;
- compatibilidad con políticas de datos de la aplicación sin inspeccionar payloads completos.

## 9. Calidad y pruebas

Cada paquete debe incluir:

- pruebas unitarias;
- pruebas de contrato de sus extensiones públicas;
- pruebas de integración con su proveedor cuando corresponda;
- pruebas de compatibilidad con versiones .NET soportadas;
- pruebas de configuración inválida;
- pruebas de trimming/AOT cuando el paquete declare soporte;
- análisis de API pública;
- documentación XML en inglés para toda API pública, exigida por compilación;
- objetivos de prueba mediante `DisplayName` y trazabilidad mediante `Trait("Rule", "<rule-id>")`;
- ejemplo mínimo compilable sin dominio de producto.

El repositorio de gobierno ejecuta una aplicación de compatibilidad que combina las versiones declaradas en el catálogo.

Los pipelines fijan una versión estable del SDK .NET 10 en `global.json`. La publicación exige compatibilidad de todas las dependencias críticas con `net10.0` y mantenerse al día con los parches soportados del runtime.

## 10. Límites

EAC Foundation no contiene:

- agregados o entidades de un negocio;
- Commands, Queries o eventos de una solución;
- nombres de bounded contexts;
- roles o permisos funcionales;
- nombres de tópicos o bases de datos de aplicaciones;
- orquestaciones de negocio;
- modelos de lectura de un producto;
- prompts, tools o casos de IA específicos;
- configuración secreta o dependiente de entorno.

## 11. Criterios de aceptación

El diseño del catálogo está preparado para implementación cuando:

- cada paquete tiene responsabilidad y dependencias explícitas;
- abstracciones y adaptadores físicos están separados;
- no existe semántica de una solución consumidora;
- versionado y compatibilidad son verificables;
- el Starter puede seleccionar capacidades sin instalar todo Foundation;
- seguridad, observabilidad y pruebas forman parte de la publicación;
- existe una estrategia de migración para consumidores;
- cada repositorio tiene scripts, binding, ejecución y ownership propios; la
  orquestación común se consume desde EAC Pipeline Catalog.

## 12. Decisión relacionada

La identidad y límites están registrados en [ADR-0011](../decisions/ADR-0011-eac-foundation.md).

## 13. Catálogo objetivo inicial

El inventario y la clasificación detallados se mantienen en el [Catálogo de capacidades](https://github.com/eac-architecture/eac-architecture-docs/blob/main/docs/catalog/CATALOGO_DE_COMPONENTES.md).

| Capacidad | Decisión para EAC Foundation |
|---|---|
| Shared Kernel | mantenerlo mínimo y sin bases de entidad o repositorio universal |
| Configuración | aplicar el [estándar EAC](https://github.com/eac-architecture/eac-architecture-docs/blob/main/docs/standards/configuration/CONFIGURATION_STANDARD.md) mediante Configuration, Options, `IValidateOptions<T>` y `ValidateOnStart` directamente |
| Seguridad | separar autenticación y autorización de los adaptadores concretos |
| HTTP | permanecer independiente de persistencia |
| Persistencia relacional | separar núcleo y un paquete por proveedor |
| Mensajería | separar abstracciones, Outbox, Inbox y adaptador de broker |
| Kafka | mantenerlo como adaptador opcional de mensajería |
| Observabilidad | integrar logs, métricas y trazas mediante OpenTelemetry |

Cada capacidad debe superar revisión de responsabilidad, seguridad, compatibilidad con `net10.0` y pruebas antes de ingresar al catálogo.

## 14. Seguimiento de implementación

F3 está completado y F4 se encuentra en curso. SharedKernel, Domain y Application están validados dentro del mismo ensamblado `EAC.Foundation`. La [auditoría de cierre de Application](https://github.com/eac-architecture/eac-foundation/blob/main/docs/governance/AUDITORIA_CIERRE_APPLICATION.md) aprueba `0.1.0-alpha.17` con 177 pruebas y autoriza PF-005.

El estado de los 16 repositorios y NuGet del catálogo, sus ondas, dependencias, gates y evidencias se mantiene en el [plan de implementación de Platform](https://github.com/eac-architecture/eac-architecture-docs/blob/main/docs/planning/PLAN_DE_IMPLEMENTACION_DE_PLATAFORMA.md). Este documento conserva el diseño y los límites del producto; no replica el tablero operativo.
