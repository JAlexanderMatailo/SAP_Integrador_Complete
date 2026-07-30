# IntegradorSAP — Arquitectura y guía de conexión

Integrador entre APIs externas y **SAP Business One**. No tiene interfaz ni
usuarios propios: recibe JSON por HTTP, habla con SAP y devuelve el resultado.

## Índice

1. [Los dos canales hacia SAP](#1-los-dos-canales-hacia-sap)
2. [Cómo debe conectarse tu API](#2-cómo-debe-conectarse-tu-api)
3. [Recorrido de una petición](#3-recorrido-de-una-petición)
4. [Qué hace cada pieza](#4-qué-hace-cada-pieza)
5. [Dónde va cada configuración](#5-dónde-va-cada-configuración)
6. [Manejo de errores](#6-manejo-de-errores)
7. [Ciclo de vida de la sesión de SAP](#7-ciclo-de-vida-de-la-sesión-de-sap)
8. [Añadir un endpoint nuevo](#8-añadir-un-endpoint-nuevo)
9. [Cosas que NO hay que tocar](#9-cosas-que-no-hay-que-tocar)
10. [Deuda técnica conocida](#10-deuda-técnica-conocida)

---

## 1. Los dos canales hacia SAP

Esto es lo primero que hay que entender: el integrador habla con SAP por **dos
vías distintas, con credenciales de naturaleza distinta**.

| | Service Layer | HANA directo |
|---|---|---|
| Para qué | leer y **escribir** documentos | **solo leer** (consultas rápidas y vistas) |
| Protocolo | HTTPS / OData v3 | ADO.NET (`Sap.Data.Hana`) |
| Credenciales | **usuario de aplicación de SAP B1**, lo envía tu API por cabecera | **usuario técnico de base de datos**, en `Web.config` |
| Clase | `Helper/ServiceLayer_Web.cs` | `Helper/SrvHana.cs` + `DataAccess/DAO.cs` |
| Uso en el proyecto | ~417 referencias en 14 managers | 30 llamadas en 8 managers |

Regla práctica: **todo lo que escribe en SAP va por Service Layer.** HANA se usa
solo para leer, porque es mucho más rápido para consultas y vistas.

---

## 2. Cómo debe conectarse tu API

### 2.1 Cabeceras

```http
POST /api/sap/GuardarOrdenVentaCTK HTTP/1.1
Host: servidor-integrador:9097
Content-Type: application/json
X-SAP-User: <usuario de SAP Business One>
X-SAP-Password: <su clave>
```

- Las dos cabeceras van **juntas o ninguna**. Si envías solo una, la respuesta es
  **400**: se hace a propósito, porque ignorarlo en silencio haría que la
  operación se ejecutara con un usuario distinto del que pediste.
- Si no envías ninguna, el integrador usa las credenciales de respaldo de
  `Config/sapCompanies.config` (pensado para pruebas o migración progresiva).
- La clave **no se registra en ningún log** y solo vive en memoria durante la
  petición.

### 2.2 El CompanyDB

La base de SAP **no está fijada en el integrador**: llega en cada petición.

- **En la ruta**, en 38 endpoints: `GET /api/sap/ConsultarCentroDeCostoPorCodigo/{codigo}/{CompanyDB}`
- **En el cuerpo**, en los POST: el modelo lleva `"CompanyDB": "SAP_IKO"`

Se valida antes de tocar SAP o HANA (ver [§5.2](#52-lista-blanca-de-empresas)).

### 2.3 Swagger

| URL | Qué es |
|---|---|
| `/swagger` | la interfaz para explorar y probar |
| `/swagger/docs/v1` | el documento OpenAPI en JSON |

Las cabeceras `X-SAP-User` y `X-SAP-Password` aparecen en **todas** las
operaciones, así que se pueden escribir desde la interfaz y probar contra SAP.
No son parámetros de acción (las lee `SapCredentialsHandler`), por eso las añade
el filtro `CabecerasCredencialesSap` de `App_Start/SwaggerConfig.cs`.

Se usa **Swashbuckle 5.6 clásico**, no `Swashbuckle.AspNetCore`: este proyecto es
Web API 2 sobre .NET Framework 4.8 y el paquete de AspNetCore solo sirve en
.NET Core. Se registra desde `WebApiConfig.Register`, no con `WebActivatorEx`.

Las descripciones salen de los comentarios `///`, para lo cual ambos proyectos
generan `DocumentationFile` (con `NoWarn 1591`, porque el código heredado no
tiene comentarios en todos sus miembros públicos).

### 2.4 Formato de la respuesta

**Todos** los endpoints devuelven la misma forma, `RespuestaGenerica`:

```json
{
  "Success": false,
  "RespuestaJson": null,
  "ErrCodigo": -10,
  "ErrMensaje": "The remote server returned an error: (400) Bad Request.\n\n\n{\"error\":{\"code\":-10,\"message\":{\"lang\":\"en-us\",\"value\":\"Business partner code is missing\"}}}",
  "ErrMensajeSap": "Business partner code is missing",
  "Error": { "error": { "code": -10, "message": { "lang": "en-us", "value": "Business partner code is missing" } } },
  "HttpStatus": 400,
  "SesionExpirada": false
}
```

En éxito, `Success = true` y `RespuestaJson` trae el JSON que devolvió SAP.

**Para leer el error, usa `ErrMensajeSap` o `Error.error.message.value`.**
`ErrMensaje` conserva el JSON crudo embebido **solo por compatibilidad**: el
consumidor actual lo extrae con `Regex.Match(mensaje, @"\{.*\}")`. Es frágil y
conviene migrar a los campos estructurados.

---

## 3. Recorrido de una petición

```
tu API
  │  POST /api/sap/...  + X-SAP-User / X-SAP-Password
  ▼
RutasHeredadasHandler      reescribe /api/CTK/... a /api/sap/... (compatibilidad)
  ▼
SapCredentialsHandler      lee las cabeceras -> SapRequestContext
  ▼                        (al terminar: cierra la sesión de SAP y limpia)
Controller                 valida y delega
  ▼
Manager                    la lógica de negocio
  ├──► BaseManager.Login ──► ServiceLayer_Web.ConectarSL
  │                            └─ LoginSap ─► SapCompanyCredentials.Obtener
  │                                             ├─ 1º SapRequestContext (cabeceras)
  │                                             └─ 2º appSettings (respaldo)
  ├──► ServiceLayer_Web.SLSendRequestReturnResponse   escritura/lectura vía OData
  └──► DAO.conectarHana ──► SrvHana.conectar          lectura directa en HANA
  ▼
RespuestaGenerica          de vuelta a tu API
```

Si algo lanza una excepción no controlada, la intercepta
`ErroresNoControladosFilter` y la convierte en `RespuestaGenerica` (ver [§6](#6-manejo-de-errores)).

---

## 4. Qué hace cada pieza

### Proyecto `IntegradorSAP.WebApi` — la fachada HTTP

| Archivo | Responsabilidad |
|---|---|
| `App_Start/WebApiConfig.cs` | registra los handlers, el filtro de errores y las rutas. **Punto de entrada de toda la configuración de tubería.** |
| `Controllers/*.cs` | 15 controllers. Solo validan y delegan al manager: no deben tener lógica de negocio. |
| `Web.config` | conexión a HANA, URL del Service Layer, timeout. Incluye `Config/sapCompanies.config`. |
| `NLog.config` | destino de log de NLog. |

**Orden de los handlers en `WebApiConfig`, y por qué importa:**

1. `RutasHeredadasHandler` — reescribe la ruta. Va **primero**: los siguientes ya
   ven la ruta nueva.
2. `SapCredentialsHandler` — lee credenciales y, al terminar, cierra la sesión.

### Proyecto `IntegradorSAP.Services` (ensamblado `IntegradorSAP.Data`)

#### `Helper/` — la infraestructura

| Archivo | Qué hace | Con quién se conecta |
|---|---|---|
| `ServiceLayer_Web.cs` | **el corazón**. Login/logout, envío de peticiones OData, cookies de sesión, traducción de errores de SAP. | lo usan los 12 managers; usa `LoginSap`, `SapRequestContext`, `LogGeneral` |
| `SrvHana.cs` | abre la conexión ADO.NET a HANA con el CompanyDB como esquema. | lo usa `DAO` |
| `SapCompanyCredentials.cs` | resuelve credenciales (cabecera → configuración) y **valida el CompanyDB**. | lo usan `LoginSap` y `DAO` |
| `SapRequestContext.cs` | guarda credenciales y cookie de sesión **durante la petición**. | `SapCredentialsHandler`, `ServiceLayer_Web`, `SapCompanyCredentials` |
| `SapCredentialsHandler.cs` | lee `X-SAP-*`; al terminar cierra la sesión de SAP. | registrado en `WebApiConfig` |
| `RutasHeredadasHandler.cs` | `/api/CTK/*` → `/api/sap/*`. **Temporal**, ver [§9](#9-cosas-que-no-hay-que-tocar). | registrado en `WebApiConfig` |
| `ErroresNoControladosFilter.cs` | excepción no controlada → `RespuestaGenerica` + log. | registrado en `WebApiConfig` |
| `SrvHana - Copia.cs` | **define `LogGeneral`**, el log de archivo. El nombre engaña: **está en uso, no borrar.** | todo el proyecto |

#### `DataAccess/DAO.cs`

Clase base de los managers para HANA. Valida el CompanyDB, abre la conexión y
ofrece las sobrecargas de `LiberarVariables` que cierran reader, command y
conexión en el `finally`.

#### `Manager/` — la lógica de negocio

Cada manager hereda de `BaseManager` (que hereda de `DAO`), así que dispone de
las dos vías: `servicio.*` para Service Layer y `conectarHana()` para HANA.

| Manager | Ámbito |
|---|---|
| `ComercialManager` | órdenes de venta, socios de negocio, costos locales |
| `OrdenesVentaManager` / `OrdenesCompraManager` | pedidos |
| `FacturacionLoteManager` | facturación en lote (el más complejo) |
| `NotaDebitoLoteManager` / `NotasCreditoManager` | notas |
| `InventarioManager` / `SalidasInventarioManager` | stock y movimientos |
| `CatalogosManager` | artículos, centros de costo, maestros |
| `ContabilidadManager` | asientos contables |
| `TransferenciasBancariasManager` | pagos y transferencias |
| `FuncionesComunesManager` | utilidades compartidas |
| `LogInOutServiceLayerManager` | login/logout explícito |

#### `Models/` — los contratos

- `RespuestaGenerica.cs` — la respuesta de **todos** los endpoints.
- `LoginSap.cs` — cuerpo del `POST /Login`. **Los nombres son contrato con SAP.**
- `ItemsViewModel.cs` — el maestro de artículos (ver
  [CAMPOS-PRODUCTOS.md](CAMPOS-PRODUCTOS.md)).
- El resto, un modelo por documento de SAP.

---

## 5. Dónde va cada configuración

### 5.1 `IntegradorSAP.WebApi/Web.config` → `<appSettings>`

| Clave | Para qué |
|---|---|
| `ServerSSL` | URL base del Service Layer, p.ej. `https://servidor:50000/b1s/v1/` |
| `ServerHana` / `PortHana` | servidor y puerto de HANA (normalmente `30015`) |
| `UserHana` / `PwdHana` | **usuario técnico de base de datos**, no el de SAP B1 |
| `Sap.TimeoutMs` | timeout de las llamadas al Service Layer, por defecto `100000` |
| `Sap.DetalleErroresEnRespuesta` | `true` añade el detalle técnico al cuerpo del error. **Solo para depurar.** |
| `IVA` | grupo de impuestos por defecto |

### 5.2 `IntegradorSAP.WebApi/Config/sapCompanies.config`

**No se versiona.** Copiar de `sapCompanies.example.config`.

```xml
<appSettings>
  <!-- OBLIGATORIO: lista blanca de empresas habilitadas -->
  <add key="Sap.CompanyDbPermitidas" value="SAP_MAXX,SAP_IKO,SAP_AUT,SAP_STO" />

  <!-- OPCIONAL: credenciales de respaldo, formato usuario|clave -->
  <!-- <add key="Sap.Login.SAP_IKO" value="usuario|clave" /> -->
</appSettings>
```

**Por qué la lista blanca es su propia clave y no se deduce de las credenciales:**
el CompanyDB llega por parámetro y se usa como **nombre de esquema** en HANA,
tanto en la cadena de conexión como concatenado al SQL. Un nombre de esquema
**no se puede pasar como `HanaParameter`** —SQL no admite identificadores
parametrizados—, así que la defensa es validar. `SapCompanyCredentials.Validar`
rechaza vacío, rechaza cualquier carácter que no sea letra/dígito/`_`, y exige
que esté en la lista.

Se invoca en los dos únicos puntos por los que pasa todo:
`DAO.conectarHana()` (toda consulta HANA) y el constructor de `LoginSap` (todo
login al Service Layer).

### 5.3 `NLog.config`

**Pendiente:** el layout es `${longdate} ${level} ${message}`, **sin
`${exception}`**, así que los stack traces no llegan al archivo aunque se use la
sobrecarga `_logger.Error(ex, ...)`. Añadir `${exception:format=ToString}`.

---

## 6. Manejo de errores

Tres niveles, de dentro hacia fuera:

### Nivel 1 — errores de negocio de SAP

SAP responde 4xx con un cuerpo JSON de forma fija. `InterpretarWebException` en
`ServiceLayer_Web` lo deserializa y rellena:

- `Error` → la estructura completa (`ErrorSSL`)
- `ErrCodigo` → el código de SAP (p.ej. `-10`)
- `ErrMensajeSap` → el texto de SAP aislado
- `HttpStatus` → el código HTTP
- `SesionExpirada` → `true` si fue 401
- `ErrMensaje` → mensaje + JSON crudo, **solo por compatibilidad**

Todo se registra en el log con `[SL] {método} {url} - HTTP {n} - codigoSap={n}`.

### Nivel 2 — errores del propio integrador

| Código | Significado |
|---|---|
| `-1000` | el Service Layer respondió, pero con un StatusCode distinto del esperado |
| `-2000` | fallo de transporte: red, TLS, timeout |
| `-3000` | excepción no controlada, la capturó el filtro global |

### Nivel 3 — excepciones no controladas

`ErroresNoControladosFilter` las convierte en `RespuestaGenerica` con el mismo
contrato JSON, y mapea:

| Excepción | HTTP | Motivo |
|---|---|---|
| `ArgumentException` | **400** | culpa del llamador (CompanyDB inválido o no habilitado) |
| `ConfigurationErrorsException` | **500** | falta configuración en el servidor |
| `TimeoutException`, `WebException` | **504** | SAP no respondió |
| resto | **500** | con un identificador `E<fecha>-<n>` para cruzar con el log |

**El detalle técnico va siempre al log y nunca al cuerpo de la respuesta**, salvo
que se active `Sap.DetalleErroresEnRespuesta`. `ErrException` está marcado
`[JsonIgnore]`: antes se serializaba y los stack traces salían hacia el
consumidor.

---

## 7. Ciclo de vida de la sesión de SAP

El Service Layer de SAP B1 tiene un **tope de sesiones concurrentes licenciadas**
y las sesiones expiran a los ~30 minutos. Gestionarlas mal no se nota en pruebas
y tumba producción.

**Modelo actual: una sesión por petición HTTP.**

1. El primer manager que necesita SAP llama a `ConectarSL`, que hace `POST /Login`.
2. La cookie (`B1SESSION` y `ROUTEID`) se guarda en `SapRequestContext`.
3. Los demás managers de esa petición **reutilizan** la sesión: `IsConected` se
   deriva de que exista la cookie.
4. Al terminar, `SapCredentialsHandler` hace `POST /Logout` y limpia el contexto.

**Por qué se hizo así:** la cookie vivía como campo de instancia de
`ServiceLayer_Web`, y como cada manager construye el suyo y los controllers
exponen `_service => new XManager()` (instancia nueva en **cada acceso**), una
sola petición abría varias sesiones. En los bucles era peor: un `foreach` con N
elementos hacía **1+N logins**, y **ninguno se cerraba** —`DesconectarSL()`
existía sin una sola llamada en todo el proyecto.

`ROUTEID` importa si el Service Layer está tras un balanceador: sin esa cookie
las peticiones pueden caer en otro nodo y la sesión deja de valer.

---

## 8. Añadir un endpoint nuevo

1. **Modelo** en `Models/`. Si lleva CompanyDB, `public string CompanyDB { get; set; }`.
2. **Método en el manager** del ámbito que toque:
   - Escribir en SAP → `servicio.SLSendRequestReturnResponse(recurso, "POST", json, HttpStatusCode.Created, false)`
   - Leer de HANA → `conectarHana(CompanyDB)`, `HanaCommand` **con `HanaParameter`**, y `LiberarVariables` en el `finally`
3. **Acción en el controller**: `[Route("MiEndpoint")]`, devolver `RespuestaGenerica`.
   **No pongas `try/catch`** solo para reenvolver la excepción: el filtro global
   ya lo hace y sin perder el stack trace.
4. Si consultas listas grandes por Service Layer, **pagina**: SAP devuelve 20
   registros por defecto y un `odata.nextLink`. Hoy no hay nada que lo siga.

---

## 9. Cosas que NO hay que tocar

| Elemento | Por qué |
|---|---|
| `U_CTK_*`, `U_EXX_*` | son nombres de campo reales en SAP y viajan como claves JSON |
| `CTK_GET_*_VIEW`, `CTK_STOCKMATERIALESVIEW`, `CTKItemsServiciosSeaboardView`, `SP_INSERT_PAGO_CTKFact` | vistas y procedimientos reales en HANA |
| `CTK_DocEntryRel`, `CTK_DocNumRel` | se serializan a SAP en el `PATCH Orders(...)`; el nombre de la propiedad **es** el del campo |
| La errata **`StatudCode`** | `ComercialController` compara ese texto literal para decidir si una anulación fue correcta. Corregirlo rompe la lógica en silencio. |
| Nombres de propiedad de `LoginSap` | contrato del `POST /Login` |
| `SrvHana - Copia.cs` | define `LogGeneral`, está en uso |
| `RutasHeredadasHandler` | quitarlo rompe al consumidor actual, que usa `/api/CTK` en 6 sitios. Retirar **solo** cuando el log no registre más usos. |

---

## 10. Deuda técnica conocida

Por impacto:

1. **Sin recuperación de sesión expirada.** `SesionExpirada` ya se detecta, pero
   nadie reintenta el login. En procesos largos (facturación en lote) la sesión
   caduca a mitad y todo lo siguiente falla.
2. **10 consultas concatenan datos del usuario al SQL** en vez de usar
   `HanaParameter`: `ComercialManager` 108, 164, 1261; `FacturacionLoteManager`
   212, 264, 381; `InventarioManager` 504; `TransferenciasBancariasManager` 159.
   Más dos interpolaciones en `$filter` de OData (`FacturacionLoteManager` 240, 353).
3. **Parseo de cookies frágil.** `Set-Cookie` se procesa con
   `Replace(',',';')` + `Split('=')`, y **si el valor de la cookie contiene `=`
   la sesión se descarta en silencio**. También mete cookies basura (`path=/b1s`).
4. **Sin paginación** en las consultas de listas por Service Layer.
5. **`NLog.config` sin `${exception}`**: los stack traces no llegan al archivo.
6. **`_service => new XManager()`** en 13 controllers: instancia nueva en cada
   acceso. Ya no cuesta logins, pero sí una conexión HANA por instancia.
7. **Control de flujo por texto de error**: `mensaje.Contains("Error")` en 8
   sitios. Ahora que existen `ErrCodigo`, `HttpStatus` y `Error`, se puede
   decidir sobre campos tipados.
8. **Sin idempotencia** en los POST: un reintento del cliente duplica documentos
   en SAP.
9. `LiberarVariables` usa `catch (Exception) { }` vacío.
10. `SrvHana` gira en un `while(true)` contando hasta 100.000 en lugar de esperar.

---

## Puesta en marcha

```bat
nuget restore IntegradorSAP.sln
msbuild IntegradorSAP.sln /t:Rebuild /p:Configuration=Release

copy IntegradorSAP.WebApi\Config\sapCompanies.example.config ^
     IntegradorSAP.WebApi\Config\sapCompanies.config
```

Requiere el **cliente SAP HANA** instalado
(`%ProgramFiles%\SAP\hdbclient\ado.net\v4.5\Sap.Data.Hana.v4.5.dll`).
Si está en otra ruta: `msbuild /p:HanaClientPath="D:\SAP\hdbclient\ado.net\v4.5"`.

Después, editar `sapCompanies.config` y poner las empresas habilitadas.

Ver también: [MIGRACION.md](../MIGRACION.md) ·
[CAMPOS-PRODUCTOS.md](CAMPOS-PRODUCTOS.md)
