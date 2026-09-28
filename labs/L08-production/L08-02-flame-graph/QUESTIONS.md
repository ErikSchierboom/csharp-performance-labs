# Questions
1. Which function has the most **self** time? Which has the most **inclusive** time among *your* (`service!`) frames?
2. Write down the **call path** from `Main` to the hottest function.
3. What fraction of the time inside `HandleRequest` is spent in that function?
4. The function is reached on only a fraction of requests, yet dominates. What does that tell you about where to look: the *average* request or the *expensive minority*? What number would you use to prove it (counts vs time)?
5. Which "small, everywhere" functions look busy in the graph but barely matter? How can you tell?
