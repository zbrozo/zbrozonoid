# zbrozonoid
a free arkanoid clone

I started using Visual Studio 2017 on Windows 10 
but currently this app works on Mint Linux (18.3) with Mono 5.16 and SFML 2.2. 
I use Mono Develop IDE Version 7.7.

I've compiled SFML myself using follwing description:
https://www.sfml-dev.org/tutorials/2.2/compile-with-cmake.php

Except for SFML (C++) libraries there is need to compile CSFML wrapper:
https://www.sfml-dev.org/download/csfml/

To compile CSFML I've used cmake-gui application and set additional CMAKE_MODULE_PATH variable with
/usr/local/share/SFML/cmake/Modules
to let CSFML find FindSFML.cmake script.

To run SFML.NET on Linux there is need to create config files called dllmaps:
https://en.sfml-dev.org/forums/index.php?topic=14140.0

![](zbrozonoid.png)







