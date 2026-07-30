# Migración de marca → `IntegradorSAP`

Clon de `Ecuador.IntegradorSapSSL` con la identidad de código renombrada de
Citikold / Apptelink / CTK a **SAP**, preservando intactos todos los contratos
externos.

Regla que gobierna toda la migración:

> Se renombra lo que solo existe dentro de este código.
> **No** se renombra nada que viva fuera: SAP, HANA, SQL Server o los clientes HTTP.

---

## 1. Lo que cambió

| Antes | Ahora |
|---|---|
| carpeta raíz `Ecuador.IntegradorSapSSL` | `IntegradorSAP` |
| `Citikold.IntegradorSapSL.sln` | `IntegradorSAP.sln` |
| `Citikold.IntegradorSapSL.WebApi` | `IntegradorSAP.WebApi` |
| `Citikold.IntegradorSapSL.Services` | `IntegradorSAP.Services` |
| `Citikold.IntegradorSapSL.Data.csproj` | `IntegradorSAP.Data.csproj` |
| namespace `Citikold.IntegradorSapSL.*` | `IntegradorSAP.*` |
| `CTKController` / `CTKManager` / `CTKViewModel` | `ComercialController` / `ComercialManager` / `ComercialViewModel` |
| `OrdenesCompraCTKController` | `OrdenesCompraController` |
| `FacturaCitikoldModel` y variantes `*Citikold*Model` | `FacturaVentaModel` y variantes `*Venta*Model` |
| `NotaDebitoCitikoldModel` | `NotaDebitoModel` |
| `NotaCreditoCTKModel` | `NotaCreditoModel` |
| `OrdenVentaCTKGuardar` | `OrdenVentaGuardarRequest` |
| `OrdenVentaCancelarCTK` | `OrdenVentaCancelarRequest` |
| `OrdenesVentaCancelarCTK` | `OrdenesVentaCancelarItem` |
| `SocioNegocioCTK` / `CostoLocalCTK` | `SocioNegocioRequest` / `CostoLocalRequest` |
| `RespSocioNegocioCTK` / `RespCostoLocalCTK` | `RespSocioNegocio` / `RespCostoLocal` |
| `GuardarOrdenVentaCTK` (método) | `GuardarOrdenVentaComercial` |
| `GuardarOrdenVentaCTKMarlon` (método) | `GuardarOrdenVentaTurnos` |
| `GuardarOrdenCitikoldCostosLocalesVenta` | `GuardarOrdenCostosLocalesVenta` |

**Proyectos eliminados:** `Apptelink.Infrastructure` y `Apptelink.Infrastructure.Data`
(137 archivos). Se verificó que **ningún** archivo de WebApi ni de Services los
referenciaba: 0 usos en código, `.config` y vistas. Tampoco estaban en ningún
`ProjectReference`. Solo figuraban en el `.sln`.

También se eliminó un `.sln` duplicado que estaba dentro de la carpeta WebApi y
apuntaba a los proyectos Apptelink.

---

## 1.b Segunda pasada: nombres de empresa fuera del código

Motivo: a esta API se conecta otra API distinta, ajena a esas empresas, y va a
atender varias empresas con bases de SAP diferentes. Por tanto los nombres de
empresa dejan de ser un contrato que preservar y pasan a ser datos de despliegue.

| Antes | Ahora |
|---|---|
| `OrdenesVentaCititugController` | `OrdenesVentaBasicaController` |
| `RoutePrefix("api/OrdenesVentaCititug")` | `api/OrdenesVentaBasica` |
| `OrdenVentaCititugGuardar` | `OrdenVentaBasicaGuardar` |
| `DocumentlineCititugVta` | `DocumentlineVentaBasica` |
| `GuardarOrdenVentaCititug` | `GuardarOrdenVentaBasica` |
| `PostOVGuardarCititug` (método y ruta) | `PostOVGuardarBasica` |
| `Route("PostOVCitikoldCostosLocales")` | `PostOVCostosLocales` |

Se eligió *Basica* porque ese modelo es una versión reducida de
`OrdenVentaGuardar`: la mayoría de campos están comentados y no lleva UDFs
`U_EXX_*`. El nombre describe la forma del payload, no una empresa.

### Credenciales y CompanyDB: de código a configuración

