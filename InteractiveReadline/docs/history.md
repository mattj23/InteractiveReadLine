# History

The history feature is meant to emulate a simple version of the typical history behavior of most shells, in which two keys (typically the up and down arrows) cycle through the list of previously entered commands.

Accomplishing this requires three things: a collection of previously entered commands, a mechanism for cycling through these commands during the read line operation, and a means of updating the collection when a new command is entered.

