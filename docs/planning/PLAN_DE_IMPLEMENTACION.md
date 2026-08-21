# Plan de implementación de EAC.Foundation

> **Orden documental:** DOC-083 · **Etapa:** Planificación · [Índice](../INDICE_DOCUMENTAL.md)

## 1. Alcance

Implementar y publicar un único ensamblado y NuGet `EAC.Foundation` para
`net10.0`. El paquete contiene SharedKernel, Domain y Application como
namespaces cohesionados. No implementa hosting, proveedores, transporte ni
reglas funcionales.

## 2. Incrementos

| ID | Alcance | Estado |
|---|---|---|
| PF-001 | scaffold, gobierno, empaquetado y CI | Validado |
| PF-002 | SharedKernel | Validado |
| PF-003 | Domain | Validado |
| PF-004 | Application | Validado |
| PF-005 | maduración de versiones preliminares hacia `1.0.0` | Validado hasta `0.1.0-rc.3`; estable diferida |

## 2.1 Cierre transversal de configuración

| ID | Alcance | Estado | Evidencia |
|---|---|---|---|
| CFG-001 | Declarar instalación y consumo tipado sin inventar configuración ambiental para un paquete de contratos y primitivas | Completado | README documenta ausencia deliberada de `appsettings`, variables, secretos, recursos implícitos y recarga |

## 3. Próximo incremento

PF-005 se ejecuta mediante estos bloques, en orden:

| Bloque | Alcance | Estado | Evidencia |
|---|---|---|---|
| CI-001 | contrato Bash y Pipeline Tekton para G0-G4 | Completado | Pipeline ejecutado sobre `1098b95`: validación, build y 177 pruebas aprobados |
| CI-002 | eventos de pull request y rama principal con Pipelines as Code | Completado | ejecuciones remotas exitosas mediante el binding reutilizable |
| REL-001 | candidato integrable, SBOM y evidencias para G5-G7, sin publicación | Completado | paquete, símbolos, hashes, SBOM y smoke consumidor generados desde el build verificado |
| REL-002 | integración remota mediante el pipeline NuGet reutilizable | Completado | PipelineRuns exitosas sobre commits remotos; sin Pipelines o Tasks locales |
| REL-003 | publicación gobernada de alpha, beta y RC sin reconstruir el candidato | Completado | `0.1.0-rc.3` publicado mediante Pipeline Catalog |
| REL-004 | promoción estable para G8 | Diferido | NuGet estable e inmutable sujeto a aprobación explícita |

La reconciliación de entrega confirma `EAC.Foundation 0.1.0-rc.3`, tag
`v0.1.0-rc.3` y commit `bed36c9d22a68323c1b6e433df0ea3a7aaa8442d`.
La PipelineRun `eac-nuget-prerelease-publication-run-xbfgf` terminó en
`Succeeded`; registró el paquete SHA-256
`2e8995b2458cfb4e796e7dfee126b4bdc3deac5ea034bd5449921171ff0ef1b1` y el
SBOM SHA-256 `b54361d06dda7a807d7471728f17f0e9798a321e81e0bd1e730c89402f284760`.
El siguiente gate de entrega es exclusivamente la promoción estable coordinada.

Los gates transversales se definen en
[EAC Architecture](https://github.com/eac-architecture/eac-engineering-governance).

Los scripts aceptan cualquier serie SemVer gobernada, desde `alpha.N` hasta el
candidato estable, sin fijar una versión concreta. Pipeline Catalog valida que
la serie coincida con `release/X.Y.Z` antes de entregar el artefacto.
`RELEASE_COMMIT`, cuando se inyecta, identifica exactamente el `HEAD` limpio
que produjo el candidato, conforme al contrato Bash transversal.

La incorporación de una capacidad que pertenezca a otro NuGet requiere su
propio repositorio; no amplía este ensamblado por conveniencia.
