
# Smart Addressables

>Note: This feature works only when Addressables is installed.

Provides efficient logic for using Unity Addressables, with explanations on caching and releasing memory for each scene.

```csharp
// Load default Addressable resources
await SmartAddressables.InitializeAsync();

// Load only the assets labeled "titlescene"
var resource = await SmartAddressables.LoadResourcesAsync("titlescene");

// Cache AssetRef in memory
var memoryInObj = resource.GetObject<GameObject>("A/S/D");
Instantiate(memoryInObj);

// Instantiate AssetRef directly
var createObj = resource.Instantiate<GameObject>("A/S/D");

// Release
resource.Release();

```
