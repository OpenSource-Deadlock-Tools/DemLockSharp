# Class Mapping Generator

This project is a dependency at build time and will look for .class.json files which contain a schema that is able to be dumped from demo files for mapping field paths to concrete types.

Once it has this mapping and it parses it, there will be spit out a class for each definition that will allow it to map all the properties automatically.

There is still a little bit of work to be done to get it fully fleshed out with all of the entities, and add some special handling to make sure it does well with arrays.

Right now it is just doing the top level properties, but will be tweaked to handle sendtable nested properties, and class references...

Once it handles all cases this will generate objects used to listen for entities, and once it is stable it will be cleaned up so that the reader can skip entities without material subscribers listening to them.

