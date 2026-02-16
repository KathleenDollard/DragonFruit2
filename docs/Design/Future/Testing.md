# Testing

DragonFruit2 is fully tested, see [the repo]() for more information.

## Testing application behavior

After retrieving a hydrated args object from DragonFruit2, you'll run the code that performs the actions of the application. For non-trivial apps, consider action methods that take the args object as a parameter. For CLIs with subcommands, this might be the Execute method of the MyArgs class. This approach lets you create a MyArgs object and pass it to your action for testing your code in isolation.

## Testing validation

## Testing default values