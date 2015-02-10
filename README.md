# carob

A trivial utility to automatically elevate and run chocolatey ( https://chocolatey.org/ ).

I, for one, never have an administrative PowerShell open when I want one. It's not the sort of thing I leave open routinely, especially as I can handle most things with functions wrapped around invoke-elevated, and having to remember to open one to run certain commands in mid-flow is, well, annoying. Mucking about with runas, almost as much so.

Unfortunately, since I use chocolatey as my package manager and take full advantage of it, said special occasions crop up more often than I'd like.

Enter carob.

Once it's installed, just use carob in exactly the way you'd use choco. If you aren't currently running in an elevated context, carob elevates for you (with associated UAC prompt) and runs choco in the elevated session with the arguments you gave it, pausing afterwards so that you can read choco's output. If you _are_ , carob simply passes the arguments straight through to choco. Either way, you get what you want, no errors, no retyping, and if you're in the habit of logging your console activity to keep track of what you did, then your choco^Wcarob command line shows up in the right place.
