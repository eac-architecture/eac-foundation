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
| PF-005 | release estable `1.0.0` | En curso |

## 3. Próximo incremento

PF-005 se ejecuta mediante estos bloques, en orden:

| Bloque | Alcance | Estado | Evidencia |
|---|---|---|---|
| CI-001 | contrato Bash y Pipeline Tekton para G0-G4 | En validación | build y 177 pruebas locales aprobados; Tasks y Pipeline aplicados al clúster local |
| CI-002 | eventos de pull request y rama principal con Pipelines as Code | Pendiente | `PipelineRun` versionados en `.tekton/` |
| REL-001 | package, SBOM y evidencias para G5-G7 | Pendiente | candidato de release verificable |
| REL-002 | firma, procedencia y publicación para G8 | Pendiente | NuGet estable e inmutable |

CI-001 se cierra al ejecutar el Pipeline contra una revisión publicada. El
repositorio aún no tiene un commit ni un remoto configurado; crear y publicar
esa revisión es el prerrequisito inmediato. La definición y los comandos de
operación están en
[Integración continua](../operations/CONTINUOUS_INTEGRATION.md).

Los gates transversales se definen en
[EAC Architecture](https://github.com/eac-architecture/eac-architecture-docs).

La incorporación de una capacidad que pertenezca a otro NuGet requiere su
propio repositorio; no amplía este ensamblado por conveniencia.
