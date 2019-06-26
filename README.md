# zbrozonoid
a free arkanoid clone

This app works on Mint Linux and Windows with Mono 5.16 and SFML 2.5 (installed with nuget). 
I use Mono Develop IDE. But it can be also compiled with Visual Studio.

---

On Linux I've compiled SFML myself using follwing description:
https://www.sfml-dev.org/tutorials/2.5/compile-with-cmake.php

Except for SFML (C++) libraries there is need to compile CSFML wrapper:
https://www.sfml-dev.org/download/csfml/

To compile CSFML I've used cmake-gui application and set additional CMAKE_MODULE_PATH variable with
/usr/local/share/SFML/cmake/Modules
to let CSFML find FindSFML.cmake script.

![](zbrozonoid.png)







