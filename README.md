Softrope
========

[![Join the chat at https://gitter.im/IainMNorman/Softrope](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/IainMNorman/Softrope?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

**Notes**

Sorry but this won't compile straight away, you probably need to install Bass.NET into your GAC. Have a look for it on http://www.un4seen.com/.

The setup project relies on Advanced Installer (AI) from Caphyon. I'd suggest not building it at all if you have not got AI.

To stop it attempting to create the installer you need to edit the project file for the UI and take out the build tasks that call AI.

The installer is only built if you're building the Release build. Debug will be fine.
