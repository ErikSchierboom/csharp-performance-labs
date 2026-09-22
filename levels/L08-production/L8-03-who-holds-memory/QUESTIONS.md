# Questions
1. Which type accounts for most **bytes**? Which type's **count** is growing between your two gcdumps? Are they the same type?
2. Some big things are *not* the leak. Which large objects are present but constant between the two dumps? How do you know they are not the problem?
3. What is the **GC root** of a leaked object (root → … → object)? Which *static field* or collection is it?
4. Two other collections exist and don't grow. What distinguishes them from the leaking one?
5. What would you tell the developers to change, in one sentence?
6. How would you prevent this from shipping (which metric or test would catch it)?
