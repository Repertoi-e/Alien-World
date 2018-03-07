Echo Building shaders
Echo Launch dir: "%~dp0"
Echo Current dir: "%CD%"
cd "%~dp0"
cd "..\Alien World\bin\Debug\Resources\Shaders"
PATH="C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86"

fxc /T vs_5_0 /E VSMain "Source\Renderer2D.hlsl" /Fo "Renderer2DVS.cso" /nologo
fxc /T ps_5_0 /E PSMain "Source\Renderer2D.hlsl" /Fo "Renderer2DPS.cso" /nologo

fxc /T vs_5_0 /E VSMain "Source\Test.hlsl" /Fo "TestVS.cso" /nologo
fxc /T ps_5_0 /E PSMain "Source\Test.hlsl" /Fo "TestPS.cso" /nologo