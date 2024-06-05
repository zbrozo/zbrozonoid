# zbrozonoid
a free arkanoid clone with two player mode (two mouses are needed)

  This app works on Mint Linux and Windows with Mono 5.16 and SFML 2.5 (installed with nuget). 
I use Mono Develop IDE. But it can be also compiled with Visual Studio.

ManyMouse library problem:

On Mint Linux 21.3 there is no libdl.so so I do: 

sudo ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/libdl.so

![](zbrozonoid.png)







