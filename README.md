
# DeadPacker

A tool to automatically compile and pack files for Deadlock modding.

![Screenshot](https://github.com/Artemon121/DeadPacker/blob/main/Assets/Screenshot_1.png?raw=true)

## 🛠️ Features

- **Compile**: Compiles the files using the resource compiler.
- **Pack**: Packs the compiled files into a VPK file.
- **Launch**: Launches Deadlock with the specified parameters.

## ⚙️ Configuration

This tool uses a [TOML](https://toml.io) configuration file to define the steps it will take. Drag and drop the config file into the DeadPacker executable to run it. The configuration file is a text file with a `.toml` extension. You can create it using any text editor.

### Example Config

```toml
# Example configuration

# Each [[step]] will be executed sequentially.
# You can add as many steps as you want and remove the ones you don't need.
# The steps are executed in the order they are defined in the file.

[[step]]
[step.close_deadlock]

[[step]]
[step.compile]
resource_compiler_path = 'L:\rCSDK10\game\bin\win64\resourcecompiler.exe' # Use single quotes for Windows paths
addon_content_directory = 'L:\rCSDK10\content\citadel_addons\better_hero_testing'

[[step]]
[step.pack]
input_directory = 'L:\rCSDK10\game\citadel_addons\better_hero_testing'
output_path = 'L:\SteamLibrary\steamapps\common\Deadlock\game\citadel\addons\better_testing_tools.vpk'
exclude = ["cache_*.soc", "tools_thumbnail_cache.bin"] # Optional. Files that match these patterns will not be included in the VPK.

[[step]]
[step.copy]
from = 'L:\SteamLibrary\steamapps\common\Deadlock\game\citadel\addons\better_testing_tools.vpk'
to = 'L:\SteamLibrary\steamapps\common\Deadlock\game\citadel\addons\pak05_dir.vpk'

[[step]]
[step.launch_deadlock]
launch_params = "-dev -convars_visible_by_default -noassert -multiple -multirun -allowmultiple +exec autoexec +map new_player_basics" # Optional
```