- `LoginSap(string CompanyDB)` era una cadena de 15 `else if` con nombres de
  empresa y contraseñas en claro. Ahora resuelve contra
  `Helper/SapCompanyCredentials.cs`, que lee `appSettings`.
- `GetCompanies()` — duplicado en `LogInOutServiceLayerManager` y
  `TransferenciasBancariasManager`, con otras 16 credenciales — ahora enumera lo
  que haya en configuración. (Ambos métodos eran código muerto: sin referencias.)
- Se eliminó el enum `EnumCompanyDB`, que fijaba cinco nombres de empresa. Sin
  referencias.
- Se eliminó la sobrecarga `SrvHana.conectar()` sin parámetros y
  `DAO.conectarHana()` sin parámetros. Sin referencias, y la primera fijaba
  `Current Schema` al esquema de una empresa: en un integrador multiempresa una
  conexión que ignora el CompanyDB recibido acabaría leyendo o escribiendo en la
  empresa equivocada.
- Se dejó de registrar la cadena de conexión completa en `log.txt`: llevaba la
  contraseña de HANA en claro. Ahora solo servidor, usuario y esquema.
- Se eliminó un `connectionStrings/DefaultContext` (ya comentado) con el usuario
  `sa` de SQL Server y su contraseña. No hay ningún `DbContext` en la solución.
- Los 25 comentarios muertos del tipo `//"NOMBRE_BASE"` y
  `// string CompanyDB = "..."` se borraron.
- Los mensajes que nombraban la razón social del cliente se redactaron por su
  regla real: la condición es `U_TipoFacturacion == "A"`, así que el texto ahora
  dice que el cliente está configurado con facturación agrupada. Es texto que el
  usuario final ve en `U_CTK_Observacion`.

### Las credenciales de SAP llegan de la API llamadora

El integrador **no custodia credenciales de SAP**. La API que lo consume las
envía en las cabeceras de cada petición:

```
X-SAP-User      usuario de SAP Business One
X-SAP-Password  su clave
```

El CompanyDB sigue llegando como siempre (en la ruta o en el cuerpo); no se
duplica en cabecera para no tener dos fuentes de verdad del mismo dato.

Se eligieron cabeceras y no el cuerpo por dos razones concretas: no hay que tocar
ninguna de las 85 rutas ni los 24 modelos de request, y funciona igual en GET que
en POST — de haberlo puesto en el cuerpo, las 38 rutas GET que reciben el
CompanyDB en la URL habrían acabado con la clave en la URL, es decir en los logs
de IIS.

El recorrido, siguiendo la forma que ya existía en
`LogInOutServiceLayer.LoginOnServiceLayer(CompanyDB, UserSap, PassUserSap)`
—las credenciales llegan y luego se usan— es:

| Paso | Quién |
|---|---|
| Lee las cabeceras y las deja en el contexto de la petición | `SapCredentialsHandler` (message handler, registrado en `WebApiConfig`) |
| Guarda y expone las credenciales durante la petición | `SapRequestContext` |
| Resuelve qué credenciales usar | `SapCompanyCredentials.Obtener` |
| Arma el `POST /Login` hacia SAP | `LoginSap` → `ServiceLayer_Web.ConectarSL` |

Detalles que importan:

- **Se usa `DelegatingHandler`, no `ActionFilter`**, porque corre antes del enlace
  de modelos y cubre las 85 rutas por igual.
- El contexto se guarda **a la vez** en `HttpContext.Current.Items` y en el
  `CallContext` lógico. Varios managers usan métodos `async` y
  `HttpContext.Current` puede quedar en null en una continuación; con los dos, la
  resolución no depende de ese detalle.
- Si las cabeceras vienen **a medias** (una sí y otra no) se responde **400**. Si
  se ignorara en silencio se caería al respaldo de configuración y la petición se
  ejecutaría con un usuario distinto del pedido.
- Si **no vienen**, se recurre al respaldo por configuración (`Sap.Login.*`), para
  que un consumidor que aún no las envíe siga funcionando.
- La clave **nunca** se registra ni se devuelve: solo vive en memoria durante la
  petición y se limpia al terminar. Se verificó que ningún log escribe el cuerpo
  del login.
