# Integración continua de EAC.Foundation

> **Orden documental:** DOC-084 · **Etapa:** Operación de producto · [Índice](../INDICE_DOCUMENTAL.md)

## 1. Propósito

Ejecutar los gates G0-G4 de `EAC.Foundation` mediante el mismo contrato Bash
en una estación de desarrollo y en Tekton. Foundation consume el perfil
compartido `nuget-ci` de EAC Pipeline Catalog; no mantiene una copia de
las Tasks ni de la Pipeline. Este bloque no firma, publica ni consume
credenciales de NuGet.

## 2. Contrato Bash

| Entrada | Responsabilidad |
|---|---|
| `scripts/validate.sh` | archivos obligatorios, identidad, versión, framework, excepciones y documentación |
| `scripts/build.sh` | restore bloqueado, formato y build sin warnings |
| `scripts/test.sh` | pruebas unitarias, contractuales y de arquitectura |
| `scripts/ci.sh` | composición local de validate, build y test |
| `scripts/pack.sh` | empaquetado local explícito, fuera de CI-001 |

La ejecución rápida es:

```bash
./scripts/ci.sh
```

`pack.sh` no forma parte de `ci.sh`. La generación, verificación y SBOM se
describen en [Candidato de release](RELEASE_CANDIDATE.md); la publicación
pertenece a REL-002.

## 3. Binding de Foundation

```text
.tekton/
└── continuous-integration.yaml
```

El archivo `.tekton/continuous-integration.yaml` contiene únicamente:

- los eventos `pull_request` y `push` dirigidos a `main`;
- los parámetros dinámicos de repositorio y commit;
- el workspace efímero y la Service Account de CI;
- la referencia inmutable a EAC Pipeline Catalog `v0.1.0`.

La Pipeline `eac-nuget-ci` y sus Tasks pertenecen al repositorio
`eac-pipeline-catalog`.

### Orden de ejecución

1. `checkout` resuelve una revisión Git y publica el commit SHA.
2. `validate` ejecuta `scripts/validate.sh` sobre ese checkout.
3. `build` ejecuta `scripts/build.sh` una sola vez.
4. `test` ejecuta `scripts/test.sh --no-build` sobre el resultado anterior.
5. la Pipeline publica resultados pequeños de commit, validación, build y
   pruebas.

Las Tasks usan usuarios sin privilegios, eliminan capabilities Linux y no
montan el socket de Docker. Las imágenes se fijan a versiones explícitas.

## 4. Ejecución

Existen tres entradas al mismo contrato:

1. `scripts/ci.sh` ejecuta el ciclo rápido directamente en la estación de
   desarrollo;
2. un `pull_request` o `push` hacia `main` activa el `PipelineRun` mediante
   Pipelines as Code;
3. `eac-pipeline-catalog/scripts/run-ci.sh` permite iniciar manualmente la
   Pipeline instalada, indicando el repositorio y la revisión.

Foundation no instala Tasks/Pipelines ni contiene un iniciador Tekton manual.
Pipelines as Code resuelve la Pipeline remota fijada a `v0.1.0`, enlaza
`{{source_url}}` y `{{revision}}`, y genera un `PipelineRun` autocontenido. La
instalación y la ejecución manual pertenecen a `eac-pipeline-catalog`.

## 5. Resultados

| Resultado | Fuente |
|---|---|
| `commit-sha` | revisión inmutable resuelta por checkout |
| `validation-status` | resultado de alcance y gobierno |
| `build-status` | resultado del contrato de build |
| `test-status` | resultado de build y pruebas |

No se escriben tokens, claves, certificados ni URLs con credenciales en Params
o Results.

## 6. Límite del incremento

CI-001 y la ejecución manual del perfil compartido quedaron validados contra
la revisión `c524e72`: build sin warnings y 177 pruebas aprobadas. El binding
de CI-002 referencia EAC Pipeline Catalog `v0.1.0`; su cierre requiere
restaurar el recurso `Repository` y comprobar un evento real de Pipelines as
Code.
