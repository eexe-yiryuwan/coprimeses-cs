# Co-prime Numbers for Uid Generation (C# Study)

## Talking

### Studied Problem

The goal is to program a random UID library, based on Wrapping Addition, sourcing Coprime Numbers. The algorithm must have finite-time worst-case-scenario.

By "random" i mean "random looking". Also i want the algorithm to be Exhaustive, meaning i want every possible uid value to be generated at some point. - this is contrary to time-based solutions, that loose entire sets of valid elements within milliseconds of insufficient usage periods.

By "finite-time worst-case-scenario" i mean that the algorithm can't get stuck, having the random number generator pick such numbers, that the program never finishes. So no direct random number usage in program logic.

### From Computer Integer Limitations to Random-Looking Numbers

In computers, values have constant capacity, as the circuits handling operations are fixed size. Then there exists a problem with addition: what to do if the size of the result would exceed the destination capacity? Most commonly, the result gets trunkated, loosing the most significant bits of information:

```
0012 + 0034 = 0046
0120 + 1010 = 1130
3000 + 9000 = ?
3000 + 9000 = (0001)2000 = 2000
```

Giving up on most significant information resulted in these positive consequences:

- The last digit is always the same magnitude = the significance does not shift
- The cutoff acquires congruence with mathematical modulo operation.
- We save all the detail.

The most important info was the modulo congruence:

```
a plus b :=   (a + b) % (max + 1)
```
This inspired me to find out interesting property, when the symmetric addition became incrementation:


```
a plus-equals b :=   a = (a + b) % (max + 1)

a increment i :=   a = (a + i) % (max + 1)
```

The interesting property is that if the increment and modulus are coprime with each other, meaning the greatest common divisor of them is one, then by applying incrementing repeatedly, we get:

- each number in the range up to the modulus will be visited at some point
- for some increments, the sequence will look random

Considering this, i picked this mechanic as the basis for my algorithm. But i needed to find the coprime pair.

### To find a random coprime pair

Having given up on finding random coprime to the computer number limit, i found an algorithm to find any pair, and how to make it random. Each coprime pair can be calculated by a recursive algorithm:

```
coprime pair = cp(i) = cp(m,n): m > n

cp(0)   =  cp(2,1)      or  cp(3,1)
cp(i+1) =  cp(2m+n, m)  or  cp(2m-n, m)  or  cp(m+2n, n)

All(i>0):  cp(i+1) > cp(i)
```

So all the pairs that exist, come by starting at `cp(2,1)` or `cp(3,1)` and applying a series of transformations. So to find a pair such that either `m` or `n` lays within a range, and such that it is a random one from all possibilities, i employed a Depth-First Tree Traversal of possible transformations, with the trasformation stack.

The algorithm i used, tries random forward transformations while the value is too low and until the value is too high. When the value gets too high, it reverts some operations and tries next in queues.

Algorithm works by assesing current value of Accumulator, if it is too small, then it creates a Shuffled Queue of transforms to do, and when the value gets too high, it reverts operations and progresses the queue.

```
Transform_History = Stack< Queue<Transform> >
Accumulator = Coprime<ITransformable<Transform> + IRevertable<Transform>>
```

## Doing

### Code Structure

- `namespace Main` uses other submodules to create a launchable application.
	- `Static Class App` is an entry point to the entire program, it calls submodules based on options
	- `namespace Args`
		- parses cli options
		- explains errors returned during arg parsing
- `namespace Ids` contain id type and a id generator
	- `class Id` is a concrete id instance
	- `class IdGenerator` generates `class Id` instances. It follows the Generator Pattern
- `namespace Coprimeses` contain coprime datastructure and facilities
	- `class Coprimes` is a concrete coprime pair, supporting transforms
		- `method Do` does a transform
		- `method Revert` undoes a transform, aka. does an Inverse Transform
	- `namespace CoprimeFinder` contains the algorithm to find coprime pairs
	- `namespace InitialVariants` contains the `cp(0) = (2,1) or (3,1)` settings facilities
	- `namespace Ops` contains the
	
		`cp(i+1) = cp(2m+n, m)  or  cp(2m-n, m)  or  cp(m+2n, n)`

		settings facilities.

### Employed Programming Patterns

| Pattern | How is used |
|---------|-------------|
| State&nbsp;Machine | Is used to interpret cli args sequence in a correct way. By using a flat Foreach-Loop i need a way to differentiate the expected identity of incomming string. |
| Null&nbsp;!=&nbsp;Error<br/>Null&nbsp;==&nbsp;Empty | Null means **only** that a value does not exist, and not that an error has occured. Instead there are additional error fields, where their null signifies the lack of errors.<br/><br/> The `Coprimeses.CoprimeFinder.FindBetween()` uses this pattern as a return value to signify that a coprime pair in provided range, does not exist. |
| Explainer&nbsp;Pattern | Not only errors can get a human-readable explaination, constructed actions and commands can be explained too. And as the explaination is done by an outside facility, it can support separate language translation framework, and self-contained messages that make sense with just the info in the object.<br/><br/> The errors and `Op` can be explained, returning `System.String` with the explaination. |
| Categorizer&nbsp;Pattern | Maintaining grouping and clonable sets of values as a separate thing. For self-contained functionality of classification. <br/><br/> I grouped the `Op` for quick cloning for Queues and Shuffling. |
| Group&nbsp;Things&nbsp;with<br/>their&nbsp;Auxiliaries | When a specific object or function has auxiliary things around it, then they should be grouped with the function or object to signify that they are only applicable there. The namespace gains the same name as the main thing in the group. <br/><br/> `Coprimes` have aux things `Op` and `InitialVariant` so are grouped together. |
| Pipeline&nbsp;Pattern | Instead of making the `IdGenerator` use `Coprimes..FindBetween`, to retrieve it a Coprime Pair, it only requires an already generated `Coprime`. - Code gets Decoupled and Modular. The Main function takes both `FindBetween` and `IdGenerator` modules **separate**, and passes required info between them. <br/><br/> Not to be mistaken for `Composition Pattern` or `Monadic Pattern` |
| Action&nbsp;Object:<br/>Do&nbsp;&amp;&nbsp;Revert | The math transforms are represented by enum values, they get applied with a `Do` function, and reverted with `Revert` function. <br/><br/> Could have the actions grouped for revertibility. |
| Readonly&nbsp;Types<br/>for&nbsp;static&nbsp;Lookups | For recognizing option introducers (for example `-m` specifies lower bound for the `m` coefficient in the coprime pair), a readonly Lookup Table is used. |

## Honest Comment

I wish we could program it just like the jpeg shaders... just caring to push the programs forward without having to write essays on how this entire thing works, because it all makes sense from well-named functions and commented algorithms...

The xml documentation syntax is uncomfortable to use.

Maybe we can program this way. Anyways may this project remain this way.
