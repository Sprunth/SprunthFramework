# SprunthFramework v0.1a
I've been using a framework similar to this for almost all of my SFML-based games so I figure it's time to seperate it into its own library.
Built with the SFML2.2.net bindings.

To jumpstart:
  
    var game = new SprunthGame(new Vector2u(800,600));
    game.Initialize("Hello SprunthFramework!");
    game.Run();
  
Note: Download and add the SFML.net references to the project, and copy the extlibs to the output folder.
  
Features:
* Complete game loop
* Heirarchial structure for drawable classes
  * Displays (different views) and drawableObjects extend the same base class
  * Extension of SFML's base events, such as the custom OnMouse{Enter, Move, Exit} events
* Easy extension with abstract classes and virtual functions


Note:
+ More extensive documentation to come once a stable version is reached.
