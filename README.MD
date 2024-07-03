### Savage.Logs
---

A ***zero garbage*** structured logging library.

 It's intended to be performant, easy to use and extend, and come out of the box integrated into existing game engines that support the common language runtime.

What does zero garbage mean?

Loggy handles all string manipulations inside a region of memory it allocates and manages completely outside of the managed heap. None of the strings put through Loggy put *any* pressure on the garbage collector. This memory is allocated at a point you determine when you initialize the library, and it's size can easily be interrogated.

What does structured logging mean?



---

### Why Structured Logging Is Important

When the complexity of your problem space grows larger than you can hold in your head, you need tools to examine what's actually happening in your application and seperate your ___ from the noise. There are two common tools programmers reach for, their debugger, and printf. Logging is not a replacement for understanding what's going on with the memory in your program. Logging is however in my opionion, an easier way to understand what's happening in your program when control flow becomes extremely hard to follow.

What do I mean by difficult to follow control flow?

**A series of events that cross the boundary of a single callstack.**

Games are full of events that take place over multiple frames. Code may be triggered by animations, sounds, or other processes that take a long time to complete, and need to leav

B Distributed Systems.

 When you need to trace a chain of events that crosses the boundaries of a single process or machine, having structured logs with accurate timestamps is *incredibly helpful* for tracking down bugs.

---

### Notes 

Loggy has been used in production environments, and has gone through several major revisions before the public version was available.


 In order to support more engines, loggy targets a very old version of dotnet (netstandard2.0), and does not have access to some of the nicer to have features of C# such as `Span<T>` and source analyzers. There are probably ways to express what Loggy does that would allow for a smaller binary size, but we've chosen not to take that route in the name of supporting more engines (Unity *cough*).sugges