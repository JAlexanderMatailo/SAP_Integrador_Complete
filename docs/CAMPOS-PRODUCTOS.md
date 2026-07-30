# Catálogo de campos de producto disponibles

Extraído de los modelos del proyecto. Marca los que necesites y te armo el endpoint.

> **Alcance de esta lista.** Son los campos que **este proyecto ya tiene modelados**
> del entity `Items` del Service Layer (equivale a la tabla `OITM`). **No es una
> introspección en vivo de tu HANA**: si tu instalación tiene UDF adicionales en
> `OITM` que nadie declaró aquí, no aparecen. Para el listado exacto de tu base:
>
> ```sql
> SELECT COLUMN_NAME, DATA_TYPE_NAME, LENGTH
> FROM SYS.TABLE_COLUMNS
> WHERE SCHEMA_NAME = 'SAP_IKO' AND TABLE_NAME = 'OITM'
> ORDER BY POSITION;
> ```
>
> Cambia `OITM` por `OITW` para el stock por bodega.

## Resumen

| Bloque | Origen en SAP | Campos | Estado |
|---|---|---|---|
| Maestro, estándar | `Items` / `OITM` | 214 | activos |
| Maestro, UDF de tu instalación | `Items` / `OITM` | 13 | activos |
| Maestro, comentados en el modelo | `Items` / `OITM` | 50 | **hay que descomentar** |
| Stock por bodega | `ItemWarehouseInfoCollection` / `OITW` | 62 | activo |
| Listas de precios | `ItemPrices` / `ITM1` | 10 | **clase declarada, sin usar** |
| Proveedor preferente | `ItemPreferredVendors` | 1 | **clase declarada, sin usar** |
| Vista de stock que ya se consume | `CTK_STOCKMATERIALESVIEW` | 14 | activo |

---

## 1. UDF de tu instalación (13)

Específicos de esta empresa. Casi todos son de ICE (impuesto a consumos
especiales) y facturación electrónica.

| Campo | Tipo |
|---|---|
| `U_TIPO_BIEN` | object |
| `U_ICE` | string |
| `U_COD_ICE_PRODUC` | object |
| `U_CLASIF_ICE` | object |
| `U_MARCA_ICE` | object |
| `U_PRESENTACION` | object |
| `U_CAPACIDAD` | object |
| `U_UNID_ICE` | object |
| `U_GRADO_ALC` | object |
| `U_PAIS_ICE` | object |
| `U_CXS_ISGC` | string |
| `U_EXX_FE_InfoAdic` | string |
| `U_EXX_FE_TipoInfoAdi` | object |

## 2. Identificación

`ItemCode`, `ItemName`, `ForeignName`, `BarCode`, `SupplierCatalogNo`,
`Manufacturer`, `Mainsupplier`, `Picture`, `User_Text`, `AttachmentEntry`,
`LinkedResource`

## 3. Clasificación

`ItemsGroupCode`, `CustomsGroupCode`, `ItemType`, `ItemClass`, `MaterialType`,
`MaterialGroup`, `ProductSource`, `ServiceGroup`, `NCMCode`, `SWW`,
`DataExportCode`, `ItemCountryOrg`, `ScsCode`, `SpProdType`

## 4. Qué tipo de artículo es (banderas)

`PurchaseItem`, `SalesItem`, `InventoryItem`, `AssetItem`, `VirtualAssetItem`,
`IsPhantom`, `TreeType`

## 5. Stock y cantidades

`QuantityOnStock`, `QuantityOrderedFromVendors`, `QuantityOrderedByCustomers`,
`MinInventory`, `MaxInventory`, `DesiredInventory`, `DefaultWarehouse`,
`ManageStockByWarehouse`, `InventoryUOM`, `ComponentWarehouse`

## 6. Costos

`MovingAveragePrice`, `AvgStdPrice`, `ProdStdCost`, `CostAccountingMethod`,
`GLMethod`, `InCostRollup`

## 7. Impuestos

`VatLiable`, `SalesVATGroup`, `PurchaseVATGroup`, `TaxType`, `ArTaxCode`,
`ApTaxCode`, `IndirectTax`, `WTLiable`, `Excisable`, `ChapterID`, `GSTRelevnt`,
`GSTTaxCategory`, `SACEntry`, `OutgoingServiceCode`, `IncomingServiceCode`

