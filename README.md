## Overview
Fork of [https://github.com/DigitalRune/DigitalRune](https://github.com/DigitalRune/DigitalRune) for the modern day Monogame/FNA.

It has major differences with the original:
1. The fork doesnt use Content Pipeline. It loads assets in raw form using [XNAssets](https://github.com/rds1983/XNAssets). Particularly 3d models are loaded from gltf/glb(version 2).
2. The fork uses [FontStashSharp](https://github.com/FontStashSharp/FontStashSharp) for the text rendering.

Right now, the fork is in proof-of-concept stage. Absolute majority of the samples work.

## Building for MonoGame
This instruction works only for Windows.
1. Clone this repo.
2. Make sure [mgfxc](https://docs.monogame.net/articles/tools/mgfxc.html) is installed.
3. Install [efscriptgen](https://github.com/rds1983/efscriptgen) using following command: `dotnet tool install --global efscriptgen`

   *That is utility for generating batch scripts for the effects' compilation.*  
5. Go to folder `DigitalRise\Source\DigitalRise.Graphics\EffectsSource` and execute command `efscriptgen .`
6. A new subfolder `MonoGameDX11` would appear. Go there and execute `compile_all.bat`
7. Repeat steps 4 and 5 for the folder `DigitalRise\SampleBrowser\Assets`
8. Open `DigitalRise.MonoGame.DirectX.sln` from the IDE and run the project `SampleBrowser.MonoGame.DirectX`. Use PgUp/PgDown to switch between samples.

## Building for FNA
This instruction works only for Windows.
1. Clone following repos:
   Url|Description|Additional
   ---|-----------|----------
   https://github.com/FNA-XNA/FNA|FNA|Clone submodules too(`git submodule update --init --recursive`)
   https://github.com/rds1983/DdsKtxXna|Library for loading DDS textures|Clone submodules too(`git submodule update --init --recursive`)
   https://github.com/FontStashSharp/FontStashSharp|Text rendering library|
   https://github.com/rds1983/XNAssets|Library for loading assets|
   https://github.com/rds1983/DigitalRise|This repo|

   All above repos should reside in the same folder. So the folder structure should look like this:
   ![image](https://github.com/rds1983/DigitalRise/assets/1057289/dc0cf4fb-654f-4e14-9e51-a22edf52b9e0)
2. Make sure the Direct3D compiler tool(fxc.exe) is within your PATH environment variable.
3. Install [efscriptgen](https://github.com/rds1983/efscriptgen) using following command: `dotnet tool install --global efscriptgen`

   *That is utility for generating batch scripts for the effects' compilation.*
4. Go to folder `DigitalRise\Source\DigitalRise.Graphics\EffectsSource` and execute command `efscriptgen .`
5. A new subfolder `FNA` would appear. Go there and execute `compile_all.bat`
6. Repeat steps 4 and 5 for the folder `DigitalRise\SampleBrowser\Assets`
7. Open `DigitalRise.FNA.Core.sln` from the IDE and run the project `SampleBrowser.FNA.Core`. Use PgUp/PgDown to switch between samples.

## License
The DigitalRise Engine is licensed under the terms and conditions of the 3-clause BSD License.
Portions of the code are based on third-party projects which are licensed under their respective
licenses. See [LICENSE.TXT](LICENSE.TXT) for more details.
