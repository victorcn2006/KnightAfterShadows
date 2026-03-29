# Shop & Inventory System Documentation

## Overview
The Shop and Inventory system is a database-driven implementation for Unity using **SQLite4Unity3d**. It ensures data integrity through **Atomic Transactions** and provides real-time UI synchronization between the player's wallet, the inventory display, and the shop interface.

---

## 1. Database Architecture

### Models (ORM)
- **`User`**: Account credentials (handled by `AuthRouter`).
- **`Player`**: Linked to a `User`. Stores the `diners` (money) field.
- **`ItemDefinition`**: Static data for items (Name, Price, Sprite Key).
- **`Inventari`**: The link between a `Player` and an `ItemDefinition` with a specific `quantity`.

### Table Initialization
The system automatically creates and populates these tables on the first run via `SQLiteReader.InitializeDefaultItems()`.

| ID | Name | Sprite Key | Default Price |
|----|------|------------|---------------|
| 1  | Skull  | skull      | 80            |
| 2  | Helmet | helmet     | 100           |
| 3  | Sword  | sword      | 150           |
| 4  | Silk   | silk       | 50            |
| 5  | Potion | potion     | 20            |
| 6  | Gem    | gem        | 500           |

---

## 2. Transactional Logic & Rollback

### Atomic Transactions
Buying and selling operations use `BeginTransaction()`, `Commit()`, and `Rollback()` to ensure that either all steps succeed or none do.

#### Buy Operation Flow:
1. **Begin Transaction.**
2. **Deduct Money:** Check if the player has enough `diners`. Update the `Player` table.
3. **Add to Inventory:** Check if the item exists in `Inventari`. Update `quantity` or insert a new row.
4. **Commit:** Save changes to the `.sqlite` file.
5. **Exception/Failure:** If any step fails, **Rollback** is triggered, restoring the player's money and inventory to their previous state.

### Testing the Rollback
A special method `BuyWithFailError()` in `ShopItemHandler` simulates a database crash mid-transaction. This is used in builds to prove that money is not lost and items are not added if an error occurs.

---

## 3. UI Implementation

### Components
- **`ShopInventoryController` (Singleton):** 
  - Manages the 20 inventory slots and the money text display.
  - Automatically refreshes the UI whenever a transaction is completed.
- **`ShopItemHandler`:**
  - Attached to each static shop row in the UI.
  - Configurable `ItemId`, `BuyPrice`, and `SellPrice` in the Unity Inspector.
- **`InventorySlot`:**
  - Controls the individual squares.
  - Shows the item sprite if the slot is filled; otherwise, hides the icon.
- **`SpriteLibrary` (ScriptableObject):**
  - Maps database `sprite_name` strings (e.g., "skull") to actual `Sprite` assets in the project.

### Integration Steps
1. **Assign IDs:** Ensure the `ItemId` in each `ShopItemHandler` matches the database IDs (1-6).
2. **Link Buttons:** Point the `OnClick()` events of the Buy/Sell buttons to the corresponding methods in the `ShopItemHandler`.
3. **Fill the Library:** Add the 6 item sprites to the `SpriteLibrary` asset with their respective keys.

---

## 4. Maintenance
- **To Reset Data:** Delete `Assets/MyDatabase.sqlite`. The system will recreate the database and default items on next play.
- **To Add Items:** Update the `InitializeDefaultItems` list in `SQLiteReader.cs`.
- **To Modify UI:** Update the `Slots` array in the `ShopInventoryController` inspector if you change the number of inventory squares.
