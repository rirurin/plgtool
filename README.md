# PLG Tool

For converting JSON exports of PLG files from Persona 3 Reload to the uasset format.

Usage: `Usage: plgtool.exe [import json] [cooked package] [io store package]`

- Import JSON: JSON file that was exported from Blender
- Cooked Package: .uasset file generated from [ZenTools-UE4](https://github.com/WistfulHopes/ZenTools-UE4)
- Patch Package: .uasset from Persona 3 Reload (Using export raw data in FModel)
- Output (optional): The output file name (defaults to using \"[filename]_MODIFIED.uasset\" in patch package folder)