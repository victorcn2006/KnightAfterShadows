# Tile Prefab Tool - Technical Architecture



## 1. Technical Architecture & Technologies

The tool is built using several advanced Unity Editor features to provide a seamless "brush-like" experience directly in the Scene View.

### Core Technologies
I used these technologies to create the UI and behaviour of the tool:
*   **`EditorWindow`**: Used to create the **Tile Palette** window (`TilePaletteWindow.cs`). This acts as the main interface where you organize and select tiles.
*   **`ScriptableObject`**: 
    *   `TilePaletteData`: Stores the library of tiles and groups as an asset file (`DefaultPalette.asset`).
    *   `TilePlacementSettings`: Saves your global configuration (grid size, layers, etc.) so they persist between sessions.
*   **`SceneView.duringSceneGui`**: A powerful callback that allows the script to draw graphics (like the blue grid cursor) and intercept mouse clicks directly in the 3D/2D Scene View.
*   **`Handles`**: Used to draw the wireframe cube cursor that snaps to the grid.
*   **`PrefabUtility`**: Ensures that when you "paint," the tool creates proper **Prefab Instances** rather than just cloning objects, maintaining the link to your original assets.
*   **`Undo` System**: Integrated so that every click or erase action can be reversed using `Ctrl+Z`.

### Key Components
1.  **TilePrefabEntry**: A simple data class that holds the prefab, a name, a thumbnail, and placement settings (offset, rotation).
2.  **TileGroup**: Allows organizing tiles into categories (e.g., "Terrain", "Structures").
3.  **Raycasting**: Uses Unity's Physics engine to translate your 2D mouse position on the screen into a 3D/2D coordinate in the game world.

---

## 2. Setting Up the Tool

Before starting to use it we need to create a prefab with some requirements to allow the tool to detect the tiles to add them to the grid or remove them by a collider:

### Create the "PlacedTile" Tag
The tool identifies what it is allowed to erase by looking for a specific tag.
1.  Go to **Edit > Project Settings > Tags and Layers**.
2.  Click the **+** under **Tags**.
3.  Add a tag named exactly: `PlacedTile`.

### Grid Settings
1.  Find the settings asset at: `Assets/Editor/TilePlacement/Settings.asset`.
2.  In the Inspector, you can change the **Grid Size** (e.g., 1 for 1x1 meter tiles).
3.  Set a **Parent Container** (an empty object in your hierarchy) if you want all painted tiles to be neatly organized under one object.

---

## 3. How to Use

### Step 1: Create a Prefab
1.  In your scene, build your object (e.g., a wall or a small house).
2.  Create an **Empty GameObject** as a parent.
3.  Drag your tiles/sprites inside that parent.
4.  **Important:** Add a **Collider** (Box Collider or Box Collider 2D) to the parent or children. The tool needs this to "see" the object for erasing.
5.  Drag the parent object into your Project folder to save it as a **Prefab**.

### Step 2: Add to Palette
1.  Open the window: **Tools > Tile Palette**.
2.  Click the **+** button on a Group header.
3.  In the popup, drag your new Prefab into the slot and give it a name.
4.  Click **Add**.

### Step 3: Paint the Level
*   **Select a Tile:** Click the thumbnail in the Palette.
*   **Place (Pencil Icon):** Left-click in the Scene View to snap and place the prefab.
*   **Erase (Eraser Icon):** Left-click on a placed tile to remove it.
*   **Pick (Eyedropper):** Click a tile in the scene to automatically select it in your Palette.
*   **Rotate:** If "Randomize Rotation" is on in Settings, the tool will spin the object automatically for variety.

---

## 4. Video Demonstration

Watch the tool in action:
[Tile Prefab Tool Demonstration](../videos/tool.mp4)

