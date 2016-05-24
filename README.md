# VirtualMachineSimulator
My rendition of the KVM screensaver in C#/OpenTK.

I wrote this based on the C++/DirectX code from this article on Code Project:
http://www.codeproject.com/Articles/5863/Windows-Virtual-Machine-screen-saver, which was written based on the Linux KVM Virtual Machine screen saver.

You can configure the simulator using the settings in the application configuration file.  Otherwise, look in the Simulation namespace (https://github.com/treytomes/VirtualMachineSimulator/tree/master/VirtualMachineScreenSaver/VirtualMachineScreenSaver/Simulation) to see how the virtual machine actually works.

The Rendering namespace handles the graphics.  I pulled this code from my ASCIIWorld2 project, also on GitHub.

The Extended ASCII font images are included for a later date when I allow switching out the default tileset for an ASCII tileset.  Please send me an e-mail if you manage to improve this code in any way!

~Trey Tomes (trey.tomes@gmail.com)
