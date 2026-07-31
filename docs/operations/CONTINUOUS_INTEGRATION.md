# Integración continua de EAC.Foundation

> **Orden documental:** DOC-084 · **Etapa:** Operación de producto · [Índice](../INDICE_DOCUMENTAL.md)

## 1. Propósito

Implementar CI-001 para ejecutar los gates G0-G4 de `EAC.Foundation` mediante
el mismo contrato Bash en una estación de desarrollo y en Tekton. Este bloque
no firma, publica ni consume credenciales de NuGet.

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

`pack.sh` no forma parte de `ci.sh`. La generación, verificación, SBOM y
publicación del artefacto pertenecen a los siguientes bloques de PF-005.

## 3. Recursos Tekton

```text
ci/tekton/
├── tasks/
│   ├── checkout.yaml
│   ├── validate.yaml
│   └── verify.yaml
├── pipelines/
│   └── continuous-integration.yaml
├── workspaces/
│   └── source-volume-claim-template.yaml
└── pod-template.yaml
```

### Orden de ejecución

1. `checkout` resuelve una revisión Git y publica el commit SHA.
2. `validate` ejecuta `scripts/validate.sh` sobre ese checkout.
3. `verify` ejecuta `scripts/ci.sh` sin secretos de release.
4. el Pipeline publica resultados pequeños de commit, validación y pruebas.

Las Tasks usan usuarios sin privilegios, eliminan capabilities Linux y no
montan el socket de Docker. Las imágenes se fijan a versiones explícitas.

## 4. Instalación en el namespace de ejecución

```bash
./scripts/apply-tekton-ci.sh
```

Valores predeterminados:

| Variable | Valor |
|---|---|
| `KUBE_CONTEXT` | `kind-eac-cicd` |
| `TEKTON_NAMESPACE` | `eac-cicd` |
| `TEKTON_SERVICE_ACCOUNT` | `eac-ci` |

## 5. Ejecución manual

El repositorio debe existir y la revisión debe estar publicada antes de
iniciar el Pipeline:

```bash
./scripts/run-tekton-ci.sh \
  https://github.com/eac-architecture/eac-foundation.git \
  main
```

Antes de modificar Tekton, el script ejecuta `git ls-remote` sin permitir un
prompt de credenciales. Si el repositorio no existe, es privado o la revisión
no está publicada, termina con un mensaje accionable y no crea un
`PipelineRun` fallido.

Superada esa validación, aplica las definiciones versionadas, crea un PVC
efímero de 2 GiB, inicia el `PipelineRun`, muestra sus logs y devuelve un exit
code de error cuando Tekton no termina correctamente.

## 6. Resultados

| Resultado | Fuente |
|---|---|
| `commit-sha` | revisión inmutable resuelta por checkout |
| `validation-status` | resultado de alcance y gobierno |
| `test-status` | resultado de build y pruebas |

No se escriben tokens, claves, certificados ni URLs con credenciales en Params
o Results.

## 7. Límite del incremento

CI-001 termina cuando el Pipeline se ejecuta correctamente contra una revisión
publicada. CI-002 añadirá los `PipelineRun` de `.tekton/` para pull request y
rama principal mediante Pipelines as Code.