- La cookie de sesión de SAP (`HANA_SL_COOKIES`) es un campo **de instancia**, no
  `static`, así que la sesión de un llamador no puede filtrarse a otro.

**Las credenciales de HANA son otra cosa** y siguen en `Web.config`
(`UserHana` / `PwdHana`): son de un usuario técnico de base de datos, no del
negocio. Las usan las 30 consultas `SELECT` directas de 8 managers.

### El login fallido pasó a ser un caso normal

Al venir las credenciales del llamador, una clave incorrecta ya no es un error de
configuración sino rutina. El código anterior no servía para eso:

```csharp
Console.WriteLine(respuesta.ErrMensaje + "\n");
Console.WriteLine(respuesta.ErrException.StackTrace);   // NullReferenceException
```

Dos defectos en tres métodos (`ConectarSL`, `ConectarSLAsync`,
`ConectarSLV2Async`) y uno más en `DesconectarSL`:

1. En IIS `Console.WriteLine` no va a ninguna parte, así que el motivo del fallo
   se perdía y el llamador solo recibía "No se pudo establecer conexión".
2. `ErrException` queda en **null** cuando el fallo viene por StatusCode
   inesperado (el camino `-1000`), así que era un `NullReferenceException` que
   tapaba el error real. En `DesconectarSL` era especialmente alcanzable: el
   Logout espera `204 NoContent`, y un `200 OK` entraba justo por ahí.

Ahora hay un `RegistrarResultadoLogin` que comprueba el null, registra por
`LogGeneral` y deja el motivo en `ErrMessage`, distinguiendo clave incorrecta de
CompanyDB inexistente o de red caída.

### Verificado contra el consumidor real: `CTK.Marlon.Api`

El consumidor es `Naviera.WebApi` (proyecto `CTK.Marlon.Api`, .NET Core). Su
`IntegracionSAPRepository` y `Services/SAP/TransmisionSapRo.cs` llaman a este
integrador por HTTP. Se comprobó ruta por ruta que el renombrado no rompió nada:

| Ruta que invoca el consumidor | Existe en el integrador |
|---|---|
| `api/CTK/ConsultarCentroDeCostoPorCodigo/{codigo}/{CompanyDB}` | sí |
| `api/CTK/GuardarCentroDeCosto/{CompanyDB}` | sí |
| `api/CTK/ConsultarItemSap` | sí |
| `api/CTK/GuardarOrdenVentaCTK` | sí |
| `api/CTK/GuardarOrdenVentaCTKMarlon` | sí |
| `api/Contabilidad/GuardarAsientoContable` | sí |
| `api/NotaCredito/POST` | sí |
| `api/OrdenesCompra/POST` | sí |
| `api/OrdenesVenta/PostGuardarDocumentoAsociado` | sí |

Los 5 prefijos que usa (`api/CTK`, `api/Contabilidad`, `api/NotaCredito`,
`api/OrdenesCompra`, `api/OrdenesVenta`) siguen intactos.

**Esto justifica a posteriori haber conservado `api/CTK`**: el consumidor lo usa
en 6 sitios, así que renombrarlo lo habría roto. Los nombres de método en C# sí
cambiaron; los strings de ruta no.

#### Cómo se comporta hoy el consumidor

- El CompanyDB sale de **su propia** configuración, clave `datosSap:CompanyDB`,
  y lo asigna a cada DTO antes de enviarlo
  (`oc.CompanyDB = _configuration.GetValue<string>("datosSap:CompanyDB")`).
- La URL base del integrador está en `datosSap:UrlApiSSL`
  (QA `:9097`, PROD `:9088`, local `:63180`).
- **No envía credenciales**: ni cabeceras ni campos de usuario/clave. Por eso el
  respaldo por configuración se conservó — sin él, este consumidor dejaría de
  funcionar el día del despliegue.

#### AVISO DE COMPATIBILIDAD

`datosSap:CompanyDB` vale hoy **`CTK_QA`**, que **no** está en la lista blanca
`SAP_MAXX,SAP_IKO,SAP_AUT,SAP_STO`. Si se despliega tal cual contra el consumidor
actual, **todas sus peticiones se rechazan**.

Mientras convivan, agregue ese valor a la lista (o déjela vacía):

