# Branch And Bound Demos

## Algorithm
https://en.wikipedia.org/wiki/Branch_and_bound

The branch and bound (BnB) algorithm works on a tree-shaped search space where it finds the best solution, or one of the best solutions.
It works by traversing the tree with a depth-first search and remembering the best solution found so far.
If a partial solution already performs worse than the best one, the branch is pruned.
In order for this to work, the score of partial solution must only get worse.
The branches of this tree can be distributed among worker threads/tasks that search in parallel.

## Implementation

### IBnBProblem interface
I implemented a general `IBnBProblem` interface that represents a node in the search tree.
As a partial solution (a subtree in the search space) can be seen as an individual (sub-)problem, I treat them equally to the initial problem.
The interface asserts that the following methods are implemented:

* `CompareTo(IBnBProblem other)` to compare two nodes, i.e. two (partial) solutions.
* `Branch(IBnBProblem best)` to get the node's children. If they score worse than the best, they are pruned.
* `IsLeaf()` to check if the node is a full or a partial solution.
* `PrintProblem()` to print the initial problem that this node is a subproblem of.

The interface also overloads some comparison operators for a more readable comparison.

#### Implemented Problems
* Travelling Salesperson Problem. https://en.wikipedia.org/wiki/Travelling_salesman_problem
* Quadratic Assignment Problem. https://en.wikipedia.org/wiki/Quadratic_assignment_problem
* Flow-Shop Scheduling. https://en.wikipedia.org/wiki/Flow-shop_scheduling

### Parallel and asynchronous code
After selecting a problem and a problem size, the user chooses a number of threads (or runs benchmarks of different thread counts).

The `BnB` object then starts as many tasks as needed and combines them with `Task.WhenAll(tasks)`.
All tasks receive a `CancellationToken` to cancel the tasks when the user presses a key.
Also, they receive a `Progress<IBnBTask>` that makes them print the current global best solution should they have found a new one.

Every 100ms, the UI thread checks whether a key has been pressed or whether the tasks have completed.
Should a task complete in a faulted state, the combined task will complete.

### BnB Algorithm
A global queue holds the work for the worker tasks and is initialized with the initial `IBnBProblem` object.
Workers can dequeue work, but if the queue becomes empty, they call `Branch(IBnBProblem best)` to expand the last problem and fill the queue with work again if the method returned any subproblems.

Each worker has a local stack of `IBnBProblems` where it repeatedly pops the top one and checks if it is a partial or full solution using `IsLeaf()`.

Partial solutions are expanded using `Branch(IBnBProblem best)` and the results are pushed on the stack.

Full solutions are checked against the worker's personal best solution, which is equal to or worse than the global best solution.
This ensures that workers can prune bad (partial) solutions, but do not access the global queue all the time.
If the solution is better than the personal best, the worker checks if it is also better than the global best, in which case it updates the global best.
In any case, the personal best is synchronized with the global best as it is already accessing it anyway.

If the worker's stack is empty, it dequeues an `IBnBProblem` from the global queue, if the queue is also empty it completes.

The termination of all workers terminates the algorithm.