## 8. Unidades de venta

`SalesUnit`, `SalesItemsPerUnit`, `SalesPackagingUnit`, `SalesQtyPerPackUnit`,
`SalesUnitLength`, `SalesLengthUnit`, `SalesUnitWidth`, `SalesWidthUnit`,
`SalesUnitHeight`, `SalesHeightUnit`, `SalesUnitVolume`, `SalesVolumeUnit`,
`SalesUnitWeight`, `SalesWeightUnit`, `SalesFactor1`…`SalesFactor4`

Segunda unidad de venta: `SalesUnitLength1`, `SalesLengthUnit1`,
`SalesUnitWidth1`, `SalesWidthUnit1`, `SalesUnitHeight1`, `SalesHeightUnit1`,
`SalesUnitWeight1`, `SalesWeightUnit1`

## 9. Unidades de compra

`PurchaseUnit`, `PurchaseItemsPerUnit`, `PurchasePackagingUnit`,
`PurchaseQtyPerPackUnit`, `PurchaseUnitLength`, `PurchaseLengthUnit`,
`PurchaseUnitWidth`, `PurchaseWidthUnit`, `PurchaseUnitHeight`,
`PurchaseHeightUnit`, `PurchaseUnitVolume`, `PurchaseVolumeUnit`,
`PurchaseUnitWeight`, `PurchaseWeightUnit`, `PurchaseFactor1`…`PurchaseFactor4`,
`BaseUnitName`

Segunda unidad de compra: `PurchaseUnitLength1`, `PurchaseLengthUnit1`,
`PurchaseUnitWidth1`, `PurchaseWidthUnit1`, `PurchaseUnitHeight1`,
`PurchaseHeightUnit1`, `PurchaseUnitWeight1`, `PurchaseWeightUnit1`

## 10. Trazabilidad: series y lotes

`ManageSerialNumbers`, `ManageBatchNumbers`, `SerialNum`,
`ForceSelectionOfSerialNumber`, `ManageSerialNumbersOnReleaseOnly`,
`SRIAndBatchManageMethod`, `IssueMethod`, `EnforceAssetSerialNumbers`

## 11. Planificación y aprovisionamiento

`PlanningSystem`, `ProcurementMethod`, `LeadTime`, `OrderIntervals`,
`OrderMultiple`, `MinOrderQuantity`, `ShipType`, `WarrantyTemplate`

## 12. Vigencia y bloqueo

`Valid`, `ValidFrom`, `ValidTo`, `ValidRemarks`,
`Frozen`, `FrozenFrom`, `FrozenTo`, `FrozenRemarks`

## 13. Cuentas contables (a nivel de artículo)

`IncomeAccount`, `ExemptIncomeAccount`, `ExpanseAccount`,
`ForeignRevenuesAccount`, `ECRevenuesAccount`, `ForeignExpensesAccount`,
`ECExpensesAccount`

## 14. Comisiones

`CommissionPercent`, `CommissionSum`, `CommissionGroup`

## 15. Propiedades libres de SAP

`Properties1` … `Properties64` — las 64 banderas configurables del maestro.

---

## 16. Stock por bodega (62) — `ItemWarehouseInfoCollection` / `OITW`

Una fila **por bodega**. Es la única colección de relación que hoy está activa
en el modelo.

**Lo que normalmente interesa:**

| Campo | Tipo | Qué es |
|---|---|---|
| `WarehouseCode` | string | código de bodega |
| `InStock` | float | existencia |
| `Committed` | float | comprometido |
| `Ordered` | float | pedido a proveedor |
| `MinimalStock` | float | stock mínimo |
| `MaximalStock` | float | stock máximo |
| `MinimalOrder` | float | pedido mínimo |
| `StandardAveragePrice` | float | precio promedio |
| `Locked` | string | bloqueada |
| `DefaultBin` | object | ubicación por defecto |
| `DefaultBinEnforced` | string | ubicación obligatoria |
| `CountedQuantity` | float | cantidad contada |
| `WasCounted` | string | fue contada |
| `Counted` | float | contado |
| `UserSignature` | int | usuario |
| `ItemCycleCounts` | object[] | conteos cíclicos |