```xml
<add key="Sap.CompanyDbPermitidas" value="SAP_MAXX,SAP_IKO,SAP_AUT,SAP_STO,CTK_QA" />
```

Y revise el `appsettings` de producción del consumidor, cuyo CompanyDB puede
diferir del de QA.

Nota aparte: en `IntegracionSAPRepository.GuardarAsientoContableAsync` la URL se
lee de la clave `datosSap:CompanyDB`, no de `datosSap:UrlApiSSL`. El nombre de la
clave engaña y conviene corregirlo en ese proyecto.

### PASO DE DESPLIEGUE OBLIGATORIO

Las credenciales ya no están en el binario. **Antes del primer arranque** hay que
crear el archivo de configuración, o todo login a SAP fallará:

```
copy IntegradorSAP.WebApi\Config\sapCompanies.example.config ^
     IntegradorSAP.WebApi\Config\sapCompanies.config
```

Ya **no hay que poner credenciales**: llegan por cabecera. Lo único obligatorio es
declarar la lista blanca de empresas habilitadas:

```xml
<add key="Sap.CompanyDbPermitidas" value="SAP_MAXX,SAP_IKO,SAP_AUT,SAP_STO" />
```

Las entradas `Sap.Login.*` quedan comentadas en la plantilla: son respaldo
opcional para pruebas locales.

`Config/sapCompanies.config` está en `.gitignore`. Solo se versiona la plantilla
`.example`.

Si falta la entrada, `SapCompanyCredentials` lanza `ConfigurationErrorsException`
diciendo exactamente qué clave agregar, en vez de intentar el login con
credenciales vacías.

El mapeo heredado del código anterior quedó extraído **fuera del repositorio**,
en `..\integrador\sapCompanies.heredado.config`, por si alguna de esas bases
sigue en uso. Ese archivo documenta además una **inconsistencia**:
`GetCompanies()` declaraba credenciales distintas de `LoginSap` para tres bases.

### El CompanyDB llega por parámetro, y se valida

Ningún endpoint tiene la base fija: el CompanyDB viaja en cada petición.

- **38 rutas** lo reciben en la URL (`{CompanyDB}` / `{database}`), en 10 controllers.
- En los POST viene en el cuerpo: unos 24 modelos de request exponen
  `public string CompanyDB`.

De ahí aterriza en dos sitios, y ese es el motivo de validarlo:

1. el `Current Schema=` de la cadena de conexión a HANA (`SrvHana.conectar`);
2. **concatenado al SQL como nombre de esquema**, p. ej.
   `"SELECT T0.* FROM \"" + CompanyDB + "\".\"OCRD\""`.

Un nombre de esquema **no se puede pasar como `HanaParameter`** — SQL no admite
identificadores parametrizados — así que la defensa correcta no es parametrizar,
es validar. `SapCompanyCredentials.Validar(companyDB)` hace tres cosas:

1. rechaza vacío;
2. rechaza cualquier carácter que no sea letra, dígito o `_`, lo que corta la
   inyección por el nombre de esquema;
3. exige que el CompanyDB esté en `Sap.CompanyDbPermitidas`: **lista blanca**.

La lista blanca es su **propia clave de configuración**, no se deduce de dónde
haya credenciales. Es deliberado: como las credenciales ahora llegan por cabecera,
puede no existir ninguna entrada `Sap.Login.*`, y aun así el conjunto de empresas
habilitadas debe seguir siendo cerrado. Son dos decisiones distintas.

Si `Sap.CompanyDbPermitidas` está vacía o ausente, no hay restricción por nombre;
la validación de caracteres se aplica siempre.

Se invoca en los dos únicos puntos por los que pasa todo:

| Punto | Cubre |
|---|---|
| `DAO.conectarHana(DataBaseName)` | toda consulta a HANA |
| `LoginSap(string CompanyDB)` | todo login al Service Layer |

Agregar una base nueva es una línea en `sapCompanies.config`; no se toca código.

---

## 2. Lo que NO cambió, y por qué

Renombrar cualquiera de estos elementos rompe el proceso en producción.

