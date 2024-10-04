fxc "D:\Projects\DigitalRise\DigitalRise\Effects\BillboardEffect.fx" /Fo "D:\Projects\DigitalRise\DigitalRise\Effects\FNA\bin\BillboardEffect_TEXTURE.efb" /T:fx_2_0 /D TEXTURE=1
@if %errorlevel% neq 0 exit /b %errorlevel%
fxc "D:\Projects\DigitalRise\DigitalRise\Effects\BillboardEffect.fx" /Fo "D:\Projects\DigitalRise\DigitalRise\Effects\FNA\bin\BillboardEffect.efb" /T:fx_2_0
@if %errorlevel% neq 0 exit /b %errorlevel%
