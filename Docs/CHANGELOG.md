# SharpShell Easy Log Change Log 📋

## v1.1 *(current)* 🆕

#### 🌟 Improvements:
 - Added light and dark visual themes. The light theme uses the default Windows system colors.
 - Added a toggle switch at the right side of the menu bar, and the "View > Dark Theme" menu item (Ctrl+D), to switch between the light and the dark theme. The selected theme is saved in the user settings.
 - The title bar follows the dark theme (DwmSetWindowAttribute / DWMWA_USE_IMMERSIVE_DARK_MODE), also when the theme is switched at runtime.
 - The scroll bars of the log output follow the dark theme.
 - The SharpShell logging settings are now written to both the 64-bit and the 32-bit registry views, so they also take effect for 32-bit processes that host SharpShell servers.
 - The current content of the log file is shown as soon as the log file is selected, instead of waiting for the next write.
 - Selecting a different log file clears the output of the previous log file.
 - Disabling the logging only removes the SharpShell logging values ("LoggingMode" and "LogPath"); the "HKLM\SOFTWARE\SharpShell" key is deleted only if it becomes empty.
 - "Explorer > Restart" no longer shows a console window while closing Explorer.
 - Added a unit tests project (MSTest).

#### 🛠️ Fixes:
 - The application crashed with an unhandled UnauthorizedAccessException when it was not run as Administrator, because writing to "HKLM\SOFTWARE\SharpShell" requires elevation. The application now requests administrative privileges (application manifest), and registry access errors are shown in a message box.
 - The application crashed when the saved log file directory no longer existed (e.g. a cleaned temp folder), because FileSystemWatcher cannot monitor a non-existent directory.
 - The application could crash when the log file was locked by the writer process while reading it (unhandled IOException in a thread-pool thread).
 - The read position of the log file was not reset when another log file was selected, or when the log file was truncated or re-created, so new log entries were not shown.
 - Partial lines and multi-byte UTF-8 characters could be split between two reads of the log file; the UTF-8 BOM could be shown as a garbage character.
 - The registry was updated twice every time the logging mode changed.
 - An invalid logging mode stored in the user settings could be written to the registry.
 - Copying the log text could crash when the clipboard was in use by another process.
 - An error writing DebugView.exe to the temp folder crashed the application.
 - Error message boxes showed "FormMain" as their caption instead of the application title.
 - The FileSystemWatcher was not disposed when the application was closed.
 - The "Main form" of the application settings pointed to a non-existent "Form1" form.

## v1.0 🔙
Initial Release.
