# Demlock: Demo Parser for Deadlock

## NOTE: THIS IS HEAVILY WIP RIGHT NOW

This is a demo parser the is being created in C# for the express purposes of taking dead lock demo files (.dem) and parsing them to extract useful stats and information.

Right now this project is at the very start, but is quickly being brought up with reference to parsers for other valve games.

## First release build
This commit marks the first build I can say is stable enough to be a release.

It is still lacking a lot of features, however it is able to fully parse the sample demo (more integration testing to come) and seems stable enough.

The interface to consume the results is not defined fully yet, but that is being worked on.

This project comes with absolutely zero assurance of stability, and is to be used at your own risk.

There has also been near zero optimization done to the running speed of the application, so please expect that it will be very slow, as there is still a lot of stuff left in that was only useful for debugging that will be removed as benchmarks are started.

If you identify anything you think you might be able to fix or upgrade please feel free to reach out as any help would be appreciated! its been a long journey to get here.



### Resources
This project is being built and maintained by the Devlock open source community, if you have questions or would like to join in on learning about demo files, you can find our discord below

https://discord.gg/m9wNg4Ak47


List of parsers that I have been basing parts of my implementation on 
- https://github.com/LaihoE/demoparser/
- https://github.com/saul/demofile-net
- https://github.com/dotabuff