### Campos de usuario (UDF) de SAP Business One
`U_CTK_Observacion` (182 usos), `U_CTK_Generado`, `U_CTK_Lote`,
`U_CTK_DocEntryRel`, `U_CTK_DocNumRel`, `U_CTK_FechaHoraGeneracion`,
`U_CTK_REF_LOG_INT`, `U_CTK_ENVIADO_BCO*`, `U_CTK_REF_BCO*`,
`U_CTK_BANCO_DEST_NUM`, `U_CTK_CTA_BAN_ORIGEN`, `U_CtkFechaHoraGeneracion`.

Son nombres de columna reales en las tablas de SAP y viajan como claves JSON
hacia el Service Layer. Cambiarlos en C# haría que SAP dejara de reconocer el
campo.

### Vistas y procedimientos en HANA
`CTK_GET_OV_BL_VIEW`, `CTK_GET_PEDIDOS_FACTURADOS_VIEW`,
`CTK_GET_PEDIDOS_PENDIENTES_FACTURAR_VIEW`,
`CTK_GET_PEDIDOS_PENDIENTES_GENERANOTADEBITO_VIEW`,
`CTK_STOCKMATERIALESVIEW`, `CTKItemsServiciosSeaboardView`,
`CTK_ARCHIVO_BANCO_TRANSF`, `SP_INSERT_PAGO_CTKFact`.

### Nombres de CompanyDB / esquema HANA
Ya **no están en el código** (ver sección 1.b): viven en
`Config/sapCompanies.config`, fuera del control de versiones. El CompanyDB llega
en cada petición y se usa tal cual, sin traducción.

### Propiedades que viajan a SAP sin prefijo `U_`
`CTK_DocEntryRel` y `CTK_DocNumRel` en `OrdenesVentaXLoteProcesadoViewModel.cs`
y `OrdenesVentaXLoteProcesadoNotaVentaViewModel.cs`.

Ese modelo se serializa completo (`JsonConvert.SerializeObject`) y se envía por
`PATCH Orders({DocEntry})` en `FacturacionLoteManager.ActualizaEstadoOrdenes`,
así que el nombre de la propiedad **es** el nombre del campo en el payload.
Ver también la nota en la sección 4.

### Rutas HTTP
Se conservan solo las que aún llevan `CTK`, porque `CTK` sigue siendo el prefijo
de los UDF y vistas de SAP y ahí no aporta ambigüedad de marca:

- `api/CTK` y sus acciones `GuardarOrdenVentaCTK`, `GuardarOrdenVentaCTKMarlon`

Las que nombraban una empresa **sí se renombraron** (ver 1.b):
`api/OrdenesVentaCititug` → `api/OrdenesVentaBasica`,
`PostOVGuardarCititug` → `PostOVGuardarBasica`,
`PostOVCitikoldCostosLocales` → `PostOVCostosLocales`.

En las rutas conservadas, el **nombre del método** en C# sí cambió; el string de
la ruta no. Es decir:

```csharp
[Route("GuardarOrdenVentaCTK")]                       // ruta intacta
public RespuestaGenerica GuardarOrdenVentaComercial(  // método renombrado
    [FromBody] OrdenVentaGuardarRequest orden) { ... }
```

Cuando quieran exponer rutas sin marca, agreguen un segundo `[Route(...)]` al
mismo método en vez de reemplazar el primero: ASP.NET Web API admite varios
atributos `Route` por acción, así la ruta vieja y la nueva conviven.

### Nada más
El catálogo de SQL Server y los mensajes que nombraban al cliente también se
quitaron en la segunda pasada (ver 1.b).

---

## 3. Cambios de build (intencionales, revisables)

El `HintPath` del cliente HANA era relativo con profundidad fija y **estaba
inconsistente entre los dos proyectos**:

