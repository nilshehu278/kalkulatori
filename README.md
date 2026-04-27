# Apedin Market (Windows Forms / .NET Framework 4.7.2)

Apedin Market is a simple offline desktop POS system for a small grocery shop.

## Features
- Product management (add, edit, delete, search)
- Stock management with low-stock warning
- POS screen with clickable product cards
- Barcode reader flow (keyboard-wedge scanners supported)
- Cart operations (increment by click, remove, clear)
- Payment + automatic change calculation
- Transactional sale completion (rollback on failure)
- Sales history + sale item details
- SQLite database auto-creation on first launch (`apedin_market.db`)
- Sample products seeded at first run

## Tech stack
- C# + Windows Forms
- .NET Framework 4.7.2 (`net472`)
- SQLite (`System.Data.SQLite.Core`)

## Run instructions
1. Open `Calculator.sln` in Visual Studio (2019+).
2. Ensure NuGet restore is enabled.
3. Build and run the `CalculatorApp` project.
4. On first launch, database file `apedin_market.db` is created automatically in the output directory.

## Barcode reader usage
Most USB barcode scanners behave like keyboards:
1. Put cursor in **Barcode Scan** field on POS screen.
2. Scan barcode.
3. Scanner sends Enter key -> product is added automatically.

## Structure
- `CalculatorApp/Models`: entity classes (`Product`, `Sale`, `SaleItem`)
- `CalculatorApp/Data`: DB initialization and repositories
- `CalculatorApp/Forms`: UI screens
- `CalculatorApp/UI`: styling helpers (`Theme`, `RoundedButton`)
