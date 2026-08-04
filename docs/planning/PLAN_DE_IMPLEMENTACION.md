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
| PF-005 | maduración de versiones preliminares hacia `1.0.0` | En curso |

## 3. Próximo incremento

PF-005 se ejecuta mediante estos bloques, en orden:

| Bloque | Alcance | Estado | Evidencia |
|---|---|---|---|
| CI-001 | contrato Bash y Pipeline Tekton para G0-G4 | Completado | Pipeline ejecutado sobre `1098b95`: validación, build y 177 pruebas aprobados |
| CI-002 | eventos de pull request y rama principal con Pipelines as Code | En curso | binding publicado; registro y evento real pendientes |
| REL-001 | candidatos alpha, beta y rc, SBOM y evidencias para G5-G7 | Completado | `0.1.0-rc.1` validado mediante Tekton: build, 177 pruebas, package, símbolos, SBOM y smoke test |
| REL-002 | firma, procedencia y publicación para G8 | Pendiente | NuGet estable e inmutable |

El siguiente resultado es publicar el prerelease aprobado mediante un tag
inmutable sobre `release/0.1.0`. La generación del candidato se describe en
[Candidato de release](../operations/RELEASE_CANDIDATE.md). CI-002 permanece
abierto hasta comprobar un evento real de Pipelines as Code.

Los gates transversales se definen en
[EAC Architecture](https://github.com/eac-architecture/eac-engineering-governance).

La incorporación de una capacidad que pertenezca a otro NuGet requiere su
propio repositorio; no amplía este ensamblado por conveniencia.