- `Data.csproj`: `..\` × 8 → resolvía a `C:\Program Files\SAP\...` (correcto)
- `WebApi.csproj`: `..\` × 7 → resolvía a `C:\Users\Program Files\SAP\...`
  (**no existe**; la referencia solo se satisfacía de rebote vía el
  `ProjectReference`)

Además declaraban versiones distintas del mismo ensamblado
(`2.17.22.0` vs `1.0.120.0`).

Ahora ambos usan una propiedad, y se unificó la versión declarada en
`2.17.22.0` (inocuo: los dos tienen `SpecificVersion=False`):

```xml
<HanaClientPath Condition=" '$(HanaClientPath)' == '' ">$(ProgramW6432)\SAP\hdbclient\ado.net\v4.5</HanaClientPath>
<HanaClientPath Condition=" !Exists('$(HanaClientPath)') ">$(ProgramFiles)\SAP\hdbclient\ado.net\v4.5</HanaClientPath>
...
<HintPath>$(HanaClientPath)\Sap.Data.Hana.v4.5.dll</HintPath>
```

Se puede sobreescribir sin editar el `.csproj`:

```
msbuild /p:HanaClientPath="D:\SAP\hdbclient\ado.net\v4.5"
```

---

## 4. Pendientes que la migración deja anotados

1. **`CTK_DocEntryRel` / `CTK_DocNumRel` parecen un defecto preexistente.**
   El resto del código consulta el mismo dato como `U_CTK_DocEntryRel`
   (por ejemplo `$filter=U_CTK_DocEntryRel eq '...'` en
   `FacturacionLoteManager`), pero estas propiedades no llevan el prefijo `U_`.
   Si SAP espera `U_CTK_DocEntryRel`, el `PATCH` nunca está guardando ese
   campo y el error pasa inadvertido. **Verificar contra SAP antes de tocar**:
   se dejaron tal cual justamente para no cambiar el comportamiento actual.

2. **Credenciales distintas para la misma base** entre `LoginSap` (en uso) y
   `GetCompanies()` (muerto), en `PRO_*` de tres empresas. Detalle en
   `..\integrador\sapCompanies.heredado.config`. Confirmar las vigentes.

3. `IntegradorSAP.WebApi.csproj` referencia
   `Properties\PublishProfiles\FolderProfile.pubxml`, que no existe.
   Ya faltaba en el original; es inocuo (`None Include`).

4. `Helper/SrvHana - Copia.cs` sigue en el proyecto y es quien define
   `LogGeneral`, la clase de log que usa `SrvHana`. El nombre "- Copia"
   sugiere un descarte, pero **está en uso**: no borrarlo sin mover
   `LogGeneral` a su propio archivo.

5. Quedan pendientes los controles de manejo de errores del análisis previo
   (filtro global de excepciones para Web API, `${exception}` en el layout de
   NLog, `ErrException` fuera del JSON de respuesta, parámetros en las consultas
   a HANA, sesión de SAP revalidada, idempotencia en los POST).
   De ese listado esta migración ya resolvió: contraseña fuera del log,
   credenciales fuera del binario y conexión que ignoraba el CompanyDB.

---

## 5. Cómo se verificó

| Verificación | Resultado |
|---|---|
| `Citikold` / `Apptelink` / `Cititug` / `CITIKOLD` / `CITITUG` en código, config y `.sln` | **0** |
| Contraseñas en el código (`Innov22`, `admsys11`, `Citi2019`, …) | **0** |
| Nombres de CompanyDB en el código | **0** |
| Conteo de cada contrato SAP/HANA, origen vs clon | idéntico en los 15 tokens |
| Marcadores de enmascarado sin restaurar | 0 |
| namespaces con marca | 0 |
| Paridad de líneas por archivo `.cs` (primera pasada, 103 archivos) | 0 discrepancias |
| Balance de llaves en los 12 archivos editados a mano | igual al original (4 con `-1` preexistente por `.Replace("}","")`) |
| Rutas de `<Compile>/<Content>` que existen en disco | 166 de 167 (falta el `.pubxml` ya ausente en origen) |
| `ProjectReference` resuelve | OK |
| Tipos declarados | 237 = 236 del original + 2 nuevos − 1 eliminado |
| Tipos duplicados introducidos | 0 (los 4 duplicados ya existían en el original) |
| XML bien formado en `.csproj` y `.config` | OK |

**Compilación no verificada en la máquina donde se hizo la migración**: el
cliente SAP HANA (`Sap.Data.Hana.v4.5.dll`) no está instalado ahí, y los
paquetes NuGet no estaban restaurados. Antes de dar por bueno el clon:

```
nuget restore IntegradorSAP.sln
msbuild IntegradorSAP.sln /t:Rebuild /p:Configuration=Release
```

La paridad de líneas y el conteo de tokens dicen que el clon es el original
módulo sustituciones léxicas, y se comprobó que ningún nombre destino colisionaba
con un tipo o método existente — pero eso no sustituye a un compilador.