**Cuentas contables por bodega** (46 campos, rara vez se exponen a un consumidor
externo): `InventoryAccount`, `CostAccount`, `TransferAccount`,
`RevenuesAccount`, `VarienceAccount`, `DecreasingAccount`, `IncreasingAccount`,
`ReturningAccount`, `ExpensesAccount`, `EURevenuesAccount`, `EUExpensesAccount`,
`ForeignRevenueAcc`, `ForeignExpensAcc`, `ExemptIncomeAcc`,
`PriceDifferenceAcc`, `ExpenseClearingAct`, `PurchaseCreditAcc`,
`EUPurchaseCreditAcc`, `ForeignPurchaseCreditAcc`, `SalesCreditAcc`,
`SalesCreditEUAcc`, `ExemptedCredits`, `SalesCreditForeignAcc`,
`ExpenseOffsettingAccount`, `WipAccount`, `ExchangeRateDifferencesAcct`,
`GoodsClearingAcct`, `NegativeInventoryAdjustmentAccount`,
`CostInflationOffsetAccount`, `GLDecreaseAcct`, `GLIncreaseAcct`,
`PAReturnAcct`, `PurchaseAcct`, `PurchaseOffsetAcct`, `ShippedGoodsAccount`,
`StockInflationOffsetAccount`, `StockInflationAdjustAccount`,
`VATInRevenueAccount`, `WipVarianceAccount`, `CostInflationAccount`,
`WHIncomingCenvatAccount`, `WHOutgoingCenvatAccount`, `StockInTransitAccount`,
`WipOffsetProfitAndLossAccount`, `InventoryOffsetProfitAndLossAccount`,
`PurchaseBalanceAccount`

## 17. Listas de precios (10) — `ItemPrices` / `ITM1`

**La clase `Itemprice` existe pero `ItemPrices` está comentado en
`ItemsViewModel`**: hoy no se trae. Si necesitas precios, hay que descomentarlo.

| Campo | Tipo |
|---|---|
| `PriceList` | int |
| `Price` | float |
| `Currency` | object |
| `AdditionalPrice1` | float |
| `AdditionalCurrency1` | object |
| `AdditionalPrice2` | float |
| `AdditionalCurrency2` | object |
| `BasePriceList` | int |
| `Factor` | float |
| `UoMPrices` | object[] |

## 18. Proveedor preferente (1) — `ItemPreferredVendors`

También comentado. La clase solo declara `BPCode` (string), así que si lo
necesitas habría que completarla.

## 19. Vista de stock que ya se consume (14) — `CTK_STOCKMATERIALESVIEW`

Lo que hoy devuelve `GET api/Inventario/GetMaterialesSap/{CompanyDB}`.
Ya viene resuelto y aplanado, con nombres en español.

| Campo | Tipo |
|---|---|
| `Codigo` | string |
| `Producto` | string |
| `Unidad` | string |
| `BodegaCodigo` | string |
| `Bodega` | string |
| `Grupo` | string |
| `CatalogoFabricante` | string |
| `Fabricante` | string |
| `UnidadInventario` | string |
| `UnidadCompra` | string |
| `UltimoPrecioCmp` | decimal |
| `Stock` | decimal |
| `PMP` | decimal |
| `Valorizado` | decimal |

---

## 20. Los 50 campos comentados

Existen en SAP pero están desactivados en el modelo. Descomentar y listo:

`odatametadata`, `AutoCreateSerialNumbersOnRelease`, `DNFEntry`, `GTSItemSpec`,
`GTSItemTaxCategory`, `FuelID`, `BeverageTableCode`, `BeverageGroupCode`,
`BeverageTypeOfProduct`, `BeverageAdditionalInfo`, `BeverageBrandCode`,
`BeverageCommercialBrand`, `ItemPrices`, `ItemPreferredVendors`,
`ItemLocalizationInfos`, `ItemProjects`, `ItemDistributionRules`,
`ItemAttributeGroups`, `ItemDepreciationParameters`, `ItemPeriodControls`,
`ItemUnitOfMeasurementCollection`, `ItemBarCodeCollection`,
`ItemIntrastatExtension` y el resto de extensiones regionales
(Brasil, India, Rusia) que no aplican en Ecuador.

Para el listado literal, buscar `//public` en
[ItemsViewModel.cs](../IntegradorSAP.Services/Models/ItemsViewModel.cs).
